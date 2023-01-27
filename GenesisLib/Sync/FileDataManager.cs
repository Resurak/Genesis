using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib.Sync
{
    public class FileDataManager
    {
        public FileDataManager() 
        {

        }

        public PathItemList<FileSyncStream> StreamList { get; set; }

        public async Task WriteData(List<FileData> dataList)
        {
            var tasks = new List<Task>();
            foreach (var file in dataList)
            {
                var local = StreamList[file.Path];
                if (local != null)
                {
                    tasks.Add(Task.Run(() => local.WriteData(file.Data)));
                    continue;
                }

                var temp = new FileSyncStream(file.Path, file.Size, FileMode.Create);
                await temp.WriteData(file.Data);

                StreamList.Add(temp);
            }

            await Task.WhenAll(tasks);
        }

        public async IAsyncEnumerable<List<FileData>> ReadData(Share destination)
        {
            var tempList = new List<FileData>();
            var memoryStream = new MemoryStream();

            var usedMemory = 0;
            foreach (var file in destination.PathList)
            {
                if (usedMemory > 1024 * 512)
                {

                }

                var path = Path.Combine(destination.Root, file.Path);
                if (file.PathType == PathType.Directory)
                {
                    continue;
                }

                if (file.Size < 1024 * 64 && file.Size + usedMemory < 1024 * 512)
                {
                    var stream = new FileSyncStream(path, file.Size, FileMode.Open);
                    var data = await stream.GetData();

                    var fileData = new FileData(path, file.Size, data);
                    tempList.Add(fileData);
                }
            }
        }
    }
}
