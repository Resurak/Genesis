using Genesis.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Net
{
    public class NetBase : IDisposable
    {
        public NetBase()
        {
            this.dataStream = new MemoryStream();

            this.dataBuffer = new byte[1024 * 4];
            this.sizeBuffer = new byte[sizeof(int)];
        }

        protected TcpClient? Client;
        protected NetworkStream? Stream;

        int size;
        byte[] dataBuffer;
        byte[] sizeBuffer;
        MemoryStream dataStream;

        public bool Connected => Stream != null && Client != null;

        public async Task<byte[]> ReceiveData()
        {
            try
            {
                ResetBuffers();

                await Stream.ReadExactlyAsync(sizeBuffer);
                size = sizeBuffer.ToInt();

                if (size <= 0)
                {
                    throw new InvalidDataException();
                }

                var diff = 0;
                var current = 0;
                while (current < size)
                {
                    diff = current + dataBuffer.Length > size ? size - current : dataBuffer.Length;

                    await Stream.ReadExactlyAsync(dataBuffer, 0, diff);
                    await dataStream.WriteAsync(dataBuffer, 0, diff);

                    current += diff;
                }

                return dataStream.ToArray();
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException or InvalidDataException or IndexOutOfRangeException)
                {
                    return new byte[0];
                }
                else if (ex is SocketException or IOException or EndOfStreamException or ObjectDisposedException or ArgumentNullException)
                {
                    throw new NotConnectedException();
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task SendData(byte[] data)
        {
            try
            {
                await Stream.WriteAsync(data.Length.ToBytes());
                await Stream.WriteAsync(data);
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException)
                {
                    return;
                }
                else if (ex is SocketException or IOException or EndOfStreamException or ObjectDisposedException or ArgumentNullException)
                {
                    throw new NotConnectedException();
                }
                else
                {
                    throw;
                }
            }
        }

        void ResetBuffers()
        {
            Array.Clear(dataBuffer, 0, dataBuffer.Length);
            Array.Clear(sizeBuffer, 0, sizeBuffer.Length);
            dataStream.SetLength(0);
        }

        public void Disconnect()
        {
            ResetBuffers();
            Stream?.Dispose();
            Client?.Dispose();
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
