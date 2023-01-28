using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib.Sync
{
    public class SyncClient : SyncBase
    {
        public async Task Connect(string ip, int port = 6969)
        {
            var address = IPAddress.Parse(ip);
            var endPoint = new IPEndPoint(address, port);

            Client = new TcpClient();
            await Client.ConnectAsync(endPoint);

            Stream = new TcpStream(Client);
            Connected?.Invoke();

            _ = ReceivePacketsLoop();
        }
    }
}
