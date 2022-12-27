using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Net
{
    public class NotConnectedException : Exception
    {
        public NotConnectedException() : base("Client not connected") { }
    }
}
