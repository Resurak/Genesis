using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Commons
{
    public class DataStorage
    {
        public DataStorage()
        {
            this.ID = Guid.NewGuid();
            this.Root = String.Empty;
            this.Files = new List<DataFile>();
        }

        public DataFile? this[Guid id] =>
            Files?.FirstOrDefault(x => x.ID == id);

        public Guid ID { get; set; }
        public string Root { get; set; }

        public List<DataFile> Files { get; set; }

        public int FileCount =>
            Files?.Count ?? 0;

        public long TotalSize => 
            Files?.Sum(x => x.Size) ?? 0;

        public static async Task<DataStorage> Create(string root)
        {
            var storage = new DataStorage();
            storage.Root = root;

            await Task.Run(() =>
            {
                foreach (var file in Directory.EnumerateFiles(storage.Root, "*", SearchOption.AllDirectories))
                {
                    var data = new DataFile(file, storage.Root);
                    storage.Files.Add(data);
                }
            });

            return storage;
        }
    }
}
