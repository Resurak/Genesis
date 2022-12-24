using Genesis.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Sync
{
    public class Storage
    {
        public Storage()
        {
            ID = Guid.NewGuid();
            Name = string.Empty;
            Path = string.Empty;
            Files = new ItemList<FileData>();
        }

        public Storage(string path) : this()
        {
            Path = path;
        }

        public Storage(string name, string path) : this(path)
        {
            Name = name;
        }

        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }

        public ItemList<FileData> Files { get; set; }

        public static async Task<Storage> Create(string path)
        {
            Storage storage = new Storage();

            await Task.Run(() =>
            {
                foreach (var file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
                {
                    var data = new FileData(file, path);
                    storage.Files.Add(Guid.NewGuid(), data);
                }
            });

            return storage;
        }
    }
}
