using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Commons
{
    public class MessagePackUtils
    {
        public static byte[] Serialize<T>(T obj, bool throwException = true)
        {
            try
            {
                return MessagePackSerializer.Typeless.Serialize(obj);
            }
            catch 
            {
                if (throwException)
                {
                    throw;
                }
                else
                {
                    return new byte[0];
                }
            }
        }

        public static T? Deserialize<T>(byte[] data, bool throwException = true) where T : class
        {
            try
            {
                return MessagePackSerializer.Typeless.Deserialize(data) as T;
            }
            catch
            {
                if (throwException)
                {
                    throw;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
