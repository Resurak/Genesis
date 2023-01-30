using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLibrary.Sync
{
    public class FileData : ISyncElement
    {
        public FileData()
        {

        }

        public FileData(PathData pathData, byte[] data)
        {
            this.ID = pathData.ID;
            this.Path = pathData.Path;

            this.Data = data;
        }

        public Guid ID { get; set; }
        public string Path { get; set; }

        public byte[] Data { get; set; }
    }
}
