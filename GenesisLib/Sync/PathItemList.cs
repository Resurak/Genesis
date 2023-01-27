using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib.Sync
{
    public class PathItemList<T> : List<T> where T : class, IPathable
    {
        public T? this[string path] =>
            this.FirstOrDefault(x => x.Path == path);
    }
}
