using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Sync.Exceptions
{
    public class RejectedException : Exception
    {
        public RejectedException() : base("Request was rejected from server") { }
    }
}
