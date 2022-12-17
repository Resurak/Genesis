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
    public class NetServer : TcpListener
    {
        public NetServer() : base(IPAddress.Any, 6969)
        {

        }

        TcpClient? Client;
        NetStream? Stream;

        public new bool Active => base.Active;
        public bool Connected => Client != null && Stream != null;

        public async Task WaitClient()
        {
            if (!Active)
            {
                Log.Warning("Server not started, can't wait client");
                return;
            }

            Client = await AcceptTcpClientAsync();
            Stream = new NetStream(Client);
        }

        public async Task<T?> ReceiveObject<T>() where T : class
        {
            if (!Active)
            {
                Log.Warning("Server not started, cannot receive object");
                return default(T);
            }

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
            if (!Active)
            {
                Log.Warning("Server not started, cannot send object");
                return;
            }

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
                    Log.Warning("Client forced disconnection, disposing");
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
            Client?.Dispose();

            Stream = null;
            Client = null;
        }
    }
}
