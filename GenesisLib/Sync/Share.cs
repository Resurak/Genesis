using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib.Sync
{
    public class Share : IGuidItem
    {
        public Share()
        {

        }

        public Share(string root)
        {
            this.ID = Guid.NewGuid();

            this.Root = root;
            this.PathList = new PathItemList<PathData>();
        }

        public Guid ID { get; set; }
        public string Root { get; set; }

        public PathItemList<PathData> PathList { get; set; }

        public async Task Update()
        {
            var info = new DirectoryInfo(Root);
            PathList = await GetPathList(info);
        }

        async Task<PathItemList<PathData>> GetPathList(DirectoryInfo info)
        {
            var dir = new PathData(info, Root);
            PathList.Add(dir);

            var list = new PathItemList<PathData>();
            foreach (var file in info.EnumerateFiles())
            {
                list.Add(new PathData(file, Root));
            }

            var taskList = new List<Task<PathItemList<PathData>>>();
            foreach (var folder in info.EnumerateDirectories())
            {
                taskList.Add(Task.Run(() => GetPathList(folder)));
            }

            var listOfFileDataList = await Task.WhenAll(taskList);
            foreach (var fileDataList in listOfFileDataList)
            {
                list.AddRange(fileDataList);
            }

            return list;
        }

        //public void CompareShare(Share source)
        //{
        //    foreach (var item in source.PathList)
        //    {
        //        var local = PathList[item.Path];
        //        if (local == null)
        //        {
        //            item.SyncFlag = true;
        //            PathList.Add(item);

        //            continue;
        //        }

        //        if (local.LastWriteTime < item.LastWriteTime)
        //        {
        //            local.SyncFlag = true;
        //            continue;
        //        }

        //        // todo: add more checking
        //    }

        //    foreach (var item in PathList)
        //    {
        //        var original = source.PathList[item.Path];
        //        if (original == null)
        //        {
        //            item.DeleteFlag = true;
        //        }
        //    }
        //}

        public PathItemList<PathData> CompareShare(Share source)
        {
            var pathList = new PathItemList<PathData>();
            foreach (var item in source.PathList)
            {
                var local = PathList[item.Path];
                if (local == null)
                {
                    item.SyncFlag = true;
                    pathList.Add(item);

                    continue;
                }

                if (local.LastWriteTime < item.LastWriteTime)
                {
                    item.SyncFlag = true;
                    pathList.Add(item);
                    continue;
                }

                // todo: add more checking
            }

            foreach (var item in PathList)
            {
                var original = source.PathList[item.Path];
                if (original == null)
                {
                    item.DeleteFlag = true;
                    pathList.Add(item);
                }
            }

            return pathList;
        }
    }
}
