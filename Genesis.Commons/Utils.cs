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
    public delegate void UpdateEventHandler(object? obj = null);
    public delegate void DataChangeEventHandler(int count = 0);

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

        public static byte[] Serialize<T>(T obj, bool throwException = true) =>
            MessagePackSerializer.Typeless.Serialize(obj);

        public static T? TypeDeserialize<T>(byte[] data, bool throwException = false) where T : class =>
            MessagePackSerializer.Typeless.Deserialize(data) as T;

        public static object? DynamicDeserialize(byte[] data, bool throwException = true) =>
            MessagePackSerializer.Typeless.Deserialize(data);
    }
}
