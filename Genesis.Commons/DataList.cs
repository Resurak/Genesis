using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Commons
{
    public class DataList<T> : List<T> where T : class, IData
    {
        public T? this[Guid id] =>
            Find(x => x.ID == id);
    }
}
