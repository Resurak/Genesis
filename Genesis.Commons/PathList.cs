using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Commons
{
    public class PathList : List<PathData>
    {
        public PathData? this[string path]
        {
            get
            {
                if (Count == 0)
                {
                    return null;
                }

                foreach (var item in this)
                {
                    if (item.Path == path)
                    {
                        return item;
                    }
                }

                return null;
            }
        }

        public PathData? this[Guid id]
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
