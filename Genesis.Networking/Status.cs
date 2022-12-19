using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Networking
{
    public class Status
    {
        public Status(ErrorCode errorCode = ErrorCode.NoError, Exception? exception = null)
        {
            this.ErrorCode = errorCode;
            this.Exception = exception;
        }

        public bool Success => this.ErrorCode == ErrorCode.NoError;

        public ErrorCode ErrorCode { get; set; }
        public Exception? Exception { get; set; }

        public static Status NoError => new Status(ErrorCode.NoError, null);
    }
}