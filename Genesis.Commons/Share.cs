using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Commons
{
    public class Share
    {
        public Share()
        {
            ID = Guid.NewGuid();
            PathList = new List<PathData>();

            Name = "Placeholder";
            RootFolder = string.Empty;

        }

        public Share(string path) : this()
        {
            RootFolder = path;
        }

        public Guid ID { get; set; }
        public string Name { get; set; }
        public string RootFolder { get; set; }

        public List<PathData> PathList { get; set; }

        bool _canSync;
        Guid _sourceShareID;

        public PathData? this[PathData data] =>
            PathList.FirstOrDefault(x => x.Path == data.Path);

        public async Task Update()
        {
            if (RootFolder.IsEmpty())
            {
                throw new InvalidOperationException("Can't update share if root path is not setted");
            }

            if (!Directory.Exists(RootFolder))
            {
                throw new DirectoryNotFoundException(RootFolder);
            }

            PathList.Clear();

            await Task.Run(() =>
            {
                foreach (var path in GetPaths(RootFolder))
                {
                    PathList.Add(path);
                }
            });
        }

        IEnumerable<PathData> GetPaths(string path)
        {
            foreach (var file in Directory.EnumerateFiles(path))
            {
                var data = PathData.FileData(file, RootFolder);
                yield return data;
            }

            foreach (var dir in Directory.EnumerateDirectories(path))
            {
                var data = PathData.FolderData(dir, RootFolder);
                yield return data;

                foreach (var sub in GetPaths(dir))
                {
                    yield return sub;
                }
            }
        }

        public void PrepareShare(Share sourceShare)
        {
            foreach (var path in sourceShare.PathList)
            {
                var local = this[path];
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
                var source = sourceShare[path];
                if (source == null)
                {
                    path.Flag_Delete = true;
                }
            }

            _canSync = true;
            _sourceShareID = sourceShare.ID;
        }

        public async Task SyncLocal(Share share)
        {
            if (!_canSync || _sourceShareID != share.ID)
            {
                PrepareShare(share);
            }

            foreach (var path in PathList)
            {
                var sourcePath = Path.Combine(share.RootFolder, path.Path);
                var destinationPath = Path.Combine(RootFolder, path.Path);

                if (path.Flag_Sync)
                {
                    switch (path.Type)
                    {
                        case PathType.File:
                        {
                            using var sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.None);
                            using var destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None);

                            await sourceStream.CopyToAsync(destinationStream);

                            Log.Information("Synced {file}", destinationPath);
                        }
                            break;
                        case PathType.Folder:
                        {
                            if (!Directory.Exists(destinationPath))
                            {
                                Directory.CreateDirectory(destinationPath);
                            }
                        }
                            break;
                    }
                }

                if (path.Flag_Delete)
                {
                    switch (path.Type)
                    {
                        case PathType.File:
                            if (File.Exists(destinationPath))
                                File.Delete(destinationPath);
                            break;
                        case PathType.Folder:
                            if (Directory.Exists(destinationPath))
                                Directory.Delete(destinationPath, true);
                            break;
                    }
                }
            }
        }
    }
}
