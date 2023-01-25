using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib.Sync
{
    public class FileData : IGuidItem
    {
        public Guid ID { get; set; }
        public long Size { get; set; }

        public string Path { get; set; }
        public byte[] Data { get; set; }
    }
}
