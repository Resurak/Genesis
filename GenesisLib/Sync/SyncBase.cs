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
            LocalShares = new ItemList<Share>();
            RemoteShares = new ItemList<Share>();

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

        public ItemList<Share> LocalShares { get; set; }
        public ItemList<Share> RemoteShares { get; set; }

        public UpdateEventHandler? Connected;
        public UpdateEventHandler? Disconnected;

        public PacketEventHandler? PacketReceived;
        public ExceptionEventHandler? ExceptionThrown;

        public SyncProgressEventHandler? SyncProgress;

        const string clientSharePath = "clientShares.json";
        const string serverSharePath = "serverShares.json";

        public async Task Update()
        {
            foreach (var share in LocalShares)
            {
                await share.Update();
            }

            Log.Information("All shares updated");
        }

        public async Task LoadClientShares() =>
            await LoadShares(clientSharePath);

        public async Task LoadServerShares() =>
            await LoadShares(serverSharePath);

        async Task LoadShares(string path)
        {
            if (!File.Exists(path))
            {
                Log.Warning("No local share list saved, skipping load");
                return;
            }

            var json = File.ReadAllText(path);
            var shareList = JsonConvert.DeserializeObject<ItemList<Share>>(json);
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

        public void SaveClient() =>
            Save(clientSharePath);

        public void SaveServer() =>
            Save(serverSharePath);

        void Save(string path)
        {
            var json = JsonConvert.SerializeObject(LocalShares, Formatting.Indented);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.WriteAllText(path, json);
            Log.Information("Server share list saved on {path}", new FileInfo(path).FullName);
        }

        public async Task AddShare(string root, bool isLocal = true)
        {
            Log.Information("Adding {root} to local share list", root);

            var share = new Share(root);
            await share.Update();

            LocalShares.Add(share);

            if (isLocal)
            {
                SaveClient();
            }
            else
            {
                SaveServer();
            }
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

                await Stream.SendObject(SyncCommand.ShareSync_Accepted);
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
                await Stream.SendObject(SyncCommand.ShareSync_Completed);
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
                await Stream.SendObject(SyncCommand.Disconnect, token.Token);
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
