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
    public class NetClient : TcpClient
    {
        public NetClient()
        {

        }

        bool disposed;
        NetStream? Stream;

        public new bool Connected => base.Connected && Stream != null; 

        public async Task Connect(string address)
        {
            try
            {
                var ip = IPAddress.Parse(address);
                var endPoint = new IPEndPoint(ip, 6969);

                await ConnectAsync(endPoint);
                Stream = new NetStream(this);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception thrown while connecting to server");
            }
        }

        public async Task<T?> ReceiveObject<T>() where T : class
        {
            if (!Connected)
            {
                Log.Warning("Client not connected, cannot receive object");
                return default(T);
            }

            try
            {
                return await Stream?.ReceiveObject<T>();
            }
            catch (Exception ex)
            {
                if (ex is SocketException || ex is IOException || ex is ObjectDisposedException)
                {
                    Log.Warning(ex, "Client forced disconnection, disposing");
                    Stream?.Dispose();
                }
                else
                {
                    Log.Warning(ex, "Exception thrown while receiving object");
                }

                return default(T);
            }
        }

        public async Task SendObject<T>(T obj) where T : class
        {
            if (!Connected)
            {
                Log.Warning("Client not connected, cannot send object");
                return;
            }

            try
            {
                await Stream?.SendObject(obj);
            }
            catch (Exception ex)
            {
                if (ex is SocketException || ex is IOException || ex is ObjectDisposedException)
                {
                    Log.Warning(ex, "Client forced disconnection, disposing");
                    Disconnect();
                }
                else
                {
                    Log.Warning(ex, "Exception thrown while sending object");
                }
            }
        }

        public void Disconnect()
        {
            Stream?.Dispose();
            Dispose();
        }
    }
}
