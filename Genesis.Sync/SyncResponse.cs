using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Sync
{
    public class SyncResponse : BasePacket
    {
        public SyncResponse() { }

        public SyncResponse(ResponseCode code)
        {
            Code = code;
        }

        public ResponseCode Code { get; set; }

        public bool IsSuccess =>
            Code == ResponseCode.Accepted;
    }

    public enum ResponseCode
    {
        Invalid,

        Accepted,
        Rejected,

        Error_InvalidToken,
        Error_InvalidParam,

        Error_NotEncrypted,
        Error_NotAuthorized,
        Error_NotAuthenticated,
    }
}
