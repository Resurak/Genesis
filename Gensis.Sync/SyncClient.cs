using Genesis.Commons;
using Genesis.Net;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gensis.Sync
{
    public class SyncClient : NetClient
    {
        public async Task Connect()
        {
            await base.Connect("127.0.0.1"); // test
            await Handshake();
        }

        public async Task<List<InfoShare>?> GetShares()
        {
            try
            {
                await SendRequest(RequestCode.GetAvailableShares);
                if (await GetResponse() is Response response)
                {
                    if (response.Code != ResponseCode.Accepted)
                    {
                        Log.Warning("Response got: {code}", response.Code);
                    }
                    else
                    {
                        return response.DeserializeParam<List<InfoShare>>();
                    }
                }
                else
                {
                    Log.Warning("Failed to receive response...");
                }

                return null;
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Exception thrown while getting available shares");
                return null;
            }
        }

        async Task SendRequest(RequestCode code, Guid token = default)
        {
            if (!Connected)
            {
                Log.Warning("Can't send request, client not connected");
                return;
            }

            try
            {
                await SendObject(new Request(code, token));
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Exception thrown while sending request");
            }
        }

        async Task<Response?> GetResponse()
        {
            if (!Connected)
            {
                Log.Warning("Can't send request, client not connected");
                return null;
            }

            try
            {
                return await ReceiveObject<Response>();
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Exception thrown while getting response");
                return null;
            }
        }

        async Task Handshake()
        {
            await SendRequest(RequestCode.Connection);
            if (await GetResponse() is Response response && response.Code == ResponseCode.Accepted)
            {
                return;
            }
            else
            {
                Log.Warning("Error while doing handshake with server, closing connection");
                base.Disconnect();
            }
        }

        public new async Task Disconnect()
        {
            await SendRequest(RequestCode.Disconnection);
            if (await GetResponse() is Response response && response.Code == ResponseCode.Accepted)
            {
                base.Disconnect();
            }
            else
            {
                Log.Warning("Error while disconnecting... forcing disconnection");
                base.Disconnect();
            }
        }
    }
}
