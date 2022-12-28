using Genesis.Commons;
using Genesis.Sync;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

Log.Information("1 for server, 2 for client");

var sel = Console.ReadLine();
if (sel == "1")
{
    var server = new SyncServer();

    server.SocketError += Server_SocketError;
    server.ServerStarted += Server_ServerStarted;
    server.ServerStopped += Server_ServerStopped;
    server.ClientConnected += Server_ClientConnected;
    server.ClientDisconnected += Server_ClientDisconnected;

    await server.CreateStorageAndShare(@"C:\Users\chiar\Desktop\root");
    await server.CreateStorageAndShare(@"C:\Users\chiar\Desktop\UnityModManager");

    await server.StartService();
    await server.AcceptRequests();

    server.StopService();
    Console.ReadLine();
}
else if (sel == "2")
{
    var client = new SyncClient();

    client.SocketError += Server_SocketError;
    client.ClientConnected += Server_ClientConnected;
    client.ClientDisconnected += Server_ClientDisconnected;

    await client.ConnectLan();

    var shares = await client.GetShares();
    //Log.Information("Shares: {@shares}", shares);

    var storages = await client.GetStorage();
    //Log.Information("Storage: {@storage}", storages);

    await client.Disconnect();
    Console.ReadLine();
}

void Server_ServerStopped(object? obj = null)
{
    Log.Information("Server stopped");
}

void Server_ServerStarted(object? obj = null)
{
    Log.Information("Server started and waiting");
}

void Server_SocketError(object? obj = null)
{
    var ex = obj as Exception;
    Log.Warning(ex, "Error thrown");
}

void Server_ClientDisconnected(object? obj = null)
{
    Log.Information("Client disconnected");
}

void Server_ClientConnected(object? obj = null)
{
    Log.Information("Client connected");
}

Console.ReadLine();

