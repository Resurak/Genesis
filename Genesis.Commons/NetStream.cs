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

        }

        public event ProgressEventHandler? FileProgress;

        public async Task<byte[]> ReceiveData(CancellationToken token = default)
        {
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
                if (ex is IndexOutOfRangeException or OperationCanceledException)
                {
                    return new byte[0];
                }
                else
                {
                    throw new NotConnectedException();
                }
            }
        }

        public async Task SendData(byte[] dataBuffer, CancellationToken token = default)
        {
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
                    throw new NotConnectedException();
                }
            }
        }

        public async Task<object?> ReceiveObject(CancellationToken token = default)
        {
            var data = await ReceiveData(token);
            return Utils.Deserialize(data);
        }

        public async Task SendObject(object obj, CancellationToken token = default)
        {
            var data = Utils.Serialize(obj);
            await SendData(data, token);
        }

        public async Task SendFile(string filePath, Guid fileID = default, CancellationToken token = default)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }

            try
            {
                using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None, 1024 * 16);

                var buffer = new byte[1024 * 16];
                var progress = new NetProgress(fileStream.Length, fileID);

                FileProgress?.Invoke(progress);

                while (fileStream.Position < fileStream.Length)
                {
                    var read = await fileStream.ReadAsync(buffer, 0, buffer.Length);
                    await WriteAsync(buffer, 0, read);

                    progress.Update(read);
                    FileProgress?.Invoke(progress);
                }

                progress.Finish();
                FileProgress?.Invoke(progress);
            }
            catch
            {
                throw new NotConnectedException();
            }
        }

        public async Task ReceiveFile(string filePath, long size, Guid fileID, CancellationToken token = default)
        {
            try
            {
                using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 16);

                var buffer = new byte[1024 * 16];
                var progress = new NetProgress(size, fileID);

                FileProgress?.Invoke(progress);

                while (fileStream.Position < size)
                {
                    var read = await ReadAsync(buffer, 0, buffer.Length);
                    await fileStream.WriteAsync(buffer, 0, read);

                    progress.Update(read);
                    FileProgress?.Invoke(progress);
                }

                progress.Finish();
                FileProgress?.Invoke(progress);
            }
            catch
            {
                throw new NotConnectedException();
            }
        }
    }
}
