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
        bool _connected;

        protected TcpClient client;
        protected DataStream stream;

        public event UpdateEventHandler? Error;

        public event UpdateEventHandler? Connected;
        public event UpdateEventHandler? Disconnected;

        public event UpdateEventHandler? DataSent;
        public event UpdateEventHandler? DataReceived;

        protected void OnConnected()
        {
            _connected = true;
            Connected?.Invoke();
        }

        protected void OnDisconnected()
        {
            _connected = false;
            Disconnected?.Invoke();
        }

        protected void OnError(Exception ex)
        {
            Error?.Invoke(ex);

            if (ex is ObjectDisposedException or SocketException or IOException)
            {
                Log.Debug("Socket exception while sending or receiving object, disconnecting");
                Disconnect();
            }
        }

        protected void OnDataSent(int count)
        {
            DataSent?.Invoke(count);
        }

        protected void OnDataReceived(int count)
        {
            DataReceived?.Invoke(count);
        }

        public async Task<object?> ReceiveObject()
        {
            try
            {
                if (_connected)
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

        public async Task SendObject(object obj)
        {
            try
            {
                if (_connected)
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
            client?.Dispose();
            stream?.Dispose();
            OnDisconnected();
        }
    }
}
