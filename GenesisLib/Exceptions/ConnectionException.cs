using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib.Exceptions
{
    public class ConnectionException : Exception
    {
        public ConnectionException() : base("Error while trying to send/receive data") 
        {

        }

        public ConnectionException(ConnectionExceptionCode code) : this()
        {
            this.Code = code;
        }

        public ConnectionExceptionCode Code { get; private set; }
    }

    public enum ConnectionExceptionCode
    {
        Unknown,

        StreamClosed,
        MaxErrorsReached,
    }
}
