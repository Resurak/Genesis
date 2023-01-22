using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Commons
{
    public class ShareList : List<ShareData>
    {
        public ShareData? this[Guid id]
        {
            get
            {
                if (Count == 0)
                {
                    return null;
                } 

                foreach (var item in this)
                {
                    if (item.ID == id)
                    {
                        return item;
                    }
                }

                return null;
            }
        }
    }
}
