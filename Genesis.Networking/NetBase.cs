using Genesis.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Networking
{
    public class NetBase
    {
        protected TcpClient Client;
        protected NetStream Stream;

        public bool Connected => Stream != null && Client != null;

        public async Task<Packet<T>> ReceiveObject<T>() where T : class
        {
            if (!Connected)
            {
                return new Packet<T>(ErrorCode.SocketError);
            }

            var data = await Stream.ReceiveData();
            if (!data.status.Success)
            {
                return new Packet<T>(data.status);
            }

            var des = MessagePackUtils.Deserialize<T>(data.data, false);
            if (des == null)
            {
                return new Packet<T>(ErrorCode.DeserializationError);
            }

            return new Packet<T>(des);
        }

        public async Task<Status> SendObject<T>(T obj) where T : class
        {
            if (!Connected)
            {
                return new Status(ErrorCode.SocketError);
            }

            var data = MessagePackUtils.Serialize(obj, false);
            if (data == null || data.Length == 0)
            {
                return new Status(ErrorCode.SerializationError, null);
            }

            return await Stream.SendData(data);
        }

        public async Task<Status> SendFile(string path)
        {
            if (!Connected)
            {
                return new Status(ErrorCode.SocketError);
            }

            return await Stream.SendFile(path);
        }

        public async Task<Status> ReceiveFile(string path, long size, bool overwrite = true)
        {
            if (!Connected)
            {
                return new Status(ErrorCode.SocketError);
            }

            return await Stream.ReceiveFile(path, size, overwrite);
        }

        public void Disconnect()
        {
            if (!Connected)
            {
                return;
            }

            Client?.Dispose();
            Stream?.Dispose();
        }
    }
}
