using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib.Sync
{
    public class SyncServer : SyncBase
    {
        TcpServer? Server;

        public bool Active => Server != null && Server.Active;

        public void Start()
        {
            Server = new TcpServer();
            Server.Start();
        }

        public async Task WaitClient()
        {
            if (!Active)
            {
                Log.Warning("Server not active, can't wait client");
                return;
            }

            if (ClientConnected)
            {
                Log.Warning("A client is already connected");
                return;
            }

            Client = await Server.AcceptTcpClientAsync();
            Stream = new TcpStream(Client);

            Connected?.Invoke();
            _ = ReceivePacketsLoop();
        }
    }
}
