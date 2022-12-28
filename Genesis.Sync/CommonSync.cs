using Genesis.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Genesis.Sync
{
    public class CommonSync : ConnectionBase
    {
        public async Task SendRequest(RequestCode code)
        {
            var request = new SyncRequest(code);
            await SendObject(request);
        } 

        public async Task SendRequest<T>(RequestCode code, T? param = null) where T : class
        {
            var request = new SyncRequest(code);
            if (param != null)
            {
                request.WithParam(param);
            }

            await SendObject(request);
        }

        public async Task SendResponse(ResponseCode code)
        {
            var response = new SyncResponse(code);
            await SendObject(response);
        }

        public async Task SendResponse<T>(ResponseCode code, T? param = null) where T : class
        {
            var response = new SyncResponse(code);  
            if (param != null)
            {
                response.WithParam(param);
            }

            await SendObject(response);
        }

        public async Task<SyncRequest?> ReceiveRequest() =>
            await ReceiveGeneric<SyncRequest>();

        public async Task<SyncResponse?> ReceiveResponse() =>
            await ReceiveGeneric<SyncResponse>();
    }
}
