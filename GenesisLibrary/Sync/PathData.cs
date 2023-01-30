using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLibrary.Sync
{
    public class PathData : ISyncElement
    {
        public PathData() { }

        public PathData(string root, FileInfo info) 
        {
            this.ID = Guid.NewGuid();
            this.Type = PathType.File;
            this.Path = string.Join('\\', info.FullName.Split('\\').Except(root.Split("\\")));

            this.Name = info.Name;
            this.Size = info.Length;

            this.CreationTime = info.CreationTime;
            this.LastWriteTime = info.LastWriteTime;
        }

        public PathData(string root, DirectoryInfo info)
        {
            this.ID = Guid.NewGuid();
            this.Type = PathType.Folder;
            this.Path = string.Join('\\', info.FullName.Split('\\').Except(root.Split("\\")));

            this.Name = info.Name;

            this.CreationTime = info.CreationTime;
            this.LastWriteTime = info.LastWriteTime;
        }

        public Guid ID { get; set; }
        public string Path { get; set; }
        public PathType Type { get; set; }

        public long Size { get; set; }
        public string Name { get; set; }

        public DateTime CreationTime { get; set; }
        public DateTime LastWriteTime { get; set; }
    }

    public enum PathType
    {
        File,
        Folder,
    }
}
