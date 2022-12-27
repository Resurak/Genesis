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

        public async Task<byte[]> ReceiveData()
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

                return LZ4.Decompress(dataStream.ToArray());
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

        public async Task SendData(byte[] data)
        {
            if (!Connected)
            {
                Log.Warning("Tried to send data but client not connected");
                return; 
            }

            try
            {
                var compressed = LZ4.Compress(data);

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

        public async Task ReceiveFile(string path, long size, bool overwrite = true)
        {

        }

        public async Task SendFile()
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

                Log.Information("Client disconnected");
            }
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
