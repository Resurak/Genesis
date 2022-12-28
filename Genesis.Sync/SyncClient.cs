using Genesis.Commons;
using Genesis.Net;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Sync
{
    public sealed class SyncClient
    {
        TcpClient client;
        DataStream stream;

        public event UpdateEventHandler? Connected;
        public event UpdateEventHandler? Disconnected;

        bool _connected;

        public async Task ConnectLan(string address = "127.0.0.1", int port = 6969)
        {
            try
            {
                var ip = IPAddress.Parse(address);
                var endPoint = new IPEndPoint(ip, port);

                client = new TcpClient();
                await client.ConnectAsync(endPoint);

                stream = new DataStream(client);
                Connected?.Invoke();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while connecting to server");
            }
        }

        public void OnConnected()
        {
            _connected = true;
            Connected?.Invoke();
        }
    }
}
