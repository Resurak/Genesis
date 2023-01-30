using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLibrary.Exceptions
{
    public class ConnectionException : Exception
    {
        public ConnectionException() : base("ExceptionThrown while trying to send/receive data") 
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
        NotConnected,

        StreamClosed,
        MaxErrorsReached,
    }
}
