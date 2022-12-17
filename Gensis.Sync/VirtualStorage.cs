using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gensis.Sync
{
    public class VirtualStorage
    {
        public VirtualStorage()
        {
            Name = String.Empty;
            RootFolder = String.Empty;

            Files = new List<InfoFile>();
        }

        public Guid ID { get; set; }
        public string Name { get; set; }

        public string RootFolder { get; set; }
        public List<InfoFile> Files { get; set; }

        public InfoFile? this[Guid id] => Files.FirstOrDefault(x => x.ID == id);
        public InfoFile? this[string name] => Files.FirstOrDefault(f => f.Name == name);

        public static async Task<VirtualStorage> Create(string name, string root)
        {
            var vs = new VirtualStorage() { Name = name, RootFolder = root, ID = Guid.NewGuid() };

            try
            {
                await Task.Run(() =>
                {
                    foreach (var file in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
                    {
                        var info = new InfoFile(file, root);
                        vs.Files.Add(info);
                    }
                })
                .ContinueWith(t => 
                { 
                    if (t.Exception != null)
                    {
                        throw t.Exception;
                    }
                });
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Exception thrown while creating virtual storage");
            }

            return vs;
        }
    }
}
