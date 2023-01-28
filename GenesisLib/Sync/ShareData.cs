using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib.Sync
{
    public class ShareData
    {
        public ShareData()
        {

        }

        public ShareData(Guid iD, PathItemList<PathData> items)
        {
            this.ID = iD;
            this.Items = items;
        }

        public Guid ID { get; set; }
        public PathItemList<PathData> Items { get; set; }
    }
}
