using Genesis.Commons;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

Log.Information("1 for server, 2 for client");

var sel = Console.ReadLine();
if (sel == "1")
{
    //var server = new SyncServer();
    //Log.Information("Creating storage and shares");

    //await server.CreateStorage("test1", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "root"));
    //server.CreateShare(server.StorageList[0], "test1");

    //await server.CreateStorage("test1", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "test"));
    //server.CreateShare(server.StorageList[1], "test2");

    //Log.Information("Starting server and waiting client...");
    //await server.StartServer();

    //while (server.Connected)
    //{
    //    Log.Information("Waiting new request");
    //    await server.ProcessRequest();
    //}

    //Log.Information("Client no more connected, closing server");
    //server.Stop();
}
else if (sel == "2")
{
    //var client = new SyncClient();

    //Log.Information("Connecting to local server...");
    //await client.Connect(); // test

    //if (!client.Connected)
    //{
    //    Log.Warning("Something occured while connecting to server...");
    //    Console.ReadLine();

    //    Environment.Exit(0);
    //}

    //Log.Information("Connected to server successfully, getting share list 3 times and then closing connection");

    //for (int i = 0; i < 3; i++)
    //{
    //    var shares = await client.GetShares();
    //    Log.Information("Shares:\n{@share}", shares);
    //}

    //await client.Disconnect();
    //Log.Information("Disconnected");

}

Console.ReadLine();

public class Test
{
    public Test()
    {
        this.Name = "Default";
        this.Description = "Default";
    }

    public Test(string name, string description)
    {
        this.Name = name;
        this.Description = description;
    }

    public string Name { get; set; }
    public string Description { get; set; }
}