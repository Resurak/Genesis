using K4os.Compression.LZ4;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Commons
{
    public class Utils
    {
        public static byte[] Compress(byte[] data) =>
            LZ4Pickler.Pickle(data);

        public static byte[] Decompress(byte[] data) =>
            LZ4Pickler.Unpickle(data);

        public static byte[] HashFile(string path)
        {
            using var md5 = MD5.Create();
            using var file = File.OpenRead(path);
            return md5.ComputeHash(file);
        }

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

        public static T? Deserialize<T>(byte[] data, bool throwException = false) where T : class
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

        public static object? DeserializeObject(byte[] data, bool throwException = true)
        {
            try
            {
                return Deserialize<object>(data, throwException);
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
