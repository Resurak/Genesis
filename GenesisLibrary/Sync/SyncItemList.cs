using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLibrary.Sync
{
    public class SyncItemList<T> : List<T> where T : class, ISyncElement
    {
        public T? this[Guid id] =>
            this.FirstOrDefault(x => x.ID == id);

        public T? this[string path] =>
            this.FirstOrDefault(x => x.Path == path);

        public T? this[Guid id, string path] =>
            this.FirstOrDefault(x => x.ID == id && x.Path == path);
    }
}
