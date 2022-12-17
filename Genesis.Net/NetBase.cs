using Genesis.Commons;
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
    public class NetBase : IDisposable
    {
        protected TcpServer? Server;
        protected TcpClient? Client;
        protected NetStream? Stream;

        public bool Connected => Stream != null && Client != null && Client.Connected;

        protected async Task ConnectClient(string address, int port)
        {
            if (Connected)
            {
                Log.Warning("Can't connected: already connected, disconnect first");
                return;
            }

            try
            {
                var ip = IPAddress.Parse(address);
                var endPoint = new IPEndPoint(ip, port);

                Client = new TcpClient();
                await Client.ConnectAsync(endPoint);

                Stream = new NetStream(Client);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Exception thrown while connecting to server");
            }
        }

        protected void StartServer()
        {
            if (Server is not null && Server.Active)
            {
                Log.Warning("Can't start server: already started");
                return;
            }

            Server = new TcpServer(6969);
            Server.Start();
        }

        protected async Task WaitClient()
        {
            if (Server is null || !Server.Active)
            {
                Log.Warning("Can't wait client: server not started");
                return;
            }

            if (Connected)
            {
                Log.Warning("Can't wait client: already connected, disconnect first");
                return;
            }

            try
            {
                Client = await Server.AcceptTcpClientAsync();
                Stream = new NetStream(Client);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Exception thrown while waiting client");
            }
        }

        public async Task<T?> ReceiveObject<T>() where T : class
        {
            if (!Connected)
            {
                Log.Warning("Can't receive object: not connected");
                return null;
            }

            try
            {
                var data = await Stream.ReceiveAsync();
                return MsgPack.Deserialize<T>(data);
            }
            catch (Exception ex)
            {
                if (ex is SocketException || ex is IOException || ex is ObjectDisposedException)
                {
                    Log.Warning(ex, "Client forced disconnection, closing");
                    Disconnect();
                }
                else
                {
                    Log.Warning(ex, "Exception thrown while receiving object");
                }

                return null;
            }
        }

        public async Task SendObject<T>(T obj) where T : class
        {
            if (!Connected)
            {
                Log.Warning("Can't send object: not connected");
                return;
            }

            try
            {
                var data = MsgPack.Serialize(obj);
                await Stream.SendAsync(data);
            }
            catch (Exception ex)
            {
                if (ex is SocketException || ex is IOException || ex is ObjectDisposedException)
                {
                    Log.Warning(ex, "Client forced disconnection, closing");
                    Disconnect();
                }
                else
                {
                    Log.Warning(ex, "Exception thrown while receiving object");
                }
            }
        }

        public async Task SendFile(string path)
        {
            if (!Connected)
            {
                Log.Warning("Can't send file: not connected");
                return;
            }

            await Stream.SendFile(path);
        }

        public async Task ReceiveFile(string path, long size)
        {
            if (!Connected)
            {
                Log.Warning("Can't receive file: not connected");
                return;
            }

            await Stream.ReceiveFile(path, size);
        }

        public void Disconnect()
        {
            Stream?.Dispose();
            Client?.Dispose();

            Stream = null;
            Client = null;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
