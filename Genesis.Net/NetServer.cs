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
            Log.Information("Starting NetServer");

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
            Log.Information("Accepting new TcpClient");

            if (Connected)
            {
                Log.Warning("A client is already connected, disconnect first");
                return;
            }

            try
            {
                base.Client = await Server.AcceptTcpClientAsync();
                base.Stream = base.Client.GetStream();

                Log.Information("Client connected");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception thrown while accepting TcpClient");
            }
        }
    }
}
