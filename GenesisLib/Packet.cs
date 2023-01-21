using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib
{
    public class Packet
    {
        public byte[]? Data { get; set; }
        public string[]? Headers { get; set; }

        public bool IsEmpty => Data == null && Headers == null;
        public static Packet Empty => new Packet();
    }
}
