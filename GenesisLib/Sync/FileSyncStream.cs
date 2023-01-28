using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib.Sync
{
    public class FileSyncStream : IPathable, IDisposable
    {
        public FileSyncStream(string path, long size, FileMode mode, SyncOptions? options = null) 
        {
            this.Path = path;
            this.BufferSize = options != null ? options.FileBufferLength : 1024 * 16;

            this.Stream = new FileStream(this.Path, mode, mode == FileMode.Open ? FileAccess.Read : FileAccess.Write, FileShare.None, 1024 * 64);
            if (mode == FileMode.Create)
            {
                this.Stream.SetLength(size);
            }
            this.Stream.Position = 0;
        }

        FileStream Stream;
        public string Path { get; set; }

        public int BufferSize { get; init; }
        public bool Completed => Stream.Position == Stream.Length;

        public async Task WriteData(byte[] data)
        {
            await this.Stream.WriteAsync(data, 0, data.Length);

            if (Completed)
            {
                Dispose();
            }
        }

        public async Task<byte[]> GetData()
        {
            var diff = Stream.Position + BufferSize < Stream.Length ? BufferSize : Stream.Length - Stream.Position;
            var buffer = new byte[diff];

            await Stream.ReadExactlyAsync(buffer);

            if (Completed)
            {
                Dispose();
            }

            return buffer;
        }

        public void Dispose()
        {
            Stream.Dispose();
        }
    }
}
