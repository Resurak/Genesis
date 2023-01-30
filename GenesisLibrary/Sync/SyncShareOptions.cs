using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLibrary.Sync
{
    public class SyncShareOptions
    {
        public SyncShareOptions() { }

        public bool AutoUpdate { get; set; } = true;
        public bool LazyLoading { get; set; } = false;

        public int MaxFiles { get; set; } = -1;
        public int UpdateInterval { get; set; } = 60_000; // 1 min
    }
}
