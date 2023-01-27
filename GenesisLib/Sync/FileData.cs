using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib.Sync
{
    public class FileData : IPathable
    {
        public FileData()
        {

        }

        public FileData(string path, long size, byte[] data)
        {
            this.Path = path;
            this.Size = size;
            this.Data = data;
        }

        public long Size { get; set; }

        public string Path { get; set; }
        public byte[] Data { get; set; }
    }
}
