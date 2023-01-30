using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLibrary.Sync
{
    public class SyncShareData : ISyncElement
    {
        public SyncShareData() 
        {

        }

        public SyncShareData(SyncShare share)
        {
            this.ID = share.ID;
            this.Path = share.Path;

            this.PathList = share.PathList;
        }

        public Guid ID { get; set; }
        public string Path { get; set; }

        public SyncItemList<PathData> PathList { get; set; }
    }
}
