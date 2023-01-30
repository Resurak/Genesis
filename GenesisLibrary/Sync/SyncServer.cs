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
        public SyncServer() 
        {
            base.ShareFilePath = "serverShares.json";
        }

        TcpServer? Server;

        public bool Active => Server != null && Server.Active;

        public void Start()
        {
            if (Active)
            {
                Log.Warning("Server already active");
                return;
            }

            Server = new TcpServer();
            Server.Start();
        }

        public async Task WaitClient()
        {
            if (ClientConnected)
            {
                Log.Warning("A client is already connected. Disconnect first");
                return;
            }

            if (!Active)
            {
                Log.Warning("Server not active, can't wait client");
                return;
            }

            Client = await Server.AcceptTcpClientAsync();
            Stream = new TcpStream(Client);

            Log.Information("Client connected");
        }
    }
}
