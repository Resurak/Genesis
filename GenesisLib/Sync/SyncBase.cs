using GenesisLib.Exceptions;
using Newtonsoft.Json;
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
        public bool ClientConnected => Client != null && Stream != null;

        public GuidItemList<Share> LocalShares { get; set; }
        public GuidItemList<Share> RemoteShares { get; set; }

        public UpdateEventHandler? Connected;
        public UpdateEventHandler? Disconnected;

        public PacketEventHandler? PacketReceived;
        public ExceptionEventHandler? ExceptionThrown;

        public SyncProgressEventHandler? SyncProgress;

        public async Task Update()
        {
            foreach (var share in LocalShares)
            {
                await share.Update();
            }

            Log.Information("All shares updated");
        }

        public async Task LoadClientShares()
        {
            var path = "clientShares.json";
            if (!File.Exists(path))
            {
                Log.Warning("No local share list saved, skipping load");
                return;
            }

            var json = File.ReadAllText(path);
            var shareList = JsonConvert.DeserializeObject<GuidItemList<Share>>(json);
            if (shareList != null)
            {
                Log.Information("Local share list loaded");
                LocalShares = shareList;

                await Update();
            }
            else
            {
                Log.Warning("Invalid share list, skipping load");
            }
            // todo
        }

        public async Task LoadServerShares()
        {
            var path = "serverShares.json";
            if (!File.Exists(path))
            {
                Log.Warning("No local share list saved, skipping load");
                return;
            }

            var json = File.ReadAllText(path);
            var shareList = JsonConvert.DeserializeObject<GuidItemList<Share>>(json);
            if (shareList != null)
            {
                Log.Information("Local share list loaded");
                LocalShares = shareList;

                await Update();
            }
            else
            {
                Log.Warning("Invalid share list, skipping load");
            }
            // todo
        }

        public void SaveLocal() 
        {
            var path = "clientShares.json";
            var json = JsonConvert.SerializeObject(LocalShares, Formatting.Indented);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.WriteAllText(path, json);
            Log.Information("Local share list saved on {path}", new FileInfo(path).FullName);
        }

        public void SaveServer()
        {
            var path = "serverShares.json";
            var json = JsonConvert.SerializeObject(LocalShares, Formatting.Indented);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.WriteAllText(path, json);
            Log.Information("Server share list saved on {path}", new FileInfo(path).FullName);
        }

        public async Task AddShare(string root, bool local = true)
        {
            Log.Information("Adding {root} to local share list", root);

            var share = new Share(root);
            await share.Update();

            LocalShares.Add(share);

            if (local)
            {
                SaveLocal();
            }
            else
            {
                SaveServer();
            }
        }

        public async Task GetShareList()
        {
            Log.Information("Downloading ShareList");
            await Stream.SendObject(PacketType.GetShareList);
        }

        public async Task ShareSync(Share localShare, Share sourceShare)
        {
            Log.Information("Requesting ShareSync between local {local} and source {source}", localShare.Root, sourceShare.Root);

            var itemList = localShare.CompareShare(sourceShare);
            var data = new ShareData(sourceShare.ID, itemList);

            SyncManager = new SyncManager(localShare, SyncOptions);
            await Stream.SendObject(data);

            Progress = new SyncProgress(itemList.Sum(x => x.Size));
            SyncProgress?.Invoke(Progress);
        }

        //protected async Task SendPacket(PacketType type, object? obj = null)
        //{
        //    try
        //    {
        //        CheckConnection();
        //        var packet = new SyncPacket(obj, type);

        //        await Stream.SendObject(packet);
        //    }
        //    catch (Exception ex)
        //    {
        //        await CheckException(ex);
        //    }
        //}

        protected async Task ReceivePacketsLoop()
        {
            while (ClientConnected)
            {
                try
                {
                    CheckConnection();

                    var data = await Stream.ReceiveObject();
                    if (data is PacketType type)
                    {
                        await ProcessPacket(type);
                        continue;
                    }

                    if (data is GuidItemList<Share> shareList)
                    {
                        RemoteShares = shareList;
                        continue;
                    }

                    if (data is ShareData shareData)
                    {
                        await ProcessShareSync(shareData);
                        continue;
                    }

                    if (data is FileData fileData)
                    {
                        await ProcessFileData(fileData);
                        continue;
                    }

                    if (data is List<FileData> fileDataList)
                    {
                        if (SyncManager != null)
                        {
                            foreach (var file in fileDataList)
                            {
                                await SyncManager.ProcessIncoming(file);
                            }
                        }

                        continue;
                    }

                    Log.Warning("{@obj}", data);
                    Log.Warning("Unknown data received");
                }
                catch (Exception ex)
                {
                    await CheckException(ex);
                }
            }

            await Disconnect();
        }


        async Task ProcessPacket(PacketType type) =>
            await ProcessPacket(new SyncPacket(null, type));

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
                case PacketType.GetShareList:
                {
                    await Stream.SendObject(LocalShares);
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
            try
            {
                var share = LocalShares[data.ID];
                if (share == null)
                {
                    Log.Warning("No share with id {id}, rejecting share sync", data.ID);
                    return;
                }

                await Stream.SendObject(PacketType.ShareSync_Accepted);
                SyncManager = new SyncManager(share, SyncOptions);

                var fileDataList = new List<FileData>();
                await foreach (var fileData in SyncManager.EnumerateFileData(data.Items))
                {
                    var total = fileDataList.Sum(x => x.Data.Length);
                    if (total > SyncOptions.StreamBufferLength)
                    {
                        await Stream.SendObject(fileDataList);
                        fileDataList.Clear();
                    }

                    fileDataList.Add(fileData);
                }

                if (fileDataList.Count > 0)
                {
                    await Stream.SendObject(fileDataList);
                    fileDataList.Clear();
                }
                await Stream.SendObject(PacketType.ShareSync_Completed);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "error...");
            }
        }


        void CheckConnection()
        {
            if (!ClientConnected)
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
            if (!ClientConnected)
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
