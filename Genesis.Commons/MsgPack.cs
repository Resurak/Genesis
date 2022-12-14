using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Commons
{
    public class MsgPack
    {
        public static byte[] Serialize<T>(T obj) =>
            MessagePackSerializer.Typeless.Serialize(obj);

        public static T? Deserialize<T>(byte[] data) where T : class =>
            MessagePackSerializer.Typeless.Deserialize(data) as T;

        //public static byte[] SerializeAndCompress<T>(T obj) => 
        //    LZ4.Compress
    }
}
