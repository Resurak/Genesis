using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib
{
    public delegate void UpdateEventHandler();
    public delegate void ObjectEventHandler(object? obj);
    public delegate void ExceptionEventHandler(Exception ex);

    public static class Utils
    {
        public static byte[] Serialize(this object? obj) =>
            MessagePackSerializer.Typeless.Serialize(obj);

        public static object Deserialize(this byte[] data) =>
            MessagePackSerializer.Typeless.Deserialize(data);

        public static bool Empty(this string? str) =>
            str == null || str.Length == 0;
    }
}
