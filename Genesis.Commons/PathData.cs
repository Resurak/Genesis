using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Commons
{
    public class PathData
    {
        public PathData()
        {
            Name = string.Empty;
            Path = string.Empty;

            Hash = new byte[0];
        }

        public Guid ID { get; set; }

        public string Name { get; set; }
        public string Path { get; set; }

        public PathType Type { get; set; }

        public long Size { get; set; }
        public byte[] Hash { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        public bool Flag_Sync { get; set; }
        public bool Flag_Delete { get; set; }

        public static PathData FileData(string path, string root)
        {
            var info = new FileInfo(path);
            var data = new PathData();

            data.ID = Guid.NewGuid();
            data.Type = PathType.File;

            data.Name = info.Name;
            data.Size = info.Length;
            data.Path = System.IO.Path.GetRelativePath(root, path);

            data.DateCreated = info.CreationTime;
            data.DateModified = info.LastWriteTime;

            return data;
        }

        public static PathData FolderData(string path, string root)
        {
            var data = new PathData();

            data.ID = Guid.NewGuid();
            data.Type = PathType.Folder;

            data.Name = new DirectoryInfo(path).Name;
            data.Path = System.IO.Path.GetRelativePath(root, path);

            return data;
        }

        public string AbsolutePath(string root) =>
            System.IO.Path.Combine(root, Path);
    }

    public enum PathType
    {
        File,
        Folder
    }
}
