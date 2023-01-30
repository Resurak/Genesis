using GenesisLibrary.Exceptions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLibrary
{
    public class TcpStream : NetworkStream
    {
        public TcpStream(TcpClient client) : base(client.Client) { }

        public async Task<byte[]> ReceiveData(CancellationToken token = default)
        {
            try
            {
                var sizeBuffer = new byte[sizeof(int)];
                await ReadExactlyAsync(sizeBuffer, token);

                var buffer = new byte[BitConverter.ToInt32(sizeBuffer)];
                await ReadExactlyAsync(buffer, token);

                return buffer;
            }
            catch (Exception ex)
            {
                Log.Debug(ex, "Exception thrown while receiving data");
                if (ex is OperationCanceledException)
                {
                    return new byte[0];
                }

                throw new ConnectionException();
            }
        }

        public async Task SendData(byte[] data, CancellationToken token = default)
        {
            try
            {
                var sizeBuffer = BitConverter.GetBytes(data.Length);

                await WriteAsync(sizeBuffer, token);
                await WriteAsync(data, token);
            }
            catch (Exception ex)
            {
                Log.Debug(ex, "Exception thrown while sending data");
                if (ex is OperationCanceledException)
                {
                    return;
                }

                throw new ConnectionException();
            }
        }

        public async Task<object?> ReceiveObject(CancellationToken token = default)
        {
            var data = await ReceiveData(token);
            if (data.Length == 0)
            {
                return null;
            }
            else
            {
                return data.FromPack();
            }
        }

        public async Task SendObject(object obj, CancellationToken token = default)
        {
            var data = obj.ToPack();
            await SendData(data, token);
        }
    }
}
