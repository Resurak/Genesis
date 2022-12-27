using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Net
{
    public class ConnectionException : Exception
    {
        public ConnectionException() : base("An error occurred while the client was connected") { }
    }
}
