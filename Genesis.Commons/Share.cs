using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Commons
{
    public class Share : IData
    {
        public Share()
        {
            ID = Guid.NewGuid();
            Name = "Placeholder";

            Folders = new DataList<FolderData>();
        }

        public Share(string path) : this()
        {
            RootPath = path;
        }

        public Guid ID { get; set; }
        public string? Name { get; set; }
        public string? RootPath { get; set; }

        public DataList<FolderData> Folders { get; set; }

        public async Task Update()
        {
            if (RootPath.IsEmpty())
            {
                throw new InvalidOperationException("Can't update share if root path is not setted");
            }

            if (!Directory.Exists(RootPath))
            {
                throw new DirectoryNotFoundException(RootPath);
            }

            var rootFolder = new FolderData(RootPath, RootPath);
            Folders.Add(rootFolder);

            await Task.Run(() =>
            {
                foreach (var folder in Directory.EnumerateDirectories(RootPath, "*", SearchOption.AllDirectories))
                {
                    var data = new FolderData(RootPath, folder);
                    Folders.Add(data);
                }
            });
        }

        public IEnumerable<FileData> EnumerateDifferentFiles(Share share)
        {
            foreach (var folder in share.Folders)
            {
                // check if there is local folder
                var localFolder = Folders.FirstOrDefault(x => x.Path == folder.Path);

                if (localFolder == null)
                {
                    // no local folder, add all files
                    foreach (var file in folder.Files)
                    {
                        yield return file;
                    }
                }
                else
                {
                    // check each file in folder
                    foreach (var file in folder.Files)
                    {
                        var localFile = localFolder.Files.FirstOrDefault(x => x.Path == file.Path);
                        if (localFile == null)
                        {
                            // no local file, add
                            yield return file;
                        }
                        else
                        {
                            // check if file hash is the same
                            if (localFile.Hash.Length > 0 && !localFile.Hash.SequenceEqual(file.Hash))
                            {
                                yield return file;
                                continue;
                            }

                            // check if local file is older
                            if (localFile.LastWriteTime < file.LastWriteTime)
                            {
                                yield return file;
                                continue;
                            }

                            // check if local file size is different
                            if (localFile.Size != file.Size)
                            {
                                yield return file;
                                continue;
                            }
                        }
                    }
                }
            }
        }
    }
}
