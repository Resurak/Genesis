using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Commons.Exceptions
{
    public class NotConnectedException : Exception
    {
        public NotConnectedException() : base("No connection between client/server") { }
    }
}
