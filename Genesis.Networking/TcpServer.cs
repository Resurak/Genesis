using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Networking
{
    public class TcpServer : TcpListener
    {
        public TcpServer(int port) : base(IPAddress.Any, port)
        {

        }

        public new bool Active => base.Active;
    }
}
