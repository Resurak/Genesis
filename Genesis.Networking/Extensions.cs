using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Networking
{
    public static class Extensions
    {
        public static NetPacket ToPacket(this byte[] data) =>
            new NetPacket(data);

        public static NetPacket ToPacket(this Status status) =>
            new NetPacket(status);
    }
}
