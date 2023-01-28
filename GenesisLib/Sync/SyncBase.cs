using GenesisLib.Exceptions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib.Sync
{
    public delegate Task PacketEventHandler(SyncPacket packet);
    public delegate void SyncProgressEventHandler(SyncProgress? progress);

    public class SyncBase
    {
        public SyncBase() 
        {
            LocalShares = new GuidItemList<Share>();
            RemoteShares = new GuidItemList<Share>();

            SyncOptions = new SyncOptions();
            FileSyncList = new PathItemList<FileSyncStream>();
        }

        protected TcpStream? Stream;
        protected TcpClient? Client;

        protected SyncProgress? Progress;
        protected PathItemList<FileSyncStream> FileSyncList;

        public SyncOptions SyncOptions { get; set; }
        public SyncManager? SyncManager { get; set; }
        public bool IsConnected => Client != null && Stream != null;

        public GuidItemList<Share> LocalShares { get; set; }
        public GuidItemList<Share> RemoteShares { get; set; }

        public UpdateEventHandler? Connected;
        public UpdateEventHandler? Disconnected;

        public PacketEventHandler? PacketReceived;
        public ExceptionEventHandler? ExceptionThrown;

        public SyncProgressEventHandler? SyncProgress;

        public void Update()
        {
            // todo
        }

        public void Load()
        {
            // todo
        }

        public void Save() 
        {
            // todo
        }

        public async Task GetShareList()
        {
            Log.Information("Downloading ShareList");
            await SendPacket(PacketType.Download_ShareList);
        }

        public async Task ShareSync(Share localShare, Share sourceShare)
        {
            Log.Information("Requesting ShareSync between local {local} and source {source}", localShare.Root, sourceShare.Root);

            var itemList = localShare.CompareShare(sourceShare);
            var data = new ShareData(sourceShare.ID, itemList);

            SyncManager = new SyncManager(localShare, SyncOptions);
            await SendPacket(PacketType.ShareSync, data);

            Progress = new SyncProgress(itemList.Sum(x => x.Size));
            SyncProgress?.Invoke(Progress);
        }

        protected async Task SendPacket(PacketType type, object? obj = null)
        {
            try
            {
                CheckConnection();
                var packet = new SyncPacket(obj, type);

                await Stream.SendObject(packet);
            }
            catch (Exception ex)
            {
                await CheckException(ex);
            }
        }

        protected async Task ReceivePacketsLoop()
        {
            while (IsConnected)
            {
                try
                {
                    CheckConnection();

                    var data = await Stream.ReceiveObject();
                    if (data is SyncPacket packet)
                    {
                        await ProcessPacket(packet);
                    }
                    else
                    {
                        Log.Warning("Unknown data received");
                    }
                }
                catch (Exception ex)
                {
                    await CheckException(ex);
                }
            }

            await Disconnect();
        }

        async Task ProcessPacket(SyncPacket packet)
        {
            switch (packet.Command)
            {
                case PacketType.Handshake:
                {
                    // nothing
                    break;
                }
                case PacketType.Disconnect:
                {
                    await Disconnect();
                    break;
                }
                case PacketType.Upload_ShareList:
                {
                    if (packet.Payload is GuidItemList<Share> shareList)
                    {
                        Log.Information("ShareList received");
                        RemoteShares = shareList;
                    }
                    else
                    {
                        Log.Warning("Received invalid share list");
                    }

                    break;
                }
                case PacketType.Download_ShareList:
                {
                    await SendPacket(PacketType.Upload_ShareList, LocalShares);
                    break;
                }
                case PacketType.FileData:
                {
                    if (packet.Payload is FileData data)
                    {
                        await ProcessFileData(data);
                    }
                    else
                    {
                        Log.Warning("Received invalid file data");
                    }

                    break;
                }
                case PacketType.FileDataList:
                {
                    if (packet.Payload is List<FileData> fileDataList)
                    {
                        foreach (var fileData in fileDataList)
                        {
                            await ProcessFileData(fileData);
                        }
                    }
                    else
                    {
                        Log.Warning("Received invalid file data list");
                    }

                    break;
                }
                case PacketType.ShareSync:
                {
                    if (packet.Payload is ShareData shareData)
                    {
                        await ProcessShareSync(shareData);
                    }
                    else
                    {
                        Log.Warning("Received invalid share data");
                        await SendPacket(PacketType.ShareSync_Rejected);
                    }

                    break;
                }
                case PacketType.ShareSync_Accepted:
                {
                    Log.Information("ShareSync request accepted");
                    break;
                }
                case PacketType.ShareSync_Rejected:
                {
                    Log.Warning("ShareSync request rejected");
                    SyncManager = null;

                    Progress = null;
                    SyncProgress?.Invoke(null);
                    break;
                }
                case PacketType.ShareSync_Completed:
                {
                    Log.Information("ShareSync completed");

                    Progress?.Update(Progress.TotalSize);
                    SyncProgress?.Invoke(Progress);

                    break;
                }
            }
        }

        async Task ProcessFileData(FileData fileData)
        {
            if (SyncManager == null)
            {
                Log.Warning("No share to sync, can't process file data");
                return;
            }

            await SyncManager.ProcessIncoming(fileData);

            Progress?.Update(fileData.Data.Length);
            SyncProgress?.Invoke(Progress);
        }

        async Task ProcessShareSync(ShareData data)
        {
            var share = LocalShares[data.ID];
            if (share == null)
            {
                Log.Warning("No share with id {id}, rejecting share sync", data.ID);
                return;
            }

            await SendPacket(PacketType.ShareSync_Accepted);
            SyncManager = new SyncManager(share, SyncOptions);

            var fileDataList = new List<FileData>();
            await foreach (var fileData in SyncManager.EnumerateFileData(data.Items))
            {
                var total = fileDataList.Sum(x => x.Data.Length);
                if (total > SyncOptions.StreamBufferLength)
                {
                    await SendPacket(PacketType.FileDataList, fileDataList);
                    fileDataList.Clear();
                }

                fileDataList.Add(fileData);
            }

            await SendPacket(PacketType.ShareSync_Completed);
        }


        void CheckConnection()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Can't receive/send data because connection is closed");
            }
        }

        async Task CheckException(Exception ex)
        {
            if (ex is OperationCanceledException)
            {
                Log.Debug("Stream operation cancelled");
            }
            else
            {
                ExceptionThrown?.Invoke(ex);
                await Disconnect();
            }
        }

        public async Task Disconnect()
        {
            if (!IsConnected)
            {
                return;
            }

            try
            {
                using var token = new CancellationTokenSource(2000);
                await Stream.SendObject(PacketType.Disconnect, token.Token);
            }
            catch
            {
                Log.Debug("Failed to send disconnect packet. Maybe already disconnected or socket error");
            }

            Stream?.Dispose();
            Client?.Dispose();

            Stream = null;
            Client = null;

            Disconnected?.Invoke();
        }
    }
}
