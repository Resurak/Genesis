using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Commons
{
    public static class Extensions
    {
        public static int ToInt(this byte[] data) =>
            BitConverter.ToInt32(data);

        public static long ToLong(this byte[] data) =>
            BitConverter.ToInt64(data);

        public static byte[] ToBytes(this int num) =>
            BitConverter.GetBytes(num);

        public static byte[] ToBytes(this long num) =>
            BitConverter.GetBytes(num);
    }
}
