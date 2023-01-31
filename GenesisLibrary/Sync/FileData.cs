using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLibrary.Sync
{
    public class FileData
    {
        public FileData()
        {

        }

        public FileData(PathData pathData, byte[] data)
        {
            this.ID = pathData.ID;
            this.Data = data;

            this.Single = pathData.Size < 1024 * 32;
        }

        public Guid ID { get; set; }
        public bool Single { get; set; }

        public byte[] Data { get; set; }
    }
}
