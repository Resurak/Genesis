using Genesis.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Sync
{
    public class SyncPacket
    {
        public SyncPacket()
        {
            ID = Guid.NewGuid();
        }

        public SyncPacket(ResponseCode code)
        {
            Type = PacketType.Response;
            Response = code;
        }

        public SyncPacket(ResponseCode code, object obj) : this(code)
        {
            ObjectType = obj.GetType();
            ObjectData = Utils.Serialize(obj);
        }

        public SyncPacket(RequestCode code)
        {
            Type = PacketType.Request;
            Request = code;
        }

        public SyncPacket(RequestCode code, object obj) : this(code)
        {
            ObjectType = obj.GetType();
            ObjectData = Utils.Serialize(obj);
        }

        public Guid ID { get; set; }
        public PacketType Type { get; set; }

        public RequestCode Request { get; set; }
        public ResponseCode Response { get; set; }

        public Type? ObjectType { get; set; }
        public byte[]? ObjectData { get; set; }

        public static SyncPacket ResponseAccepted => new SyncPacket(ResponseCode.Accepted);
        public static SyncPacket ResponseRejected => new SyncPacket(ResponseCode.Rejected);
    }

    public enum PacketType
    {
        Unknown,

        Request,
        Response,
    }

    public enum RequestCode
    {
        Null,
        Disconnect,

        GetShareList,
        DownloadFile,
    }

    public enum ResponseCode
    {
        Null,

        Accepted,
        Rejected,
    }
}
