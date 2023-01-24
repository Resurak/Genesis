using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib.Sync
{
    public class FolderData : ISyncItem
    {
        public FolderData()
        {
            FileList = new SyncItemList<FileData>();
            FolderList = new SyncItemList<FolderData>();
        }

        public FolderData(string root, string path, params string[] exclusions) : this()
        {
            this.Name = Path.GetRelativePath(root, path);

            foreach (var file in Directory.EnumerateFiles(path))
            {
                if (exclusions.Length > 0 && exclusions.Contains(file))
                {
                    continue;
                }

                var fileData = new FileData(root, file);
                FileList.Add(fileData);
            }

            foreach (var folder in Directory.EnumerateDirectories(path))
            {
                if (exclusions.Length > 0 && exclusions.Contains(folder))
                {
                    continue;
                }

                var folderData = new FolderData(root, folder);
                FolderList.Add(folderData);
            }
        }

        public Guid ID { get; set; }
        public string Name { get; set; }

        public SyncItemList<FileData> FileList { get; set; }
        public SyncItemList<FolderData> FolderList { get; set; }

        public void Update(string root)
        {
            var removalList = new List<Guid>();

            for (int i = 0; i < FileList.Count; i++)
            {
                var file = FileList[i];
                var path = Path.Combine(root, file.Name);

                if (!File.Exists(path))
                {
                    removalList.Add(file.ID);
                    continue;
                }

                var fileData = new FileData(root, path);
                if (fileData.LastWriteDate > file.LastWriteDate)
                {
                    FileList[i] = fileData;
                } 

                // todo : add more check
            }

            foreach (var id in removalList)
            {
                var file = FileList[id];
                if (file != null)
                {
                    FileList.Remove(file);
                }
            }
        }
    }
}
