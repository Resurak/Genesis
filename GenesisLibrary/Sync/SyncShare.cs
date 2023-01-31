using MessagePack;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLibrary.Sync
{
    public class SyncShare : ISyncElement, IDisposable
    {
        public SyncShare() { }

        public SyncShare(string root, SyncShareOptions? options = null)
        {
            this.ID = Guid.NewGuid();
            this.Path = root;

            this.Name = "PlaceHolder";
            this.Options = options != null ? options : new SyncShareOptions();

            this.SyncPathList = new SyncItemList<PathData>();
            this.DeletePathList= new SyncItemList<PathData>();

            this.LocalPathList = new SyncItemList<PathData>();
            this.InternalTimer = new Timer(TimerCallback, null, -1, -1);

            if (Options.AutoUpdate)
            {
                this.InternalTimer.Change(1000, Options.UpdateInterval);
            }
        }

        public Guid ID { get; set; }
        public string Path { get; set; }

        public string Name { get; set; }
        public DateTime LastUpdate { get; set; }

        public SyncShareOptions Options { get; set; }

        public SyncItemList<PathData> LocalPathList { get; set; }

        public SyncItemList<PathData> SyncPathList { get; set; }
        public SyncItemList<PathData> DeletePathList { get; set; }

        [IgnoreMember]
        private bool syncing;

        [IgnoreMember]
        private bool updating;

        [IgnoreMember]
        private Timer InternalTimer;

        public async Task Update()
        {
            if (updating)
            {
                return;
            }

            updating = true;

            var info = new DirectoryInfo(Path);
            LocalPathList = await GetPathData(info);

            updating = false;
            LastUpdate = DateTime.Now;
        }

        async Task<SyncItemList<PathData>> GetPathData(DirectoryInfo info)
        {
            var list = new SyncItemList<PathData>();
            var rootData = new PathData(Path, info);

            list.Add(rootData);
            foreach (var file in info.EnumerateFiles())
            {
                var fileData = new PathData(Path, file);
                LocalPathList.Add(rootData);
            }

            var taskList = new List<Task<SyncItemList<PathData>>>();
            foreach (var folder in info.EnumerateDirectories())
            {
                var folderData = new PathData(Path, folder);
                var task = Task.Run(() => GetPathData(folder));

                taskList.Add(task);
            }

            var pathList = await Task.WhenAll(taskList);
            foreach (var path in pathList)
            {
                list.AddRange(path);
            }

            return list;
        }

        async void TimerCallback(object? state = null)
        {
            Log.Debug("Updating share {id}", ID);
            await Update();
        }

        public SyncShareData CompareShare(SyncShare source)
        {
            Log.Verbose("Comparing local share with share {id}", source.ID);
            foreach (var pathData in source.LocalPathList)
            {
                var local = LocalPathList[pathData.Path];
                if (local == null)
                {
                    SyncPathList.Add(pathData);
                    continue;
                }

                if (local.LastWriteTime < pathData.LastWriteTime)
                {
                    SyncPathList.Add(pathData);
                    continue;
                }

                if (local.Size != pathData.Size)
                {
                    SyncPathList.Add(pathData);
                    continue;
                }
            }

            var shareData = new SyncShareData(source.ID, SyncPathList.Select(x => x.ID).ToList());
            return shareData;
        }

        public void Dispose()
        {
            InternalTimer.Dispose();
        }
    }
}
