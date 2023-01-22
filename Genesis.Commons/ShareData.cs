using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Commons
{
    public class ShareData
    {
        public ShareData()
        {
            ID = Guid.NewGuid();
            Name = "Placeholder";

            Root = string.Empty;
            PathList = new PathList();
        }

        public ShareData(string root) : this()
        {
            Root = root;
        }

        public ShareData(string name, string root) : this(root)
        {
            Name = name;
        }

        public PathData? this[Guid id] =>
            PathList.FirstOrDefault(x => x.ID == id);

        public PathData? this[string path] =>
            PathList.FirstOrDefault(x => x.Path == path);

        public Guid ID { get; set; }

        public string Name { get; set; }
        public string Root { get; set; }

        public PathList PathList { get; set; }
        public DateTime LastUpdate { get; set; }

        public async Task Update()
        {
            if (Root.IsEmpty())
            {
                throw new ArgumentNullException("Root");
            }

            if (!Directory.Exists(Root))
            {
                throw new DirectoryNotFoundException(Root);
            }

            await Task.Run(() =>
            {
                PathList.Clear();
                GetPaths(Root);
            });

            LastUpdate = DateTime.Now;
        }

        public void PrepareShare(ShareData sourceShare)
        {
            foreach (var path in sourceShare.PathList)
            {
                var local = PathList[path.Path];
                if (local == null)
                {
                    path.Flag_Sync = true;
                    PathList.Add(path);
                }
                else
                {
                    local.ID = path.ID;
                    if (local.Hash.Length > 0 && !local.Hash.SequenceEqual(path.Hash))
                    {
                        local.Flag_Sync = true;
                        continue;
                    }

                    if (local.DateModified < path.DateModified)
                    {
                        local.Flag_Sync = true;
                        continue;
                    }

                    if (local.Size < path.Size)
                    {
                        local.Flag_Sync = true;
                        continue;
                    }
                }
            }

            foreach (var path in PathList)
            {
                var source = sourceShare.PathList[path.Path];
                if (source == null)
                {
                    path.Flag_Delete = true;
                }
            }
        }

        void GetPaths(string path)
        {
            foreach (var file in Directory.EnumerateFiles(path))
            {
                var data = PathData.FileData(file, Root);
                PathList.Add(data);
            }

            foreach (var dir in Directory.EnumerateDirectories(path))
            {
                var data = PathData.FolderData(dir, Root);
                PathList.Add(data);

                GetPaths(dir);
            }
        }
    }
}
