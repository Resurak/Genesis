using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLibrary.Sync
{
    public class SyncServer : SyncBase
    {
        public SyncServer() : base("serverShares.json")
        {

        }

        TcpServer? Server;

        public void Start()
        {
            if (Server?.Active ?? false)
            {
                Log.Warning("Server already active");
                return;
            }

            Server = new TcpServer();
            Server.Start();

            Log.Information("Server started");
        }

        public void Stop()
        {
            if (ClientConnected)
            {
                Disconnect();
            }

            if (Server == null || !Server.Active)
            {
                return;
            }

            base.Dispose();
            Server?.Stop();

            Log.Information("Server stopped");
        }

        public async Task WaitClient()
        {
            if (ClientConnected)
            {
                Log.Warning("A client is already connected. Disconnect first");
                return;
            }

            if (!Server?.Active ?? false)
            {
                Log.Warning("Server not active, can't wait client");
                return;
            }

            Client = await Server.AcceptTcpClientAsync();
            Stream = new TcpStream(Client);

            Log.Information("Client connected");
            OnConnected();
        }
    }
}
