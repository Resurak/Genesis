using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib
{
    public class TcpServer : TcpListener
    {
        public TcpServer(int port = 6969) : base(IPAddress.Any, port) { }
        public TcpServer(string ip, int port) : base(IPAddress.Parse(ip), port) { }
    }
}
