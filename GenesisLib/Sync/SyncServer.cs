using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib.Sync
{
    public class SyncServer : SyncBase
    {
        TcpServer? Server;

        public bool Active => Server != null && Server.Active;

        public void Start()
        {
            Server = new TcpServer();
            Server.Start();
        }

        public async Task WaitClient()
        {
            if (!Active)
            {
                Log.Warning("Server not active, can't wait client");
                return;
            }

            if (ClientConnected)
            {
                Log.Warning("A client is already connected");
                return;
            }

            Client = await Server.AcceptTcpClientAsync();
            Stream = new TcpStream(Client);

            Connected?.Invoke();
        }

        public async Task ReceiveRequest()
        {
            var temp = await Stream.ReceiveObject();
            if (temp is SyncCommand command)
            {
                switch (command)
                {
                    case SyncCommand.Handshake:
                        break;
                    case SyncCommand.Disconnect:
                        break;
                    case SyncCommand.Upload_ShareList:
                        break;
                    case SyncCommand.GetShareList:
                        await Stream.SendObject(LocalShares);
                        break;
                    case SyncCommand.FileData:
                        break;
                    case SyncCommand.FileDataList:
                        break;
                    case SyncCommand.ShareSync:
                        break;
                    case SyncCommand.ShareSync_Accepted:
                        break;
                    case SyncCommand.ShareSync_Rejected:
                        break;
                    case SyncCommand.ShareSync_Completed:
                        break;
                }
            }
            else if (temp is ShareData data)
            {
                await ProcessShareSync(data);
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
                foreach (var file in data.Items)
                {
                    var path = Path.Combine(share.Root, file.Path);
                    var fileSync = new FileSyncStream(path, file.Size, FileMode.Open);

                    while (!fileSync.Completed)
                    {
                        var fileData = fileSync.GetData();
                        // come back here
                    }
                }
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

    }
}
