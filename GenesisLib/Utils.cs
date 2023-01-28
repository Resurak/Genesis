using MessagePack;
using Newtonsoft.Json;
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
        public static byte[] Msg_Serialize(this object? obj) =>
            MessagePackSerializer.Typeless.Serialize(obj);

        public static object Msg_Deserialize(this byte[] data) =>
            MessagePackSerializer.Typeless.Deserialize(data);

        public static byte[] Serialize(this object obj) =>
            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));

        public static object Deserialize(this byte[] data) =>
            JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data));

        public static bool Empty(this string? str) =>
            str == null || str.Length == 0;
    }
}
