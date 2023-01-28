using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib
{
    public class PathData : IPathable
    {
        public PathData(FileInfo info, string root)
        {
            Name = info.Name;
            Path = string.Join('\\', info.FullName.Split('\\').Except(root.Split('\\')));

            PathType = PathType.File;

            Size = info.Length;
            Hash = new byte[0];

            CreationTime = info.CreationTime;
            LastWriteTime = info.LastWriteTime;
        }

        public PathData(DirectoryInfo info, string root)
        {
            Name = info.Name;
            Path = string.Join('\\', info.FullName.Split('\\').Except(root.Split('\\')));

            PathType = PathType.Directory;

            Size = 0;
            Hash = new byte[0];

            CreationTime = info.CreationTime;
            LastWriteTime = info.LastWriteTime;
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
