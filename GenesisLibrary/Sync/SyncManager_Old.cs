using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLibrary.Sync
{
    public class SyncManager_Old
    {
        //static SyncItemList<SyncStream> Streams = new SyncItemList<SyncStream>();

        //public static void CheckFolders(SyncShare share, SyncShare local)
        //{
        //    foreach (var folder in share.PathList.Where(x => x.Type == PathType.Folder))
        //    {
        //        var path = folder.Path.GetAbsolutePath(share.Path);
        //        if (!Directory.Exists(path))
        //        {
        //            Directory.CreateDirectory(path);
        //        }
        //    }

        //    foreach (var folder in local.PathList.Where(x => x.Type == PathType.Folder))
        //    {
        //        var path = folder.Path.GetAbsolutePath(share.Path);
        //        var source = share.PathList[folder.ID];

        //        if (source == null)
        //        {
        //            if (Directory.Exists(path))
        //            {
        //                Directory.Delete(path, true);
        //            }
        //        }
        //    }
        //}

        //public static void CheckFiles(SyncShare share, SyncShare local)
        //{
        //    foreach (var file in local.PathList.Where(x => x.Type == PathType.File))
        //    {
        //        var path = file.Path.GetAbsolutePath(share.Path);
        //        var source = share.PathList[file.ID];

        //        if (source == null)
        //        {
        //            if (File.Exists(path))
        //            {
        //                File.Delete(path);
        //            }
        //        }
        //    }
        //}

        //public async static Task SendFiles(SyncShareData share, TcpStream stream)
        //{
        //    var size = 0L;
        //    var fileDataList = new List<FileData>();

        //    foreach (var file in share.PathList)
        //    {
        //        if (size > 1024 * 512)
        //        {
        //            foreach (var fileData in fileDataList)
        //            {
        //                await stream.SendObject(fileData);
        //            }

        //            size = 0L;
        //            fileDataList.Clear();
        //        }

        //        if (file.Type == PathType.Folder)
        //        {
        //            continue;
        //        }

        //        var path = file.Path.GetAbsolutePath(share.Path);
        //        using var fileStream = new SyncStream(share.Path, file, FileMode.Open);

        //        if (file.Size < 1024 * 512)
        //        {
        //            var data = await fileStream.GetData((int)file.Size);

        //            var fileData = new FileData(file, data);
        //            fileDataList.Add(fileData);

        //            size += file.Size;
        //        }
        //        else
        //        {
        //            while (!fileStream.Completed)
        //            {
        //                var diff = fileStream.Length - fileStream.Position < 1024 * 512 ? 1024 * 512 : fileStream.Length - fileStream.Position;
        //                var data = await fileStream.GetData((int)diff);

        //                var fileData = new FileData(file, data);
        //                await stream.SendObject(fileData);
        //            }
        //        }
        //    }

        //    if (fileDataList.Count > 0)
        //    {
        //        foreach (var fileData in fileDataList)
        //        {
        //            await stream.SendObject(fileData);
        //        }

        //        fileDataList.Clear();
        //    }
        //}

        //public async static Task ProcessFileData(SyncShare share, FileData data)
        //{
        //    if (data.Data.Length == 0)
        //    {
        //        Log.Warning("Received data with 0 length. ID: {id} || Path: {path}", data.ID, data.Path);
        //        return;
        //    }  

        //    var local = share.PathList[data.ID];
        //    if (local == null)
        //    {
        //        Log.Warning("No path data found with id {id}", data.ID); 
        //        return;
        //    }

        //    var stream = Streams[local.ID];
        //    if (stream == null)
        //    {
        //        stream = new SyncStream(share.Path, local, FileMode.Create);
        //        await stream.WriteData(data.Data);

        //        Streams.Add(stream);
        //    }
        //}

        //public static void Dispose()
        //{
        //    foreach (var stream in Streams)
        //    {
        //        stream.Dispose();
        //    }

        //    Streams.Clear();
        //}
    }
}
