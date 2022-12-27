using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Sync_Old
{
    public class SyncResponse
    {
        public byte[]? Data { get; set; }
        public ResponseCode Code { get; set; }
    }

    public enum ResponseCode
    {
        Accepted,
        Rejected,

        Error_InvalidToken,
        Error_InvalidParamID,
    }
}
