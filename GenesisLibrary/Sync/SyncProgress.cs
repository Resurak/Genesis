using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLibrary.Sync
{
    public class SyncProgress
    {
        public SyncProgress() { }

        public SyncProgress(long total) 
        {
            Total = total;
            SyncedFiles = new List<PathData>();

            Start = DateTime.Now;
        }

        public long Total { get; set; }
        public long Current { get; set; }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public List<PathData> SyncedFiles { get; set; }

        public int Percent => (int)Math.Ceiling(((double)Total / Current) * 100);

        private long precedent;
        public void Update(int count, PathData? data = null)
        {
            Current += count;
            if (data != null)
            {
                SyncedFiles.Add(data);
            }
        }

        public void Stop()
        {
            Current = Total;
            End = DateTime.Now;
        }
    }
}
