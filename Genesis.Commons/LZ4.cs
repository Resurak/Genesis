using K4os.Compression.LZ4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Commons
{
    public class LZ4
    {
        public static byte[] Compress(byte[] data) =>
            LZ4Pickler.Pickle(data);

        public static byte[] Decompress(byte[] data) =>
            LZ4Pickler.Unpickle(data);
    }
}
