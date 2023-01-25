using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib.Sync
{
    public class PathData : ISyncItem
    {
        public PathData()
        {
            Flags = new SyncFlags();

            FileList = new SyncItemList<PathData>();
            FolderList = new SyncItemList<PathData>();
        }

        public PathData(string name, PathType type, long length = 0, DateTime creation = default, DateTime lastwrite = default) : this()
        {
            ID = Guid.NewGuid();

            Name = name;
            Type = type;

            Size = length;
            CreationDate = creation;
            LastWriteDate = lastwrite;
        }

        public Guid ID { get; set; }
        public long Size { get; set; }
        public SyncFlags Flags { get; set; }

        public string Name { get; set; }
        public PathType Type { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime LastWriteDate { get; set; }

        public SyncItemList<PathData> FileList { get; set; }
        public SyncItemList<PathData> FolderList { get; set; }

        public int FileCount => FileList.Count;
        public int FolderCount => FolderList.Count;

        public int TotalFileCount
        {
            get
            {
                var total = 0;
                foreach (var item in FileList)
                {
                    total += 1;
                }

                foreach (var item in FolderList)
                {
                    total += item.TotalFileCount;
                }

                return total;
            }
        }

        public long TotalFileSize
        {
            get
            {
                var total = 0L;
                foreach (var item in FileList)
                {
                    total += item.Size;
                }

                foreach (var item in FolderList)
                {
                    total += item.TotalFileSize;
                }

                return total;
            }
        }

        public void ToSync()
        {
            Flags = new SyncFlags(true);
        }

        public void ToDelete()
        {
            Flags = new SyncFlags(false, true);
        }
    }

    public enum PathType
    {
        File,
        Folder,

        Reserved1,
        Reserved2,
        Reserved3,
    }
}
