using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib.Sync
{
    public class SyncOptions
    {
        public SyncOptions()
        {
            TwoWay = false;
            AutoAcceptFile = true;

            MaxErrors = 0;
            FileBufferLength = 1024 * 16;
            StreamBufferLength = 1024 * 64;
        }

        public bool TwoWay { get; set; }
        public bool AutoAcceptFile { get; set; }

        public int MaxErrors { get; init; }
        public int FileBufferLength { get; init; }
        public int StreamBufferLength { get; init; } // currently not used
    }
}
