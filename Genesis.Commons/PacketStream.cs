using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Commons
{
    public class PacketStream : NetworkStream 
    {
        public PacketStream(TcpClient client) : base(client.Client) 
        {
            sizeBuffer = new byte[sizeof(int)];
        }

        byte[] sizeBuffer;

        public async Task<Packet?> ReceivePacketAsync()
        {
            try
            {
                await ReadExactlyAsync(sizeBuffer);
                var size = sizeBuffer.ToInt();

                if (size <= 0)
                {
                    throw new IndexOutOfRangeException();
                }

                var temp = new byte[size];
                await ReadExactlyAsync(temp);

                return Utils.TypeDeserialize<Packet>(temp);
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException or IndexOutOfRangeException)
                {
                    return Packet.Empty;
                }
                else
                {
                    throw new EndOfStreamException();
                }
            }
        }

        public async Task SendPacketAsync(Packet packet)
        {
            try
            {
                var data = Utils.Serialize(packet);
                var size = data.Length.ToBytes();

                await WriteAsync(size);
                await WriteAsync(data);
            }
            catch (Exception ex)
            {
                if (ex is not OperationCanceledException or IndexOutOfRangeException)
                {
                    throw new EndOfStreamException();
                }
            }
        }
    }
}
