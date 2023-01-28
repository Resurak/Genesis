using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib.Sync
{
    public class SyncManager
    {
        public SyncManager(Share share, SyncOptions options)
        {
            this.Share = share;
            this.Options = options;
            this.FileSyncStreamList = new PathItemList<FileSyncStream>();
        }

        public Share Share { get; set; }
        public SyncOptions Options { get; set; }

        public long TotalSize { get; set; }
        public long CurrentSize { get; set; }

        public int Progress => (int)Math.Ceiling(((double)TotalSize / CurrentSize) * 100);

        public PathItemList<FileSyncStream> FileSyncStreamList { get; set; }

        public async IAsyncEnumerable<FileData> EnumerateFileData(PathItemList<PathData> pathItemList)
        {
            foreach (var item in pathItemList)
            {
                var path = Path.Combine(Share.Root, item.Path);
                using var stream = new FileSyncStream(path, item.Size, FileMode.Open);

                while (!stream.Completed)
                {
                    var data = await stream.GetData();
                    yield return new FileData(item.Path, item.Size, data);
                }
            }
        }

        public async Task ProcessIncoming(FileData data) =>
            await ProcessData(data, FileMode.Create);

        async Task ProcessData(FileData data, FileMode mode)
        {
            var path = Path.Combine(Share.Root, data.Path);
            var stream = FileSyncStreamList[path];

            if (stream == null)
            {
                stream = new FileSyncStream(path, data.Size, mode);
                await stream.WriteData(data.Data);

                FileSyncStreamList.Add(stream);
            }
            else
            {
                await stream.WriteData(data.Data);
            }

            FileSyncStreamList.RemoveAll(x => x.Completed);
        }
    }
}
