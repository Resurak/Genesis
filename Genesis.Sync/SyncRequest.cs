using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Sync
{
    public class SyncRequest : BasePacket
    {
        public SyncRequest() { }

        public SyncRequest(RequestCode code)
        {
            Code = code;
        }

        public RequestCode Code { get; set; }
    }

    public enum RequestCode
    {
        Disconnection,

        ShareList,
        ShareInfo,

        StorageList,
        StorageInfo,

        Sync_File,
        Sync_Storage
    }
}
