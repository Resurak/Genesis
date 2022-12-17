using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gensis.Sync
{
    public class Request
    {
        public Request()
        {

        }

        public Request(RequestCode code)
        {
            this.Code = code;
        }

        public Request(RequestCode code, Guid token)
        {
            this.Code = code;
            this.Token = token;
        }

        public Request(RequestCode code, params string[] headers)
        {
            this.Code = code;
            this.Headers = headers;
        }

        public Guid Token { get; set; }
        public RequestCode Code { get; set; }

        public string[]? Headers { get; set; }
    }

    public enum RequestCode
    {
        Connection,
        Disconnection,

        Authentication,
        EncryptConnection,

        GetAvailableShares,
        GetAvailableVirtualStorage,

        SyncShare
    }
}
