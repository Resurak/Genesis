using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLibrary.Sync
{
    public class SyncShareData
    {
        public SyncShareData() 
        {

        }

        public SyncShareData(Guid shareID, List<Guid> fileIDList)
        {
            this.ID = shareID;
            this.IDList = fileIDList;
        }

        public Guid ID { get; set; }
        public List<Guid> IDList { get; set; }
    }
}
