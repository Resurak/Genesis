using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Net
{
    public class NetServer : NetBase
    {
        protected TcpServer Server;

        public bool Active => Server?.Active ?? false;

        public void Start()
        {
            if (Active)
            {
                Log.Warning("Server already active, returning");
                return;
            }

            Server = new TcpServer();
            Server.Start();
        }

        public async Task WaitClient()
        {
            try
            {
                base.Client = await Server.AcceptTcpClientAsync();
                base.Stream = base.Client.GetStream();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception thrown while accepting TcpClient");
            }
        }
    }
}
