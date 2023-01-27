using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib.Sync_old
{
    public struct SyncFlags
    {
        public SyncFlags(bool sync, bool delete = false)
        {
            this.Sync = sync;
            this.Delete = delete;
        }

        public bool Sync { get; set; }
        public bool Delete { get; set; }
    }
}
