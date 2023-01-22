using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Sync.Exceptions
{
    public class UnknownObjectException : Exception
    {
        public UnknownObjectException() : base("Unknown / Null object received") { }
    }
}
