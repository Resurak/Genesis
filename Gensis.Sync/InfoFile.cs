using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gensis.Sync
{
    public class InfoFile
    {
        public InfoFile()
        {

        }

        public InfoFile(string path, string root)
        {
            this.ID = Guid.NewGuid();
            var info = new FileInfo(path);

            this.Name = info.Name;
            this.Size = info.Length;
            this.Path = System.IO.Path.GetRelativePath(root, path);

            this.CreationTime = info.CreationTime;
            this.LastWriteTime = info.LastWriteTime;
        }

        public Guid ID { get; set; }

        public string Name { get; set; }
        public string Path { get; set; }

        public long Size { get; set; }  
        public DateTime CreationTime { get; set; }
        public DateTime LastWriteTime { get; set; }
    }
}
