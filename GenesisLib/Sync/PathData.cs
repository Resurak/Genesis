using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib.Sync
{
    public class PathData : IPathable
    {
        public PathData(FileInfo info, string root)
        {
            this.Name = info.Name;
            this.Path = string.Join('\\', info.FullName.Split('\\').Except(root.Split('\\')));

            this.PathType = PathType.File;

            this.Size = info.Length;
            this.Hash = new byte[0];

            this.CreationTime = info.CreationTime;
            this.LastWriteTime = info.LastWriteTime;
        }

        public PathData(DirectoryInfo info, string root)
        {
            this.Name = info.Name;
            this.Path = string.Join('\\', info.FullName.Split('\\').Except(root.Split('\\')));

            this.PathType = PathType.Directory;

            this.Size = 0;
            this.Hash = new byte[0];

            this.CreationTime = info.CreationTime;
            this.LastWriteTime = info.LastWriteTime;
        }

        public string Name { get; set; }
        public string Path { get; set; }
        public PathType PathType { get; set; }

        public long Size { get; set; }
        public byte[] Hash { get; set; }

        public DateTime CreationTime { get; set; }
        public DateTime LastWriteTime { get; set; }

        public bool SyncFlag { get; set; }
        public bool DeleteFlag { get; set; }
    }

    public enum PathType
    {
        File,
        Directory
    }
}
