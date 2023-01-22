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
    public class SyncServer : TcpServer
    {
        public SyncServer() : base() { }

        TcpClient? client;
        NetStream? stream;

        public bool Connected => client != null && stream != null;
        public List<Share> LocalShares { get; set; } = new List<Share>();

        public async Task CreateShare(string root)
        {
            var share = new Share(root);
            await share.Update();

            Log.Information("Created share with id {id} and root {root}", share.ID, share.RootFolder);
            LocalShares.Add(share);
        }

        public async Task WaitClient()
        {
            client = await AcceptTcpClientAsync();
            stream = new NetStream(client);

            Log.Information("Client connected, sending shares");
            await stream.SendObject(LocalShares);
        }

        public async Task ReceiveRequests()
        {
            while (Connected)
            {
                var obj = await stream.ReceiveObject();
                if (obj is FileData data)
                {
                    await ProcessFileRequest(data);
                    continue;
                }
                
                if (obj is Guid id && id == Utils.DisconnectToken)
                {
                    Disconnect();
                    continue;
                }

                Log.Information("Unknonw request, skipping");
            }

            Disconnect();
        }

        async Task ProcessFileRequest(FileData data)
        {
            var share = LocalShares.FirstOrDefault(x => x.ID == data.ShareID);
            if (share == null)
            {
                Log.Warning("Invalid share id provided, skipping");

                await stream.SendObject("Invalid share id provided");
                return;
            }

            var file = share.PathList.FirstOrDefault(x => x.ID == data.FileID);
            if (file == null)
            {
                Log.Warning("Invalid file id provided, skipping");

                await stream.SendObject("Invalid file id provided");
                return;
            }

            if (file.Type == PathType.Folder)
            {
                Log.Warning("File requested is a folder, skipping");

                await stream.SendObject("File requested is a folder");
                return;
            }

            Log.Information("Client requested {file}, sending {num} bytes", file.AbsolutePath(share.RootFolder), file.Size);

            await stream.SendObject(Utils.AcceptedToken);
            await stream.SendFile(file.AbsolutePath(share.RootFolder));
        }

        public void Disconnect()
        {
            if (!Connected)
            {
                return;
            }

            stream?.Dispose();
            client?.Dispose();

            stream = null;
            client = null;

            Log.Information("Client disconnected");
        }
    }
}
