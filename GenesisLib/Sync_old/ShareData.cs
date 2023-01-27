using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib.Sync_old
{
    public class ShareData
    {
        public ShareData() { }

        public ShareData(string root)
        {
            this.Root = root;
        }

        public string Root { get; set; }
        public PathData RootPathData { get; set; }

        public async Task Update()
        {
            RootPathData = await GetPathData(Root);
        }

        async Task<PathData> GetPathData(string path)
        {
            var dirInfo = new DirectoryInfo(path);
            var pathData = new PathData(dirInfo.Name, PathType.Folder);

            foreach (var file in dirInfo.EnumerateFiles())
            {
                var fileData = new PathData(file.Name, PathType.File, file.Length, file.CreationTime, file.LastWriteTime);
                pathData.FileList.Add(fileData);
            }

            var taskList = new List<Task<PathData>>();
            foreach (var folder in dirInfo.EnumerateDirectories())
            {
                taskList.Add(Task.Run(() => GetPathData(folder.FullName)));
            }

            pathData.FolderList.AddRange(await Task.WhenAll(taskList));
            return pathData;
        }

        public async Task CompareShares(ShareData source, bool twoWay = false)
        {
            await Task.Run(() =>
            {
                ComparePathData(RootPathData, source.RootPathData, twoWay);
            });
        }

        void ComparePathData(PathData local, PathData source, bool twoWay = false)
        {
            foreach (var file in source.FileList)
            {
                var data = local.FileList[file.Name];
                if (data == null)
                {
                    file.ToSync();
                    local.FileList.Add(file);

                    continue;
                }

                if (data.LastWriteDate < file.LastWriteDate)
                {
                    data.ToSync();
                    continue;
                }

                // todo: add more checking
            }

            foreach (var folder in source.FolderList)
            {
                var data = local.FolderList[folder.Name];
                if (data == null)
                {
                    folder.ToSync();
                    local.FolderList.Add(folder);

                    continue;
                }

                ComparePathData(data, folder, twoWay);
            }
        }

        //public async Task<PathData> GetFilesToSync()
        //{
        //    var list = new List<PathData>();

        //}
    }
}
