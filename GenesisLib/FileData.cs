using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib
{
    public class FileData
    {
        public int TotalBlocks { get; set; }
        public int CurrentBlock { get; set; }

        public byte[] DataBlock { get; set; }
        public FileMetadata Metadata { get; set; }
    }
}
