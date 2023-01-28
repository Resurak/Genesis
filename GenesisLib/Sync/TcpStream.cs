using GenesisLib.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib.Sync
{
    public class TcpStream : IDisposable
    {
        public TcpStream(TcpClient client)
        {
            stream = client.GetStream();
        }

        NetworkStream stream;

        public async Task<byte[]> ReceiveData(CancellationToken token = default)
        {
            try
            {
                var sizeBuffer = new byte[sizeof(int)];
                await stream.ReadExactlyAsync(sizeBuffer, token);

                var buffer = new byte[BitConverter.ToInt32(sizeBuffer, 0)];
                await stream.ReadExactlyAsync(buffer, token);
                return buffer ;
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException)
                {
                    throw;
                }

                throw new ConnectionException();
            }
        }

        public async Task SendData(byte[] data, CancellationToken token = default)
        {
            try
            {
                var sizeBuffer = BitConverter.GetBytes(data.Length);

                await stream.WriteAsync(sizeBuffer, token);
                await stream.WriteAsync(data, token);
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException)
                {
                    throw;
                }

                throw new ConnectionException();
            }
        }

        public async Task<object> ReceiveObject(CancellationToken token = default)
        {
            var data = await ReceiveData(token);
            if (data.Length == 0)
            {
                return new object();
            }

            return data.Msg_Deserialize();
        }

        public async Task SendObject(object obj, CancellationToken token = default)
        {
            var data = obj.Msg_Serialize();
            await SendData(data, token);
        }

        public void Dispose()
        {
            stream?.Dispose();
        }
    }
}
