using Genesis.Commons;
using Genesis.Net;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Sync
{
    public sealed class SyncServer : NetServer
    {
        public List<DataShare> ShareList { get; set; }
        public List<DataStorage> StorageList { get; set; }

        public async Task StartService()
        {
            Log.Information("Starting SyncServer");

            base.Start();
            Log.Information("Server started");

            await base.WaitClient();
            Log.Information("Client connected. Accepting requests");

            await AcceptRequests();
            Log.Information("Client disconnected");
        }

        async Task AcceptRequests()
        {
            while (Connected)
            {
                var packet = await ReceivePacket();
                if (packet is not null)
                {
                    await ProcessPacket(packet);
                }
                else
                {
                    if (!Connected)
                    {
                        break;
                    }

                    Log.Warning("Received packet was null, maybe operation was cancelled");
                }
            }
        }

        async Task ProcessPacket(SyncPacket packet)
        {
            if (packet.Type != PacketType.Request)
            {
                Log.Warning("Received invalid packet type");
                return;
            }

            switch (packet.Request)
            {
                case RequestCode.ShareList:
                    await SendResponse(ResponseCode.Accepted, ShareList);
                    break;
                case RequestCode.ShareInfo:
                    var share = ShareList.FirstOrDefault(x => x.ID == (packet.DataID ?? Guid.Empty));
                    if (share != null)
                    {
                        await SendResponse(ResponseCode.Accepted, share);
                    }
                    else
                    {
                        await SendResponse(ResponseCode.Error_InvalidToken);
                    }
                    break;
                case RequestCode.StorageList:
                    await SendResponse(ResponseCode.Accepted, StorageList);
                    break;
                case RequestCode.StorageInfo:
                    var storage = StorageList.FirstOrDefault(x => x.ID == (packet.DataID ?? Guid.Empty));
                    if (storage != null)
                    {
                        await SendResponse(ResponseCode.Accepted, storage);
                    }
                    else
                    {
                        await SendResponse(ResponseCode.Error_InvalidToken);
                    }
                    break;
                case RequestCode.Sync_File:
                    break;
                case RequestCode.Sync_Storage:
                    break;
            }
        }

        async Task<SyncPacket?> ReceivePacket()
        {
            var data = await ReceiveData();
            if (data.Length == 0)
            {
                if (!Connected)
                {
                    Log.Warning("Client forced disconnection");
                    return null;
                }

                return null;
            }

            return Utils.Deserialize<SyncPacket>(data);
        }

        async Task SendResponse(ResponseCode code)
        {
            var packet = new SyncPacket();

            packet.Type = PacketType.Response;
            packet.Response = code;

            var data = Utils.Serialize(packet);
            await base.SendData(data);
        }

        async Task SendResponse<T>(ResponseCode code, T? obj = null) where T : class
        {
            var packet = new SyncPacket();

            packet.Type = PacketType.Response;
            packet.Response = code;

            if (obj != null)
            {
                packet.DataType = typeof(T);
                packet.DataValue = Utils.Serialize(obj);
            }

            var data = Utils.Serialize(packet);
            await base.SendData(data);
        }
    }
}
