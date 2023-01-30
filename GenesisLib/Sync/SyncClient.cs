using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib.Sync
{
    public class SyncClient : SyncBase
    {
        public async Task Connect(string ip, int port = 6969)
        {
            var address = IPAddress.Parse(ip);
            var endPoint = new IPEndPoint(address, port);

            Client = new TcpClient();
            await Client.ConnectAsync(endPoint);

            Stream = new TcpStream(Client);
            Connected?.Invoke();
        }

        public async Task GetShareList()
        {
            Log.Information("Getting ShareList");
            await Stream.SendObject(SyncCommand.GetShareList);

            var data = await Stream.ReceiveObject();
            if (data is ItemList<Share> shareList)
            {
                RemoteShares = shareList;
            }
            else
            {
                Log.Warning("Invali ShareList received");
            }
        }

        public async Task ShareSync(Share localShare, Share sourceShare)
        {
            Log.Information("Requesting ShareSync between local {local} and source {source}", localShare.Root, sourceShare.Root);

            var itemList = localShare.CompareShare(sourceShare);
            var data = new ShareData(sourceShare.ID, itemList);

            SyncManager = new SyncManager(localShare, SyncOptions);
            await Stream.SendObject(data);

            var response = await Stream.ReceiveObject();
            if (response is SyncCommand command && command == SyncCommand.ShareSync_Accepted)
            {
                var temp = await Stream.ReceiveObject();
                if (temp is List<FileData> fileDataList)
                {
                    foreach (var item in fileDataList)
                    {
                        await SyncManager.ProcessIncoming(item);
                    }
                }
                else if (temp is FileData fileData)
                {
                    await SyncManager.ProcessIncoming(fileData);
                }
                else if (temp is SyncCommand commandd && command == SyncCommand.ShareSync_Completed)
                {
                    Log.Information("Share sync completed");
                }
                else
                {
                    Log.Warning("Unknown object received");
                }
            }
            else
            {
                SyncManager = null;
                Log.Warning("Unknown object received");
            }
        }
    }
}
