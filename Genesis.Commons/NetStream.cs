using Genesis.Commons.Exceptions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Commons
{
    public class NetStream : NetworkStream
    {
        public NetStream(TcpClient client) : base(client.Client) 
        {
            CanStream = true;
        }

        public bool CanStream { get; private set; }

        public async Task<byte[]> ReceiveData(CancellationToken token = default)
        {
            if (!CanStream)
            {
                return new byte[0];
            }

            try
            {
                var sizeBuffer = new byte[sizeof(int)];
                await ReadExactlyAsync(sizeBuffer, token);

                if (sizeBuffer.ToInt() <= 0)
                {
                    throw new IndexOutOfRangeException();
                }

                var dataBuffer = new byte[sizeBuffer.ToInt()];
                await ReadExactlyAsync(dataBuffer, token);

                return dataBuffer;
            }
            catch (Exception ex)
            {
                if (ex is not IndexOutOfRangeException or OperationCanceledException)
                {
                    CanStream = false;
                }

                return new byte[0];
            }
        }

        public async Task SendData(byte[] dataBuffer, CancellationToken token = default)
        {
            if (!CanStream)
            {
                return;
            }

            try
            {
                if (dataBuffer.Length == 0)
                {
                    return;
                }

                var sizeBuffer = dataBuffer.Length.ToBytes();

                await WriteAsync(sizeBuffer, token);
                await WriteAsync(dataBuffer, token);
            }
            catch (Exception ex)
            {
                if (ex is not IndexOutOfRangeException or OperationCanceledException)
                {
                    CanStream = false;
                }
            }
        }

        public async Task SendFile(string filePath, IProgress<long>? progress = null, CancellationToken token = default)
        {
            if (!CanStream)
            {
                throw new NotConnectedException();
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }

            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None, 1024 * 16);

            var buffer = new byte[1024 * 16];
            while (fileStream.Position < fileStream.Length)
            {
                var read = await fileStream.ReadAsync(buffer, 0, buffer.Length);
                await WriteAsync(buffer, 0, read);

                progress?.Report(fileStream.Position);
            }
        }

        public async Task ReceiveFile(string filePath, long size, IProgress<long>? progress = null, CancellationToken token = default)
        {
            if (!CanStream)
            {
                throw new NotConnectedException();
            }

            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 16);

            var buffer = new byte[1024 * 16];
            while (fileStream.Position < size)
            {
                var read = await ReadAsync(buffer, 0, buffer.Length);
                await fileStream.WriteAsync(buffer, 0, read);

                progress?.Report(fileStream.Position);
            }
        }

        public async Task<object?> ReceiveObject(CancellationToken token = default)
        {
            if (!CanStream)
            {
                return null;
            }

            var data = await ReceiveData(token);
            if (data == null)
            {
                return null;
            }

            return Utils.Deserialize(data);
        }

        public async Task SendObject(object obj, CancellationToken token = default)
        {
            if (!CanStream)
            {
                return;
            }

            var data = Utils.Serialize(obj);
            await SendData(data, token);
        }
    }
}
