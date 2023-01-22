using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Commons
{
    public class NetProgress
    {
        public NetProgress(long total, Guid fileID) 
        {
            ID = fileID;

            Total = total;
            DateStarted= DateTime.Now;
        }

        public Guid ID { get; set; }

        public long Total { get; set; }
        public long Current { get; set; }

        public DateTime DateStarted { get; set; }
        public DateTime DateFinished { get; set; }

        public int Percent => (int)Math.Ceiling((double)Total / (double)Current) * 100;

        public void Update(long current)
        {
            Current = current;
        }

        public void Finish()
        {
            Current = Total;
            DateFinished = DateTime.Now;
        }
    }
}
