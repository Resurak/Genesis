using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLibrary.Exceptions
{
    public class UnknownObjectException : Exception
    {
        public UnknownObjectException(object? obj = null) : base("Unknown object deserialized") 
        {
            Object = obj;
        }

        public object? Object { get; private set; }
    }
}
