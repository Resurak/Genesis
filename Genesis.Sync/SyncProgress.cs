using Genesis.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Sync
{
    public class SyncProgress
    {
        public SyncProgress(long totalSyncSize) 
        {
            TotalSyncBytes = totalSyncSize;
            StartDate = DateTime.Now;
        }

        public PathData? CurrentFile { get; set; }

        public long TotalFileBytes { get; set; }
        public long CurrentFileBytes { get; set; }

        public long TotalSyncBytes { get; set; }
        public long CurrentSyncBytes { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int FilePercent => (int)Math.Ceiling(((double)CurrentFileBytes / TotalFileBytes) * 100);
        public int SyncPercent => (int)Math.Ceiling(((double)CurrentSyncBytes / TotalSyncBytes) * 100);

        public void Stop()
        {
            CurrentFile = null;

            CurrentFileBytes = 0;
            TotalFileBytes = 0;

            CurrentSyncBytes = TotalSyncBytes;
            EndDate = DateTime.Now;
        }
    }
}
