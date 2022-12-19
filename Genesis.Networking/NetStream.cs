using Genesis.Commons;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.Sockets;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Networking
{
    public class NetStream : IDisposable
    {
        public NetStream(TcpClient client)
        {
            this.dataStream = new MemoryStream();
            this.networkStream = client.GetStream();

            this.sizeBuffer = new byte[sizeof(int)];
            this.dataBuffer = new byte[1024 * 1024];
        }

        bool disposed;

        byte[] sizeBuffer;
        byte[] dataBuffer;

        MemoryStream dataStream;
        NetworkStream networkStream;

        public async Task<(byte[] data, Status status)> ReceiveData()
        {
            try
            {
                Array.Clear(sizeBuffer);
                Array.Clear(dataBuffer);
                dataStream.SetLength(0);

                var read = await networkStream.ReadAsync(sizeBuffer, 0, sizeBuffer.Length);
                CheckDataSize(read);

                var size = BitConverter.ToInt32(sizeBuffer);
                CheckDataSize(size);

                var current = 0;
                while (current < size)
                {
                    read = await networkStream.ReadAsync(dataBuffer, 0, dataBuffer.Length);
                    await dataStream.WriteAsync(dataBuffer, 0, read);

                    current += read;
                }

                return (data: LZ4.Decompress(dataBuffer.ToArray()), status: Status.NoError);
            }
            catch (Exception ex)
            {
                return (data: new byte[0], status: GetStatus(ex));
            }
        }

        public async Task<Status> SendData(byte[] data)
        {
            try
            {
                CheckDataSize(data.Length);

                var compressed = LZ4.Compress(data);
                var size = BitConverter.GetBytes(compressed.Length);

                await networkStream.WriteAsync(size);
                await networkStream.WriteAsync(compressed);

                return Status.NoError;
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
                    read = await networkStream.ReadAsync(dataBuffer);
                    await fileStream.WriteAsync(dataBuffer, 0, read);
                }

                return Status.NoError;
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
                    await networkStream.WriteAsync(dataBuffer, 0, read);
                }

                return Status.NoError;
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
                return new Status(ErrorCode.OperationCancelled);
            }

            if (ex is InvalidOperationException)
            {
                return new Status(ErrorCode.InvalidPacket);
            }

            if (ex is ArgumentOutOfRangeException || ex is IndexOutOfRangeException || ex is InvalidDataException)
            {
                return new Status(ErrorCode.DataError);
            }

            if (ex is SocketException || ex is IOException || ex is ObjectDisposedException)
            {
                Dispose();
                return new Status(ErrorCode.SocketError);
            }

            return new Status(ErrorCode.UnknownException, ex);
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            dataStream.Dispose();
            networkStream.Dispose();

            Array.Clear(sizeBuffer);
            Array.Clear(dataBuffer);
        }
    }
}
