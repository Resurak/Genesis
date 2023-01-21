using Genesis.Commons;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Sync
{
    public sealed class SyncClient : CommonSync
    {
        public async Task ConnectLan(string address = "127.0.0.1", int port = 6969)
        {
            try
            {
                var ip = IPAddress.Parse(address);
                var endPoint = new IPEndPoint(ip, port);

                client = new TcpClient();
                await client.ConnectAsync(endPoint);

                stream = new DataStream(client);
                OnConnected();
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

        public async Task<DataList<Share>?> GetShares()
        {
            await SendRequest(RequestCode.ShareList);
            return await GetObject<DataList<Share>>();
        }

        public async Task<DataList<DataStorage>?> GetStorage()
        {
            await SendRequest(RequestCode.StorageList);
            return await GetObject<DataList<DataStorage>>();
        }

        public async Task SyncShare(Share localShare, Share serverShare)
        {

        }

        async Task<SyncResponse> GetResponse() =>
            await ReceiveResponse() ?? new SyncResponse(ResponseCode.Invalid);

        async Task<T?> GetObject<T>() where T : class
        {
            var response = await GetResponse();
            if (!response.IsSuccess)
            {
                OnError("Request not accepted. Response: " + response.Code);
                return null;
            }

            if (response.ParamValue is T param)
            {
                return param;
            }
            else
            {
                OnError("Received invalid list");
                return null;
            }
        } 

        public new async Task Disconnect()
        {
            await SendRequest(RequestCode.Disconnection);
            base.Disconnect();
        }
    }
}
