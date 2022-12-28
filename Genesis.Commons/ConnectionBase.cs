using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Commons
{
    public class ConnectionBase
    {
        protected TcpClient client;
        protected DataStream stream;

        public event UpdateEventHandler? SocketError;

        public event UpdateEventHandler? ClientConnected;
        public event UpdateEventHandler? ClientDisconnected;

        public event UpdateEventHandler? ClientDataSent;
        public event UpdateEventHandler? ClientDataReceived;

        public bool Connected { get; set; }

        protected void OnDataSent(int count)
        {
            ClientDataSent?.Invoke(count);
        }

        protected void OnDataReceived(int count)
        {
            ClientDataReceived?.Invoke(count);
        }

        protected void OnConnected()
        {
            this.Connected = true;
            ClientConnected?.Invoke();
        }

        protected void OnDisconnected()
        {
            this.Connected = false;
            ClientDisconnected?.Invoke();
        }

        protected void OnError(string message) =>
            OnError(new Exception(message));

        protected void OnError(Exception ex)
        {
            SocketError?.Invoke(ex);

            if (ex is ObjectDisposedException or SocketException or IOException)
            {
                Log.Debug("Socket exception while sending or receiving object, disconnecting");
                Disconnect();
            }
        }

        public async Task<object?> ReceiveObject()
        {
            try
            {
                if (!Connected)
                {
                    throw new SocketException();
                }

                var data = await stream.ReceiveData();
                if (data.Length == 0)
                {
                    return null;
                }

                OnDataReceived(data.Length);

                return Utils.DynamicDeserialize(data);
            }
            catch (Exception ex)
            {
                OnError(ex);
                return null;
            }
        }

        public async Task<T?> ReceiveGeneric<T>() where T : class
        {
            var obj = await ReceiveObject();
            if (obj is not null && obj is T)
            {
                return obj as T;
            }
            else
            {
                return null;
            }
        } 

        public async Task SendObject(object obj)
        {
            try
            {
                if (!Connected)
                {
                    throw new SocketException();
                }

                var data = Utils.Serialize(obj);
                await stream.SendData(data);

                OnDataSent(data.Length);
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

        public void Disconnect()
        {
            if (!Connected)
            {
                return;
            }

            client?.Dispose();
            stream?.Dispose();
            OnDisconnected();
        }
    }
}
