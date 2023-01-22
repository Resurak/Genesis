using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Commons.Exceptions
{
    public class SerializationException : Exception
    {
        public SerializationException(Exception? ex = null) : base("Exception thrown while serializing/deserializing data", ex) { }
    }
}
