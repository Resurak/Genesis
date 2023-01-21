using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Commons
{
    public class FileData : IData
    {
        public FileData() 
        {

        }

        public FileData(string path, string root, bool hashFile = true)
        {
            try
            {
                this.ID = Guid.NewGuid();
                var info = new FileInfo(path);

                this.Size = info.Length;
                this.Hash = hashFile ? Utils.HashFile(path) : new byte[0];

                this.Name = info.Name;
                this.Path = System.IO.Path.GetRelativePath(root, path);

                this.CreationTime = info.CreationTime;
                this.LastWriteTime = info.LastWriteTime;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error detected while creating file data");
            }
        }

        public Guid ID { get; set; }

        public long Size { get; set; }
        public byte[] Hash { get; set; }

        public string Name { get; set; }
        public string Path { get; set; }

        public DateTime CreationTime { get; set; }
        public DateTime LastWriteTime { get; set; }
    }
}
