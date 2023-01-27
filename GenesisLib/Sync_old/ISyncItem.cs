using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib.Sync_old
{
    public interface ISyncItem : IGuidItem
    {
        public string Name { get; set; }
    }
}
