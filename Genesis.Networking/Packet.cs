using Genesis.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Networking
{
    public class Packet<T> where T : class
    {
        public Packet(T value) 
        {
            this.Param = value;
            this.Status = Status.NoError;
        }

        public Packet(Status status)
        {
            this.Status = status;
        }

        public Packet(ErrorCode code, Exception? ex = null)
        {
            this.Status = new Status(code, ex);
        }

        public T? Param { get; set; }
        public Status Status { get; set; }
    }

    public enum ErrorCode
    {
        NoError,

        DataError,
        SocketError,

        UnknownException,
        OperationCancelled,

        InvalidPacket,
        SerializationError,
        DeserializationError,
    }
}
