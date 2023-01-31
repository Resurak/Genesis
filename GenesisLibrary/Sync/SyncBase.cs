using GenesisLibrary.Exceptions;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLibrary.Sync
{
    public class SyncBase : IDisposable
    {
        public SyncBase(string filePath = "localShares.json")
        {
            ShareFilePath = filePath;

            LocalShareList = new SyncItemList<SyncShare>();
            RemoteShareList = new SyncItemList<SyncShare>();

            Load();

            WorkingThread = new Thread(async () => await WaitData());
        }

        protected TcpClient? Client;
        protected TcpStream? Stream;

        protected Thread WorkingThread;
        protected SyncManager? SyncManager;

        public StatusUpdateEventHandler? Connected;
        public StatusUpdateEventHandler? Disconnected;
        public SyncProgressEventHandler? SyncProgress;

        public bool ClientConnected => Client != null && Stream != null;

        public SyncItemList<SyncShare> LocalShareList { get; set; }
        public SyncItemList<SyncShare> RemoteShareList { get; set; }

        public string ShareFilePath { get; set; } = string.Empty;

        public void Load()
        {
            Log.Information("Loading {x} from {path}", nameof(LocalShareList), ShareFilePath);

            try
            {
                //var data = File.ReadAllBytes(ShareFilePath);
                //var obj = data.FromPack();

                var json = File.ReadAllText(ShareFilePath);
                var obj = json.FromJson<SyncItemList<SyncShare>>();

                if (obj != null)
                {
                    LocalShareList = obj;
                }
                else
                {
                    throw new Exception("Invalid json in file " + ShareFilePath + "\ntype: " + obj.GetType());
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Error while loading {x}", nameof(LocalShareList));
            }
        }

        public void Save()
        {
            Log.Information("Saving {x} to {path}", nameof(LocalShareList), ShareFilePath);

            try
            {
                //var data = LocalShareList.ToPack();
                var json = LocalShareList.ToJson();
                if (File.Exists(ShareFilePath))
                {
                    File.Delete(ShareFilePath);
                }

                //File.WriteAllBytes(ShareFilePath, data);
                File.WriteAllText(ShareFilePath, json);
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

            var data = local.CompareShare(source);

            SyncManager = new SyncManager(local);
            await Stream.SendObject(data);
        }

        async Task WaitData()
        {
            while (ClientConnected)
            {
                try
                {
                    CheckConnection();

                    var obj = await Stream.ReceiveObject();
                    if (obj is SyncItemList<SyncShare> syncShareList)
                    {
                        Log.Information("Received remote share list");

                        RemoteShareList = syncShareList;
                        await Stream.SendObject(LocalShareList);

                        continue;
                    }

                    if (obj is SyncShareData syncShareData)
                    {
                        Log.Information("Received sync share data");

                        var local = LocalShareList[syncShareData.ID];
                        if (local == null)
                        {
                            Log.Warning("No local sync share with id {id}", syncShareData.ID);
                            continue;
                        }

                        local.SyncPathList = SyncManager.GetFilesToSync(local, syncShareData.IDList);
                        Log.Information("Sending files");

                        SyncManager = new SyncManager(local);
                        await SyncManager.SendFileData(Stream);

                        Log.Information("All requested files sent");

                        SyncManager = null;
                        continue;
                    }

                    if (obj is List<FileData> fileDataList)
                    {
                        if (SyncManager == null)
                        {
                            Log.Warning("Received fileDataList, but no share to sync, skipping");
                            continue;
                        }

                        await SyncManager.ProcessFileData(fileDataList, new Progress<SyncProgress>(x => SyncProgress?.Invoke(x)));
                        continue;
                    }

                    if (obj is FileData fileData)
                    {
                        if (SyncManager == null)
                        {
                            Log.Warning("Received fileData, but no share to sync, skipping");
                            continue;
                        }

                        await SyncManager.ProcessFileData(fileData, new Progress<SyncProgress>(x => SyncProgress?.Invoke(x)));
                        continue;
                    }

                    Log.Warning("Unknown object received, skipping");
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Exception thrown while waiting data");
                }
            }
        }

        protected void CheckConnection()
        {
            if (!ClientConnected)
            {
                Log.Warning("Can't execute action. Client not connected");
                Disconnect();
                //throw new ConnectionException(ConnectionExceptionCode.NotConnected);
            }
        }

        protected void OnConnected()
        {
            WorkingThread.Start();
            Connected?.Invoke();
        }

        protected void OnDisconnected()
        {
            if (WorkingThread.ThreadState == ThreadState.Running)
            {
                WorkingThread.Join();
            }

            Disconnected?.Invoke();
        }

        public void Disconnect()
        {
            Dispose();
            OnDisconnected();
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
