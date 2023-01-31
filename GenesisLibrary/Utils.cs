using GenesisLibrary.Sync;
using MessagePack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLibrary
{
    public delegate void StatusUpdateEventHandler();
    public delegate void SyncProgressEventHandler(SyncProgress progress);

    public static class Utils
    {
        public static byte[] ToPack(this object? obj) =>
            MessagePackSerializer.Typeless.Serialize(obj);

        public static object? FromPack(this byte[] data) =>
            MessagePackSerializer.Typeless.Deserialize(data);

        public static string ToJson(this object obj) =>
            JsonConvert.SerializeObject(obj);

        public static object? FromJson(this string json) =>
            JsonConvert.DeserializeObject(json);

        public static T? FromJson<T>(this string json) where T : class =>
            JsonConvert.DeserializeObject<T>(json);

        public static bool Empty(this string? str) =>
            str == null || str.Length == 0;

        public static string GetAbsolutePath(this string path, string root) =>
            Path.Combine(root, path);
    }
}
