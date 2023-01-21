using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Commons
{
    public class FolderData : IData
    {
        public FolderData()
        {
            ID = Guid.NewGuid();

            Files = new DataList<FileData>();
        }

        public FolderData(string root, string path) : this()
        {
            Path = System.IO.Path.GetRelativePath(root, path);

            foreach (var file in Directory.EnumerateFiles(path))
            {
                var data = new FileData(file, root);
                Files.Add(data);
            }

            foreach (var dir in Directory.EnumerateDirectories(path)) 
            {
                FolderCount++;
            }
        }

        public Guid ID { get; set; }
        public string Path { get; set; }

        public DataList<FileData> Files { get; set; }

        public int FileCount { get; private set; }
        public int FolderCount { get; private set; }
    }
}
