using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Net
{
    public class NetClient : NetBase
    {
        public NetClient()
        {

        }

        public async Task Connect(string address)
        {
            await ConnectClient(address, 6969);
        }
    }
}
