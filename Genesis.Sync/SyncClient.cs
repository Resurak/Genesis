using Genesis.Commons;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Sync
{
    public class SyncClient : TcpClient
    {
        public SyncClient() : base() { }

        NetStream? stream;

        public new bool Connected => stream != null;

        public List<Share> LocalShares { get; set; } = new List<Share>();
        public List<Share> ServerShares { get; set; } = new List<Share>();

        public async Task ConnectAsync()
        {
            await ConnectAsync("127.0.0.1", 6969);
            stream = new NetStream(this);

            Log.Information("Connected to server");
            await GetShareList();
        }

        public async Task CreateShare(string root)
        {
            var share = new Share(root);
            await share.Update();

            Log.Information("Created share with id {id} and root {root}", share.ID, share.RootFolder);
            LocalShares.Add(share);
        }

        public async Task GetShareList()
        {
            var shares = await stream.ReceiveObject();
            if (shares is List<Share> list)
            {
                ServerShares = list;
                Log.Information("Received share list from server");
            }
            else
            {
                Log.Warning("Could not receive share list from serverShare");
            }
        }

        public async Task SyncShares(Guid localShareID, Guid serverShareID)
        {
            var localShare = LocalShares.FirstOrDefault(x => x.ID == localShareID);
            var serverShare = ServerShares.FirstOrDefault(x => x.ID == serverShareID);

            if (localShare == null)
            {
                Log.Warning("Could not find localShare share with id {id}", localShareID);
                return;
            }

            if (serverShare == null)
            {
                Log.Warning("Could not find serverShare share with id {id}", serverShareID);
                return;
            }

            Log.Information("Preparing local share to sync");
            localShare.PrepareShare(serverShare);

            foreach (var path in localShare.PathList)
            {
                var absolutePath = path.AbsolutePath(localShare.RootFolder);

                if (path.Flag_Sync)
                {
                    switch (path.Type)
                    {
                        case PathType.File:
                        {
                            var data = new FileData() { FileID = path.ID, ShareID = serverShare.ID };
                            await stream.SendObject(data);

                            if (!await IsRequestAccepted())
                            {
                                Log.Warning("Can't download file with id {id} from server", path.ID);
                                continue;
                            }

                            await stream.ReceiveFile(absolutePath, path.Size, path.ID);
                            Log.Information("Received {path}", absolutePath);

                            break;
                        }
                        case PathType.Folder:
                        {
                            if (!Directory.Exists(absolutePath))
                            {
                                Directory.CreateDirectory(absolutePath);
                            }

                            break;
                        }
                    }
                }

                if (path.Flag_Delete)
                {
                    switch (path.Type)
                    {
                        case PathType.File:
                            if (File.Exists(absolutePath))
                                File.Delete(absolutePath);
                            break;
                        case PathType.Folder:
                            if (Directory.Exists(absolutePath))
                                Directory.Delete(absolutePath, true);
                            break;
                    }
                }
            }
        }

        async Task<bool> IsRequestAccepted()
        {
            var obj = await stream.ReceiveObject();
            return obj is Guid id && id == Utils.AcceptedToken;
        }

        public async Task Disconnect()
        {
            if (!Connected)
            {
                return;
            }

            await stream.SendObject(Utils.DisconnectToken);

            stream?.Dispose();
            this.Close();
        }
    }
}
