using GenesisLibrary.Exceptions;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLibrary.Sync
{
    public class SyncBase : IDisposable
    {
        public SyncBase() 
        {
            LocalShareList = new SyncItemList<SyncShare>();
            RemoteShareList = new SyncItemList<SyncShare>();

            Load();

            NetWorker.DoWork += this.NetWorker_DoWork;
            //NetWorker.RunWorkerAsync()
        }

        private void NetWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            throw new NotImplementedException();
        }

        BackgroundWorker NetWorker;

        protected TcpClient? Client;
        protected TcpStream? Stream;
        protected SyncShare? ShareToSync;

        public event ReceivedObjectEventHandler? ReceivedObject;

        public bool ClientConnected => Client != null && Stream != null;

        public SyncItemList<SyncShare> LocalShareList { get; set; }
        public SyncItemList<SyncShare> RemoteShareList { get; set; }

        public string ShareFilePath { get; set; } = "localShares.json";

        public void Load()
        {
            Log.Verbose("Loading {x} from {path}", nameof(LocalShareList), ShareFilePath);

            try
            {
                var json = File.ReadAllText(ShareFilePath);
                var obj = json.FromJson();

                if (obj is SyncItemList<SyncShare> list)
                {
                    LocalShareList = list;
                }
                else
                {
                    throw new Exception("Invalid json in file " + ShareFilePath);
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Error while loading {x}", nameof(LocalShareList));
            }
        }

        public void Save()
        {
            Log.Verbose("Saving {x} to {path}", nameof(LocalShareList), ShareFilePath);

            try
            {
                var json = LocalShareList.ToJson();
                if (File.Exists(ShareFilePath))
                {
                    File.Delete(ShareFilePath);
                }

                File.WriteAllText(json, ShareFilePath);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Error while saving {x}", nameof(LocalShareList));
            }
        }

        public async Task UpdateAll()
        {
            Log.Verbose("Updating all shares");

            foreach (var share in LocalShareList)
            {
                await share.Update();
            }
        }

        public async Task AddShare(string root)
        {
            var share = new SyncShare(root);
            await share.Update();

            LocalShareList.Add(share);

            Log.Information("New share created. ID: {id} || Path: {path}", share.ID, share.Path);
            Save();
        }

        public async Task RequestShareList()
        {
            CheckConnection();

            Log.Verbose("Sending share list");
            await Stream.SendObject(LocalShareList);
        }

        public async Task RequestSync(SyncShare local, SyncShare source)
        {
            CheckConnection();
            Log.Verbose("Sending share to sync");

            local.CompareShare(source);
            ShareToSync = local;

            var data = new SyncShareData(local.RemoteShare);
            await Stream.SendObject(data);
        }

        public async Task WaitData(bool recursive = true)
        {
            try
            {
                CheckConnection();

                var obj = await Stream.ReceiveObject();
                if (obj == null)
                {
                    Log.Warning("Received invalid object");
                }
                else
                {
                    await ProcessRequest(obj);
                }

                if (recursive)
                {
                    await WaitData(recursive);
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Exception thrown while accepting request");
            }
        }

        protected async Task ProcessRequest(object obj)
        {
            if (obj is SyncItemList<SyncShare> syncShareList)
            {
                Log.Information("Received {x}", nameof(RemoteShareList));
                RemoteShareList = syncShareList;

                return;
            }

            if (obj is SyncShareData shareData)
            {
                Log.Information("Received sync share request. Requested share:\nID: {id} || Path: {path}", shareData.ID, shareData.Path);
                await SyncManager.SendFiles(shareData, Stream);

                return;
            }

            if (obj is List<FileData> fileDataList)
            {
                if (ShareToSync == null)
                {
                    Log.Warning("No share to sync, disposing received file data");
                    return;
                }

                foreach (var data in fileDataList)
                {
                    await SyncManager.ProcessFileData(ShareToSync, data);
                }

                return;
            }

            if (obj is FileData fileData)
            {
                if (ShareToSync == null)
                {
                    Log.Warning("No share to sync, disposing received file data");
                    return;
                }

                await SyncManager.ProcessFileData(ShareToSync, fileData);
                return;
            }

            Log.Warning("Unknown object received, skipping");
        }

        protected void CheckConnection()
        {
            if (!ClientConnected)
            {
                throw new ConnectionException(ConnectionExceptionCode.NotConnected);
            }
        }

        public void Dispose()
        {
            Log.Verbose("Disposing {x}", nameof(SyncBase));

            Stream?.Dispose();
            Client?.Dispose();

            Stream = null;
            Client = null;
        }
    }
}
