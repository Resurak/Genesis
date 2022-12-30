using Genesis.Commons;
using Genesis.Sync;
using GenesisLib;
using Serilog;
using System.Diagnostics;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

using var process = Process.GetCurrentProcess();
using var fileStream = new FileStream(@"C:\Users\danie\Desktop\prova.test", FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 8);

var meta = new FileMetadata(@"C:\Users\danie\Desktop\chiara\cassaforte.7z", @"C:\Users\danie\Desktop\chiara\");
await foreach (var data in FileUtils.EnumerateFileData(meta, @"C:\Users\danie\Desktop\chiara\"))
{
    if (data.CurrentBlock % 10 == 0)
    {
        Log.Information("Iteration: {i} of {o} || Memory: {mem} MB", data.CurrentBlock, data.TotalBlocks, (process.PrivateMemorySize64 / (1024 * 1024)));
    }

    await fileStream.WriteAsync(data.DataBlock);
}

Log.Information("done");

Console.ReadLine();
Environment.Exit(0);

Log.Information("1 for server, 2 for client");

var sel = Console.ReadLine();
if (sel == "1")
{
    //var server = new SyncServer();

    //server.SocketError += Server_SocketError;
    //server.ServerStarted += Server_ServerStarted;
    //server.ServerStopped += Server_ServerStopped;
    //server.ClientConnected += Server_ClientConnected;
    //server.ClientDisconnected += Server_ClientDisconnected;

    //await server.CreateStorageAndShare(@"C:\Users\chiar\Desktop\root");
    //await server.CreateStorageAndShare(@"C:\Users\chiar\Desktop\UnityModManager");

    //await server.StartService();
    //await server.AcceptRequests();

    //server.StopService();
    //Console.ReadLine();
}
else if (sel == "2")
{
    //var client = new SyncClient();

    //client.SocketError += Server_SocketError;
    //client.ClientConnected += Server_ClientConnected;
    //client.ClientDisconnected += Server_ClientDisconnected;

    //await client.ConnectLan();

    //var shares = await client.GetShares();
    ////Log.Information("Shares: {@shares}", shares);

    //var storages = await client.GetStorage();
    ////Log.Information("Storage: {@storage}", storages);

    //await client.Disconnect();
    //Console.ReadLine();
}

//void Server_ServerStopped(object? obj = null)
//{
//    Log.Information("Server stopped");
//}

//void Server_ServerStarted(object? obj = null)
//{
//    Log.Information("Server started and waiting");
//}

//void Server_SocketError(object? obj = null)
//{
//    var ex = obj as Exception;
//    Log.Warning(ex, "Error thrown");
//}

//void Server_ClientDisconnected(object? obj = null)
//{
//    Log.Information("Client disconnected");
//}

//void Server_ClientConnected(object? obj = null)
//{
//    Log.Information("Client connected");
//}

Console.ReadLine();

