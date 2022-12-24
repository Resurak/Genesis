using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Sync
{
    public class FileData
    {
        public FileData()
        {

        }

        public FileData(string path, string root)
        {
            var info = new FileInfo(path);

            this.Size = info.Length;
            this.Name = info.Name;  

            this.CreationTime = info.CreationTime;
            this.LastModifiedTime = info.LastWriteTime;

            this.Path = System.IO.Path.GetRelativePath(root, path);
        }

        public Guid ID { get; set; }
        public long Size { get; set; }

        public string Path { get; set; }
        public string Name { get; set; }

        public DateTime CreationTime { get; set; }
        public DateTime LastModifiedTime { get; set;}
    }
}
