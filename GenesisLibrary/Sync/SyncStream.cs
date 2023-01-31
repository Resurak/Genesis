using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLibrary.Sync
{
    public class SyncStream : FileStream, ISyncElement
    {
        public SyncStream(string root, PathData data, FileMode mode) : base(System.IO.Path.Combine(root, data.Path), mode, mode == FileMode.Open ? FileAccess.Read : FileAccess.Write, FileShare.None, 1024 * 64) 
        {
            this.ID = data.ID;
            this.Path = data.Path;
        }

        public Guid ID { get; set; }
        public string Path { get; set; }

        public bool Completed => Position < Length;

        public async Task<byte[]> GetData(int count = -1)
        {
            try
            {
                if (count == -1)
                {
                    if (Length < 1024 * 32)
                    {
                        count = (int)Length;
                    }
                    else
                    {
                        count = Position + 1024 * 32 < Length ? 1024 * 32 : (int)(Length - Position);
                    }
                }

                var buffer = new byte[count];
                await ReadExactlyAsync(buffer);

                return buffer;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while reading data from file {path}", base.Name);
                return new byte[0];
            }
        }

        public async Task WriteData(byte[] buffer)
        {
            try
            {
                await WriteAsync(buffer);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while writing data to file {path}", base.Name);
            }
        }
    }
}
