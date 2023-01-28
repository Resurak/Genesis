using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib.Exceptions
{
    public class SerializationException : Exception
    {
        public SerializationException() : base("ExceptionThrown while serializing\\deserializing data") 
        {

        }

        public SerializationException(byte[]? data) : base("ExceptionThrown while deserializing data") 
        {
            Data = data;
        }

        public SerializationException(object? obj = null) : base("ExceptionThrown while serializing object")
        {
            Object = obj;
        }

        public new byte[]? Data { get; private set; }
        public object? Object { get; private set; }
    }
}
