using Genesis.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Networking
{
    public class NetBase : IDisposable
    {
        public NetBase()
        {
            this.dataStream = new MemoryStream();
            this.sizeBuffer = new byte[sizeof(int)];
            this.dataBuffer = new byte[1024 * 1024];
        }

        protected TcpClient client;
        protected MemoryStream dataStream;
        protected NetworkStream netStream;

        public bool Connected => client != null && netStream != null;

        bool disposed;

        byte[] sizeBuffer;
        byte[] dataBuffer;

        public async Task<NetPacket> ReceivePacket()
        {
            try
            {
                Array.Clear(sizeBuffer);
                Array.Clear(dataBuffer);
                dataStream.SetLength(0);

                var read = await netStream.ReadAsync(sizeBuffer, 0, sizeBuffer.Length);
                CheckDataSize(read);

                var size = BitConverter.ToInt32(sizeBuffer);
                CheckDataSize(size);

                var current = 0;
                while (current < size)
                {
                    read = await netStream.ReadAsync(dataBuffer, 0, dataBuffer.Length);
                    await dataStream.WriteAsync(dataBuffer, 0, read);

                    current += read;
                }

                var decompressed = LZ4.Decompress(dataBuffer.ToArray());
                return decompressed.ToPacket();
            }
            catch (Exception ex)
            {
                return GetStatus(ex).ToPacket();
            }
        }

        public async Task<Status> SendData(byte[] data)
        {
            try
            {
                CheckDataSize(data.Length);

                var compressed = LZ4.Compress(data);
                var size = BitConverter.GetBytes(compressed.Length);

                await netStream.WriteAsync(size);
                await netStream.WriteAsync(compressed);

                return Status.Success;
            }
            catch (Exception ex)
            {
                return GetStatus(ex);
            }
        }

        public async Task<Status> SendFile(string path)
        {
            try
            {
                Array.Clear(dataBuffer);

                var read = 0;
                using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None, 1024 * 1024);

                while (fileStream.Position < fileStream.Length)
                {
                    read = await netStream.ReadAsync(dataBuffer);
                    await fileStream.WriteAsync(dataBuffer, 0, read);
                }

                return Status.Success;
            }
            catch (Exception ex)
            {
                return GetStatus(ex);
            }
        }

        public async Task<Status> ReceiveFile(string path, long size, bool overwrite = true)
        {
            try
            {
                Array.Clear(dataBuffer);

                var read = 0;
                using var fileStream = new FileStream(path, overwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None, 1024 * 1024);

                while (fileStream.Position < size)
                {
                    read = await fileStream.ReadAsync(dataBuffer);
                    await netStream.WriteAsync(dataBuffer, 0, read);
                }

                return Status.Success;
            }
            catch (Exception ex)
            {
                return GetStatus(ex);
            }
        }

        void CheckDataSize(int read)
        {
            if (read <= 0)
            {
                throw new IndexOutOfRangeException("Data to read/write <= 0");
            }
        }

        Status GetStatus(Exception ex)
        {
            if (ex is OperationCanceledException)
            {
                return new Status(StatusCode.OperationCancelled);
            }

            if (ex is InvalidOperationException)
            {
                return new Status(StatusCode.InvalidPacket);
            }

            if (ex is ArgumentOutOfRangeException || ex is IndexOutOfRangeException || ex is InvalidDataException)
            {
                return new Status(StatusCode.DataError);
            }

            if (ex is SocketException || ex is IOException || ex is ObjectDisposedException)
            {
                Dispose();
                return new Status(StatusCode.ConnectionError);
            }

            return new Status(StatusCode.UnknownException, ex);
        }

        public void Disconnect()
        {
            client?.Dispose();
            netStream?.Dispose();
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;

            client?.Dispose();
            netStream?.Dispose();
            dataStream?.Dispose();

            Array.Clear(sizeBuffer);
            Array.Clear(dataBuffer);
        }

    }
}
