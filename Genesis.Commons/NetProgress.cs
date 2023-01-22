using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Commons
{
    public class NetProgress
    {
        public NetProgress(long total) 
        {
            Total = total;
        }

        public long Total { get; set; }
        public long Current { get; set; }

        public int Percent => (int)Math.Ceiling((double)Total / (double)Current) * 100;

        public void Update(long current)
        {
            Current = current;
        }

    }
}
