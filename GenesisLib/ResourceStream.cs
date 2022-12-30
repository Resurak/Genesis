using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib
{
    public class ResourceStream : NetworkStream
    {
        public ResourceStream(TcpClient client) : base(client.Client) 
        {
            dataArray = new byte[1024 * 256];
            sizeArray = new byte[sizeof(int)];
        }

        int errors;
        byte[] dataArray;
        byte[] sizeArray;

        public async Task<Resource> GetResource(CancellationToken token = default)
        {
            try
            {
                await ReadExactlyAsync(sizeArray);
                var size = sizeArray.ToInt();

                if (size <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(size));
                }

                Array.Resize(ref dataArray, size);
                await ReadExactlyAsync(dataArray);


            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException)
                {
                    return new Resource();
                }
                else if (ex is SerializationException or CompressionException)
                {
                    throw;
                }
                else
                {
                    throw new SocketException();
                }
            }
        }

        void ResetBuffers()
        {
            Array.Clear(sizeArray);
            Array.Clear(dataArray);
        }
    }
}
