using Genesis.Commons;
using Serilog;
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

        protected async Task<byte[]> ReceiveData()
        {
            if (!Connected)
            {
                Log.Warning("Tried to receive data but client not connected");
                return new byte[0];
            }

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

                return Utils.Decompress(dataStream.ToArray());
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException)
                {
                    Log.Debug("Data read was cancelled or was invalid size");
                }
                else
                {
                    Log.Debug(ex, "Exception while reading data, disconnecting");
                    Disconnect();
                }

                return new byte[0];
            }
        }

        protected async Task SendData(byte[] data)
        {
            if (!Connected)
            {
                Log.Warning("Tried to send data but client not connected");
                return; 
            }

            try
            {
                var compressed = Utils.Compress(data);

                await Stream.WriteAsync(compressed.Length.ToBytes());
                await Stream.WriteAsync(compressed);
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException)
                {
                    Log.Debug("Data write was cancelled");
                }
                else
                {
                    Log.Debug(ex, "Exception while writing data, disconnecting");
                    Disconnect();
                }
            }
        }

        protected async Task ReceiveFile(string path, long size, bool overwrite = true)
        {

        }

        protected async Task SendFile()
        {

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

            if (Connected)
            {
                Stream?.Dispose();
                Client?.Dispose();

                Stream = null;
                Client = null;
            }
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
