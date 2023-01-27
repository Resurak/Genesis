using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib.Sync
{
    public class FileSyncStream : IPathable
    {
        public FileSyncStream(string path, long size, FileMode mode) 
        {
            this.Path = path;

            this.Stream = new FileStream(path, mode, mode == FileMode.Open ? FileAccess.Read : FileAccess.Write, FileShare.None, 1024 * 64, FileOptions.WriteThrough);
            this.Stream.SetLength(size);
        }

        FileStream Stream;
        public string Path { get; set; }

        public int BufferSize => 1024 * 64;
        public bool Completed => Stream.Position == Stream.Length;

        public async Task WriteData(byte[] data)
        {
            await this.Stream.WriteAsync(data, 0, data.Length);
        }

        public async Task<byte[]> GetData()
        {
            var diff = Stream.Position + BufferSize < Stream.Length ? BufferSize : Stream.Length - Stream.Position;
            var buffer = new byte[diff];

            await Stream.ReadExactlyAsync(buffer);
            return buffer;
        }
    }
}
