using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Networking
{
    public class NetPacket
    {
        public NetPacket(byte[] data) 
        {
            this.Data = data;
            this.Status = Status.Success;
        }

        public NetPacket(StatusCode code, Exception? ex = null)
        {
            this.Status = new Status(code, ex);
        }

        public NetPacket(Status status)
        {
            this.Status = status;
        }

        public byte[]? Data { get; set; }
        public Status Status { get; set; }
    }
}
