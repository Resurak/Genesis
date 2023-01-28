using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib.Sync
{
    public class SyncProgress
    {
        public SyncProgress(long total) 
        {
            TotalSize = total;
            FileCompleted = new List<string>();
        }

        public long TotalSize { get; set; }
        public long CurrentSize { get; set; }

        public int Progress => (int)Math.Ceiling(((double)TotalSize / CurrentSize) * 100);
        public List<string> FileCompleted { get; set; }

        public void Update(long size, params string[] files)
        {
            CurrentSize += size;
            if (files.Length > 0)
            {
                FileCompleted.AddRange(files);
            }
        }
    }
}
