using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Sync
{
    public class SyncPacket
    {
        public Guid Token { get; set; }

        public PacketType Type { get; set; }
        public RequestCode Request { get; set; }
        public ResponseCode Response { get; set; }

        public Guid? DataID { get; set; }
        public Type? DataType { get; set; }
        public byte[]? DataValue { get; set; }

        public DateTime SentDate { get; set; }
        public DateTime ReceivedDate { get; set; }
    }

    public enum PacketType
    {
        Request,
        Response,
        Handshake,
    }

    public enum RequestCode
    {
        ShareList,
        ShareInfo,

        StorageList,
        StorageInfo,

        Sync_File,
        Sync_Storage
    }

    public enum ResponseCode
    {
        Accepted,
        Rejected,

        Error_InvalidToken,
        Error_InvalidParam,

        Error_NotEncrypted,
        Error_NotAuthorized,
        Error_NotAuthenticated,
    }
}
