using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Sync
{
    public class BasePacket
    {
        public Type? ParamType { get; set; }
        public object? ParamValue { get; set; }

        public void WithParam<T>(T param)
        {
            this.ParamType = typeof(T);
            this.ParamValue = param;
        }
    }
}
