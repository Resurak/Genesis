using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib
{
    public class CompressionException : Exception
    {
        public CompressionException(Exception? ex = null) : base("Exception thrown while compressing/decompressing data", ex) { }
    }
}
