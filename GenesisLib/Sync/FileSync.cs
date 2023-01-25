using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib.Sync
{
    public class FileSync : FileStream , IGuidItem
    {
        public Guid ID { get; set; }

        public FileSync(Guid id, string path, FileSyncMode mode, long length = 0) : base(path, 
            mode == FileSyncMode.Source ? FileMode.Open : FileMode.Create,
            mode == FileSyncMode.Source ? FileAccess.Read : FileAccess.Write,
            FileShare.None,
            BufferSize,
            mode == FileSyncMode.Source ? FileOptions.WriteThrough : FileOptions.None)
        {
            ID = id;

            if (length > 0)
            {
                SetLength(length);
            }
        }

        public async Task WriteBlock(byte[] buffer)
        {
            if (!CanStream)
            {
                throw new EndOfStreamException();
            }

            if (buffer.Length + Position > Length)
            {
                throw new InvalidOperationException("Buffer data overflow");
            }

            while (Streaming)
            {
                await Task.Delay(5);
            }

            Streaming = true;
            await WriteAsync(buffer);

            Streaming = false;
        }

        public async Task<byte[]> GetBlock()
        {
            if (!CanStream)
            {
                throw new EndOfStreamException();
            }

            while (Streaming)
            {
                await Task.Delay(5);
            }

            Streaming = true;

            var diff = Position + BufferSize < Length? BufferSize : Length - Position;
            var buffer = new byte[diff];

            await ReadExactlyAsync(buffer);

            Streaming = false;
            return buffer;
        }

        public bool Streaming { get; private set; }

        public bool CanStream => Position < Length;
        public bool Completed => Position == Length;

        public const int BufferSize = 1024 * 16;
    }

    public enum FileSyncMode
    {
        Source,
        Destination,
    }
}
