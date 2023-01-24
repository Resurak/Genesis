using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib.Sync
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

        async Task GetFolderInfo(string path, PathData data)
        {
            var tasks = new List<Task>();
            var dirInfo = new DirectoryInfo(path);

            var tempData = new List<PathData>();
            foreach (var file in dirInfo.EnumerateFiles())
            {
                var pathData = new PathData(file.Name, PathType.File, file.Length, file.CreationTime, file.LastWriteTime);
                data.FileList.Add(pathData);
            }

            foreach (var folder in dirInfo.EnumerateDirectories()) 
            {
                var pathData = new PathData(folder.Name, PathType.Folder);
                tasks.Add(Task.Run(() => GetFolderInfo(folder.FullName, pathData)));
            }

            await Task.WhenAll(tasks);
            data.FolderList.AddRange(tempData);
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
    }
}
