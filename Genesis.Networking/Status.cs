using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Networking
{
    public class Status
    {
        public Status(StatusCode errorCode = StatusCode.Success, Exception? exception = null)
        {
            this.Code = errorCode;
            this.Exception = exception;
        }

        public bool OK => this.Code == StatusCode.Success;

        public StatusCode Code { get; set; }
        public Exception? Exception { get; set; }

        public static Status Success => new Status(StatusCode.Success, null);
    }

    public enum StatusCode
    {
        Success,

        InvalidOperation,

        DataError,
        ConnectionError,

        UnknownException,
        OperationCancelled,

        InvalidPacket,
        SerializationError,
        DeserializationError,
    }
}