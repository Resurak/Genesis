using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib.Sync
{
    public class FileData : ISyncItem
    {
        public FileData()
        {

        }

        public FileData(string path)
        {
            var info = new FileInfo(path);
            Length = info.Length;
            CreationDate = info.CreationTime;
            LastWriteDate = info.LastWriteTime;
        }

        public FileData(string root, string path) : this(path)
        {
            ID = Guid.NewGuid();
            Hash = new byte[0];

            Name = Path.GetRelativePath(root, path);
        }

        public Guid ID { get; set; }
        public SyncFlags Flags { get; set; }

        public byte[] Hash { get; set; }
        public string Name { get; set; }

        public long Length { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastWriteDate { get; set;}
    }
}
