using Genesis.Commons;
using Genesis.Sync;
using Serilog;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

if (args.Length > 0)
{
    if (args[0] == "client")
    {
        Log.Information("Starting client, enter ip address");

        var ip = Console.ReadLine();
        var client = new SyncClient();

        var lines = File.ReadAllLines("localShares.txt");
        if (lines.Length == 0)
        {
            Log.Information("No local shares, closing...");

            Console.ReadLine();
            return;
        }

        foreach (var line in lines)
        {
            await client.CreateShare(line);
        }

        var local = @"C:\Users\danie\Desktop\share test";
        var syncProgress = new SyncProgress(-1);

        var showTimer = false;
        var timer = new Timer(ShowProgress, null, Timeout.Infinite, 500);


        await client.ConnectAsync(ip);
        await client.GetShareList();

        client.SyncProgress += ProgressCallback;

        while (client.IsConnected)
        {
            Log.Information("Select local share to sync:");
            for (int i = 0; i < client.LocalShares.Count; i++)
            {
                var share = client.LocalShares[i];
                Log.Information("[{i}]:\t\t{name} | {path} | {id}", i, share.Name, share.Root, share.ID);
            }

            var selection = Console.ReadLine();
            var num = int.Parse(selection);

            if (num < 0 || num > client.LocalShares.Count)
            {
                Log.Warning("Invalid number entered...");

                await client.Disconnect();
                return;
            }

            var localShare = client.LocalShares[num];
            await localShare.Update();

            Log.Information("Select server share to sync:");
            for (int i = 0; i < client.ServerShares.Count; i++)
            {
                var share = client.ServerShares[i];
                Log.Information("[{i}]:\t\t{name} | {path} | {id}", i, share.Name, share.Root, share.ID);
            }

            selection = Console.ReadLine();
            num = int.Parse(selection);

            if (num < 0 || num > client.ServerShares.Count)
            {
                Log.Warning("Invalid number entered...");

                await client.Disconnect();
                return;
            }

            var serverShare = client.ServerShares[num];

            Log.Information("Syncing local share {path1} with server share {path2}", localShare.Root, serverShare.Root);
            timer.Change(0, 500);

            await client.SyncShare(localShare, serverShare/*, new Progress<PathData>(x => Log.Information("Synced {path}", x.Path))*/);

            timer.Change(0, 500);
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            Log.Information("Local folder synced\n------------------------------------------------------------");
        }

        await client.Disconnect();

        Console.ReadLine();
        return;

        void ShowProgress(object? state = null)
        {
            Log.Information("Percent completed => {percent}", syncProgress.SyncPercent);
        }

        void ProgressCallback(SyncProgress progress)
        {
            syncProgress = progress;
        }
    }

    if (args[0] == "server")
    {
        Log.Information("Starting server");

        var local1 = @"C:\Users\danie\Desktop\root";
        var local2 = @"C:\Users\danie\Desktop\wallpaper";
        var local3 = @"C:\Users\danie\Desktop\VanillaExpandedFramework-061022";

        var server = new SyncServer();
        server.Start();

        var lines = File.ReadAllLines("serverShares.txt");
        if (lines.Length == 0)
        {
            Log.Information("No local shares, closing...");

            Console.ReadLine();
            return;
        }

        foreach (var line in lines)
        {
            await server.CreateShare(line);
        }

        //await server.CreateShare(local1);
        //await server.CreateShare(local2);
        //await server.CreateShare(local3);

        await server.WaitClient();
        await server.ReceiveRequests();

        Console.ReadLine();
        return;
    }
}
else
{
    Log.Warning("No mode selected, returning");

    Console.Read();
    return;
}

//var sw = Stopwatch.StartNew();

//Log.Information("Creating pseudo remote share");
//var remoteShare = new ShareData(@"C:\Users\danie\Desktop\Giochi\server minecraft");
//await remoteShare.Update();

//Log.Information("Creating pseudo local share");
//var localShare = new ShareData(@"C:\Users\danie\Desktop\share test\");
//await localShare.Update();

//Log.Information("Syncing shares");

//localShare.PrepareShare(remoteShare);
//await localShare.SyncLocal(remoteShare);

//sw.Stop();
//Log.Information("finished in {sw}", sw.ElapsedMilliseconds);
//Console.ReadLine();

//return;

//Log.Information("1 for server, 2 for client");

//var sel = Console.ReadLine();
//if (sel == "1")
//{
//    var server = new TcpServer();
//    server.Start();

//    var client = await server.AcceptTcpClientAsync();
//    var stream = client.GetStream();

//    try
//    {
//        while (true)
//        {
//            var sizeBuffer = new byte[sizeof(int)];
//            await stream.ReadExactlyAsync(sizeBuffer);

//            var size = BitConverter.ToInt32(sizeBuffer);
//            var dataBuffer = new byte[size];

//            await stream.ReadExactlyAsync(dataBuffer);
//            Log.Information("Received: {data}" + Encoding.UTF8.GetString(dataBuffer));
//        }
//    }
//    catch (EndOfStreamException) 
//    {
//        Log.Warning("End of stream detected");
//    }
//    catch (Exception ex)
//    {
//        Log.Warning(ex, "Exception thrown");
//    }
//}
//else if (sel == "2")
//{
//    var client = new TcpClient();
//    await client.ConnectAsync("127.0.0.1", 6969);

//    var stream = client.GetStream();
//    try
//    {
//        for (int i = 0; i < 10; i++)
//        {
//            var rand = new Random();
//            var message = Encoding.UTF8.GetBytes("Random number: " + rand.Next(1, 10000));

//            var size = BitConverter.GetBytes(message.Length);
//            await stream.WriteAsync(size);
//            await stream.WriteAsync(message);
//        }

//        await stream.WriteAsync(new byte[3]);
//        stream.Dispose();
//        client.Dispose();
//    }
//    catch (EndOfStreamException)
//    {
//        Log.Warning("End of stream detected");
//    }
//    catch (Exception ex)
//    {
//        Log.Warning(ex, "Exception thrown");
//    }
//}
//Log.Information("Finished");
//Console.ReadLine();

//Console.ReadLine();

