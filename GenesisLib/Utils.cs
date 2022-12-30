using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib
{
    public delegate void ResourceEventHandler(Resource resource);

    public static class Utils
    {
        public static async IAsyncEnumerable<byte[]> SplitDataAsync(Stream stream, int maxSize = 1024 * 8, [EnumeratorCancellation] CancellationToken token = default)
        {
            do
            {
                var buffer = new byte[maxSize];
                var read = await stream.ReadAsync(buffer, 0, buffer.Length, token);

                yield return buffer.Take(read).ToArray();
            }
            while (stream.Position < stream.Length);
        }

        public static async IAsyncEnumerable<byte[]> SplitDataAsync(byte[] data, int maxSize = 1024 * 8, [EnumeratorCancellation] CancellationToken token = default)
        {
            using var ms = new MemoryStream(data);
            do
            {
                var buffer = new byte[maxSize];
                var read = await ms.ReadAsync(buffer, 0, buffer.Length, token);

                yield return buffer.Take(read).ToArray();
            }
            while (ms.Position < ms.Length);
        }

        public static int ToInt(this byte[] data)
        {
            if (data.Length != sizeof(int))
            {
                throw new ArgumentException("Data lenght is not sizeof(int)");
            }

            return BitConverter.ToInt32(data);
        }

        public static byte[] ToByte(this int num) =>
            BitConverter.GetBytes(num);
    }
}
