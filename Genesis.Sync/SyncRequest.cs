using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Sync_Old
{
    public class SyncRequest
    {
        public Guid Token { get; set; }
        public Guid ParamID { get; set; }

        public byte[]? Data { get; set; }
        public RequestCode Code { get; set; }
    }

    public enum RequestCode
    {
        ShareList,
        Disconnect,

        Sync_File,
        Sync_Share,
    }
}
