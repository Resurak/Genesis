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
    public class NetServer : NetBase
    {
        public NetServer() 
        {

        }

        public bool Started => Server?.Active ?? false;

        public async Task StartAndWaitClient()
        {
            StartServer();
            await WaitClient();
        }

        public void Stop()
        {
            base.Server?.Stop();
        }
    }
}
