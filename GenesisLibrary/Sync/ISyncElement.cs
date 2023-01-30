using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLibrary.Sync
{
    public interface ISyncElement
    {
        public Guid ID { get; set; }
        public string Path { get; set; }
    }
}
