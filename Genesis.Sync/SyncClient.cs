using Genesis.Commons;
using Genesis.Commons.Exceptions;
using Genesis.Sync.Exceptions;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Sync
{
    public sealed class SyncClient : BaseSync
    {
        public SyncClient() : base()
        {
            ServerShares = new ShareList();
        }

        TcpClient? Client;
        NetStream? NetStream;

        public event UpdateEventHandler? Connected;
        public event UpdateEventHandler? Disconnected;

        public ShareList ServerShares { get; set; }
        public bool IsConnected => Client != null && NetStream != null && NetStream.CanStream;

        public async Task ConnectAsync(string address)
        {
            if (IsConnected)
            {
                throw new InvalidOperationException("Client already connected. Disconnect first");
            }

            var ip = IPAddress.Parse(address);
            var endPoint = new IPEndPoint(ip, 6969);

            Client = new TcpClient();
            await Client.ConnectAsync(endPoint);

            NetStream = new NetStream(Client);
            Connected?.Invoke(this);
        }

        public async Task GetShareList()
        {
            await CheckConnection();
            await NetStream.SendObject(Tokens.GetShares);

            var obj = await NetStream.ReceiveObject();
            if (obj is ShareList list)
            {
                ServerShares = list;
                return;
            }

            throw new RejectedException();
        }

        public async Task SyncShare(ShareData localShare, ShareData serverShare, IProgress<PathData>? fileReceived = null)
        {
            await CheckConnection();
            localShare.PrepareShare(serverShare);

            var syncList = localShare.PathList.Where(x => x.Flag_Sync);
            var deleteList = localShare.PathList.Where(x => x.Flag_Delete);
            var incompleteList = new PathList();

            var progress = new SyncProgress(syncList.Sum(x => x.Size));
            OnProgress(progress);

            foreach (var data in syncList)
            {
                if (!IsConnected)
                {
                    incompleteList.Add(data);

                    progress.CurrentSyncBytes += data.Size;
                    OnProgress(progress);

                    continue;
                }

                var path = data.AbsolutePath(localShare.Root);
                switch (data.Type)
                {
                    case PathType.Folder:
                    {
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        break;
                    }
                    case PathType.File:
                    {
                        var fileData = new FileData(data.ID, serverShare.ID);
                        if (!await IsRequestAccepted(fileData))
                        {
                            Log.Warning("Server rejected download of file {path}", data.Path);
                            incompleteList.Add(data);

                            progress.CurrentSyncBytes += data.Size;
                            OnProgress(progress);

                            continue;
                        }

                        progress.CurrentFile = data;
                        progress.TotalFileBytes = data.Size;

                        try
                        {
                            var before = 0L;
                            await NetStream.ReceiveFile(path, data.Size, new Progress<long>(x =>
                            {
                                var diff = x - before;

                                progress.CurrentFileBytes = x;
                                progress.CurrentSyncBytes += diff;
                                OnProgress(progress);

                                before = x;
                            }));

                            fileReceived?.Report(data);
                        }
                        catch (Exception ex)
                        {
                            Log.Warning(ex, "Error while receiving {file}. Deleting", path);

                            if (File.Exists(path))
                            {
                                File.Delete(path);
                            }
                        }

                        break;
                    }
                }
            }

            progress.Stop();
            OnProgress(progress);

            foreach (var data in deleteList)
            {
                var path = data.AbsolutePath(localShare.Root);
                switch (data.Type)
                {
                    case PathType.File:
                        if (File.Exists(path))
                            File.Delete(path);
                        break;
                    case PathType.Folder:
                        if (Directory.Exists(path))
                            Directory.Delete(path, true);
                        break;
                }
            }

            if (incompleteList.Count > 0)
            {
                Log.Warning(new string('-', 50));

                Log.Warning("A total of {num} files had errors while being received", incompleteList.Count);
                Log.Warning("Full list:");

                var i = 1;
                foreach (var data in incompleteList)
                {
                    Log.Warning("\t\t[{i}]: {path} | {size}", i, data.Path, data.Size);
                    i++;
                }

                Log.Warning(new string('-', 50));
            }
        }

        public async Task Disconnect()
        {
            if (NetStream != null && NetStream.CanStream)
            {
                await NetStream.SendObject(Tokens.Disconnect);
            }

            if (NetStream == null && Client == null)
            {
                return;
            }

            NetStream?.Dispose();
            Client?.Close();

            NetStream = null;
            Client = null;

            Disconnected?.Invoke(this);
        }

        public async Task<bool> IsRequestAccepted(object obj)
        {
            if (!IsConnected)
            {
                return false;
            }

            await NetStream.SendObject(obj);

            var resp = await NetStream.ReceiveObject();
            return resp is Guid id && id == Tokens.Accepted;
        }

        async Task CheckConnection()
        {
            if (!IsConnected)
            {
                await Disconnect();
                throw new NotConnectedException();
            }
        }
    }
}
