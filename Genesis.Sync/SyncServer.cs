using Genesis.Commons;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Sync
{
    public sealed class SyncServer : CommonSync
    {
        public SyncServer()
        {
            this.ShareList = new DataList<DataShare>();
            this.StorageList = new DataList<DataStorage>();
        }

        TcpServer server;

        public DataList<DataShare> ShareList { get; set; }
        public DataList<DataStorage> StorageList { get; set; }

        public event UpdateEventHandler? ServerStarted;
        public event UpdateEventHandler? ServerStopped;

        public async Task StartService()
        {
            try
            {
                server = new TcpServer();
                server.Start();

                ServerStarted?.Invoke();

                client = await server.AcceptTcpClientAsync();
                stream = new DataStream(client);

                OnConnected();
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

        public void StopService()
        {
            if (!server.Active)
            {
                return;
            }

            Disconnect();

            server.Stop();
            ServerStopped?.Invoke();
        }

        public async Task AcceptRequests()
        {
            while (Connected)
            {
                var request = await ReceiveRequest();
                await ProcessRequest(request);
            }
        }

        async Task ProcessRequest(SyncRequest? request)
        {
            if (request == null)
            {
                OnError("Received request was null or invalid");
                return;
            }

            Log.Information("Received new {code} request", request.Code);
            switch (request.Code)
            {
                case RequestCode.ShareList:
                    await SendResponse(ResponseCode.Accepted, ShareList);
                    break;
                case RequestCode.ShareInfo:
                    await SendShare(request);
                    break;
                case RequestCode.StorageList:
                    await SendResponse(ResponseCode.Accepted, StorageList);
                    break;
                case RequestCode.StorageInfo:
                    await SendStorage(request);
                    break;
                case RequestCode.Sync_File:
                    break;
                case RequestCode.Sync_Storage:
                    break;
                case RequestCode.Disconnection:
                    base.Disconnect();
                    break;
            }
        }

        async Task SendShare(BasePacket packet)
        {
            if (packet.ParamValue is Guid id)
            {
                var share = ShareList[id];
                if (share != null)
                {
                    await SendResponse(ResponseCode.Accepted, share);
                }
                else
                {
                    await SendResponse(ResponseCode.Error_InvalidToken);
                }
            } 
            else
            {
                await SendResponse(ResponseCode.Error_InvalidParam);
            }
        }

        async Task SendStorage(BasePacket packet)
        {
            if (packet.ParamValue is Guid id)
            {
                var share = StorageList[id];
                if (share != null)
                {
                    await SendResponse(ResponseCode.Accepted, share);
                }
                else
                {
                    await SendResponse(ResponseCode.Error_InvalidToken);
                }
            }
            else
            {
                await SendResponse(ResponseCode.Error_InvalidParam);
            }
        }

        public async Task CreateStorageAndShare(string root)
        {
            var storage = await DataStorage.Create(root);
            StorageList.Add(storage);

            var share = new DataShare { ID = Guid.NewGuid(), Name = "testing " + new Random().Next(1, 10000), StorageID = storage.ID, StorageRoot = storage.Root };
            ShareList.Add(share);
        }

        public void CreateShare(DataStorage storage)
        {
            var share = new DataShare { ID = Guid.NewGuid(), Name = "testing " + new Random().Next(1, 10000), StorageID = storage.ID, StorageRoot = storage.Root };
            ShareList.Add(share);
        }
    }
}
