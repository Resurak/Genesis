using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Commons
{
    public class DataStream
    {
        public DataStream(TcpClient client)
        {
            this.networkStream = client.GetStream();
            this.dataStream = new MemoryStream();

            this.dataBuffer = new byte[1024 * 4];
            this.sizeBuffer = new byte[sizeof(int)];
        }

        int size;
        byte[] dataBuffer;
        byte[] sizeBuffer;

        MemoryStream dataStream;
        NetworkStream networkStream;

        public async Task<byte[]> ReceiveData(CancellationToken token = default)
        {
            try
            {
                ResetBuffers();

                await networkStream.ReadExactlyAsync(sizeBuffer, token);
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

                    await networkStream.ReadExactlyAsync(dataBuffer, 0, diff, token);
                    await dataStream.WriteAsync(dataBuffer, 0, diff, token);

                    current += diff;
                }

                return Utils.Decompress(dataStream.ToArray());
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException)
                {
                    Log.Debug("Data read was cancelled");
                    return new byte[0];
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task SendData(byte[] data, CancellationToken token = default)
        {
            try
            {
                var compressed = Utils.Compress(data);

                await networkStream.WriteAsync(compressed.Length.ToBytes(), token);
                await networkStream.WriteAsync(compressed, token);
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException)
                {
                    Log.Debug("Data write was cancelled");
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task ReceiveFile(string path, long size, bool overwrite = true)
        {
            var read = 0;
            using var fileStream = new FileStream(path, overwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.Write, FileShare.None, 1024 * 4);

            while (fileStream.Position < size)
            {
                read = await networkStream.ReadAsync(dataBuffer, 0, dataBuffer.Length);
                await fileStream.WriteAsync(dataBuffer, 0, read);
            }
        }

        public async Task SendFile(string path)
        {
            var read = 0;
            using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None, 1024 * 4);

            while (fileStream.Position < fileStream.Length)
            {
                read = await fileStream.ReadAsync(dataBuffer, 0, dataBuffer.Length);
                await networkStream.WriteAsync(dataBuffer, 0, read);
            }
        }

        void ResetBuffers()
        {
            Array.Clear(dataBuffer, 0, dataBuffer.Length);
            Array.Clear(sizeBuffer, 0, sizeBuffer.Length);
            dataStream.SetLength(0);
        }

        public void Dispose()
        {
            ResetBuffers();
            networkStream?.Dispose();
        }

    }
}
