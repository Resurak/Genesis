using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib
{
    public class SerializationException : Exception
    {
        public SerializationException(string variableName = "", Exception? ex = null) : base("Exception thrown while serializing or deserializing" + (!string.IsNullOrEmpty(variableName) ? $" {variableName}" : ""), ex) { }
    }
}
