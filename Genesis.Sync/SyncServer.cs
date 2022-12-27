using Genesis.Commons;
using Genesis.Networking;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Sync_Old
{
    public class SyncServer : NetServer
    {
        public SyncServer() 
        {
            AvailableShares = new List<ShareData>();
            AvailableStorage = new List<Storage>();
        }

        public List<Storage> AvailableStorage { get; set; }
        public List<ShareData> AvailableShares { get; set; }

        public async Task StartService()
        {
            var status = Start();
            if (!status.OK)
            {
                Log.Warning(status.Exception, "Cannot start sync service. StatusCode: {code}", status.Code);
                return;
            }

            Log.Information("Service started. Waiting client");

            status = await WaitClient();
            if (!status.OK)
            {
                Log.Warning(status.Exception, "Cannot start sync service. StatusCode: {code}", status.Code);
                return;
            }

            status = await DoHandshake();
            if (!status.OK)
            {
                Log.Warning("Cannot start sync service. StatusCode: {code}", status.Code);
                return;
            }

            Log.Information("Client connected");
        }

        async Task<Status> DoHandshake()
        {
            var packet = await ReceivePacket();
            if (!packet.Status.OK || packet.Data == null)
            {
                Log.Error("Error while receiving handshake from client");
                Disconnect();

                return new Status(StatusCode.ConnectionError);
            }

            var item = await GetObject<Handshake>();
            if (item != null)
            {
                return Status.Success;
            }
            else
            {
                Log.Error("Error while receiving handshake from client");
                Disconnect();

                return new Status(StatusCode.DataError);
            }
        }

        async Task<T?> GetObject<T>() where T : class
        {
            var packet = await ReceivePacket();
            if (!packet.Status.OK || packet.Data == null)
            {
                if (packet.Status.Code == StatusCode.ConnectionError)
                {
                    Log.Warning("Client closed connection, disconnecting");
                    Disconnect();

                    return null;
                }
                else
                {
                    Log.Warning("Error while receiving object. {code}", packet.Status.Code);
                    return null;
                }
            }

            var data = Serialization.DeserializeObject(packet.Data);
            if (data is T item)
            {
                return item;
            }
            else
            {
                Log.Warning("Deserialized object invalid");
                return null;
            }
        }
    }
}
