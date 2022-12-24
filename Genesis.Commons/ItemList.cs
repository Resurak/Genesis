using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Commons
{
    public class ItemList<T> : Dictionary<Guid, T> where T : class
    {
        public ItemList() : base()
        {

        }
    }
}
