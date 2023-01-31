using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GenesisLibrary.Sync
{
    public class SyncManager
    {
        public SyncManager() { }

        public SyncManager(SyncShare local)
        {
            this.Local = local;
            this.Progress = new SyncProgress(local.SyncPathList.Sum(x => x.Size));
            //this.FilesToSync = files;
        }

        public SyncShare Local { get; set; }
        public SyncProgress Progress { get; set; }

        private SyncStream? tempFileStream;

        public async Task ProcessFileData(FileData fileData, IProgress<SyncProgress>? progress = null)
        {
            var data = Local.SyncPathList[fileData.ID];
            if (data == null)
            {
                throw new InvalidOperationException("File not found with id " + fileData.ID);
            }

            if (!fileData.Single)
            {
                if (tempFileStream == null)
                {
                    tempFileStream = new SyncStream(Local.Path, data, FileMode.Create);
                    await tempFileStream.WriteData(fileData.Data);
                }
                else
                {
                    await tempFileStream.WriteData(fileData.Data);
                }

                if (tempFileStream.Completed)
                {
                    tempFileStream.Dispose();
                    tempFileStream = null;

                    Progress.Update(0, data);
                }
            }
            else
            {
                using var stream = new SyncStream(Local.Path, data, FileMode.Create);
                await stream.WriteData(fileData.Data);

                Progress.Update(0, data);
            }

            Progress.Update(fileData.Data.Length);
            progress?.Report(Progress);
        }

        public async Task ProcessFileData(List<FileData> fileDataList, IProgress<SyncProgress>? progress = null)
        {
            foreach (var file in fileDataList)
            {
                await ProcessFileData(file, progress);
            }
        }

        public async Task SendFileData(TcpStream stream)
        {
            var list = new List<FileData>();
            foreach (var pathData in Local.SyncPathList)
            {
                if (list.Sum(x => x.Data.Length) > 1024 * 512)
                {
                    await stream.SendObject(list);
                    list.Clear();
                }

                using var syncStream = new SyncStream(Local.Path, pathData, FileMode.Open);

                if (pathData.Size < 1024 * 32)
                {
                    var data = await syncStream.GetData();

                    var fileData = new FileData(pathData, data);
                    list.Add(fileData);
                }
                else
                {
                    while (!syncStream.Completed)
                    {
                        var data = await syncStream.GetData();
                        var fileData = new FileData(pathData, data);

                        await stream.SendObject(fileData);
                    }
                }
            }
        }

        public static SyncItemList<PathData> GetFilesToSync(SyncShare share, List<Guid> idList)
        {
            var list = new SyncItemList<PathData>();
            foreach (var item in idList)
            {
                var pathData = share.LocalPathList[item];
                if (pathData != null)
                {
                    list.Add(pathData);
                }
            }

            return list;
        }
    }
}
