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
    public class NetStream : NetworkStream
    {
        public NetStream(TcpClient client) : base(client.Client) 
        {
            sizeBuffer = new byte[sizeof(int)];
            dataBuffer = new byte[1024 * 1024];
            dataStream = new MemoryStream();
        }

        bool disposed;

        byte[] sizeBuffer;
        byte[] dataBuffer;
        MemoryStream dataStream;

        public async Task<byte[]> ReceiveAsync(CancellationToken? token = null)
        {
            if (disposed)
            {
                return new byte[0];
            }

            try
            {
                Array.Clear(sizeBuffer);
                Array.Clear(dataBuffer);
                dataStream.SetLength(0);

                var read = await ReadAsync(sizeBuffer);
                if (read < 4)
                {
                    throw new ArgumentOutOfRangeException("Data read less than size of int, probably a connection error");
                }

                var size = BitConverter.ToInt32(sizeBuffer);
                if (size == int.MinValue)
                {
                    Log.Debug("Received disconnection code, closing stream");
                    return new byte[0];
                }

                if (size <= 0)
                {
                    Log.Debug("Data to read less or equal 0, probably a connection error");
                    return new byte[0];
                }

                if (size > 1024 * 1024 * 4) // 4 mb
                {
                    Log.Warning("Data to receive ({size}) is too much. Contact developer", size);
                    return new byte[0];
                }

                var current = 0;
                while (current < size)
                {
                    read = await ReadAsync(dataBuffer);
                    await dataStream.WriteAsync(dataBuffer, 0, read);

                    current += read;
                }

                Log.Information("received {@data}", dataStream.ToArray().Length);

                var decompressed = LZ4.Decompress(dataStream.ToArray());
                return decompressed;
            }
            catch (OperationCanceledException)
            {
                Log.Debug("Read cancelled");
                return new byte[0];
            }
            catch
            {
                throw;
            }
        }

        public async Task SendAsync(byte[] data, CancellationToken? token = null)
        {
            if (disposed)
            {
                return;
            }

            try
            {
                var compressed = LZ4.Compress(data);
                var size = BitConverter.GetBytes(compressed.Length);

                Log.Information("sending {@data}", compressed.Length);

                await WriteAsync(size);
                await WriteAsync(compressed);
            }
            catch (OperationCanceledException)
            {
                Log.Debug("Write cancelled");
            }
            catch
            {
                throw;
            }
        }

        public async Task SendFile(string path)
        {

        }

        public async Task ReceiveFile(string path, long size)
        {

        }

        public new void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            base.Dispose();
        }
    }
}
