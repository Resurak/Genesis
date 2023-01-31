using GenesisLibrary.Sync;
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
        SyncProgress? progress = null;
        var timer = new Timer(TimerCallback, null, 0, 1000);

        var client = new SyncClient();
        client.SyncProgress += UpdateProgress;

        while (true)
        {
            var selection = Console.ReadLine();
            if (selection == "connect")
            {
                await client.Connect("127.0.0.1");
                continue;
            }

            if (selection.StartsWith("add "))
            {
                var path = selection.Substring("add ".Length);
                path = path.Replace("\"", "");
                //if (!path.Contains(" "))
                //{

                //}

                if (!Directory.Exists(path))
                {
                    Log.Warning("Can't add share of {path}", path); 
                    continue;
                }

                await client.AddShare(path);
                continue;
            }

            if (selection == "getList")
            {
                await client.RequestShareList();
                continue;
            }

            if (selection == "list")
            {
                if (client.LocalShareList.Count > 0)
                {
                    Log.Information("{x}", nameof(client.LocalShareList));
                    for (int i = 0; i < client.LocalShareList.Count; i++)
                    {
                        Log.Information("[{i}]: {path}", i, client.LocalShareList[i].Path);
                    }
                }

                if (client.LocalShareList.Count > 0)
                {
                    Log.Information("{x}", nameof(client.RemoteShareList));
                    for (int i = 0; i < client.RemoteShareList.Count; i++)
                    {
                        Log.Information("[{i}]: {path}", i, client.RemoteShareList[i].Path);
                    }
                }

                continue;
            }

            if (selection.StartsWith("sync "))
            {
                var split = selection.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                var num1 = 0;
                var num2 = 0;

                if (split.Length != 3)
                {
                    Log.Warning("Invalid command");
                    continue;
                }

                if (!int.TryParse(split[1], out num1) || num1 < 0 && num1 >= client.LocalShareList.Count)
                {
                    Log.Warning("Wrong local share number entered");
                    continue;
                }

                if (!int.TryParse(split[2], out num2) || num2 < 0 && num2 >= client.RemoteShareList.Count)
                {
                    Log.Warning("Wrong local share number entered");
                    continue;
                }

                await client.RequestSync(client.LocalShareList[num1], client.RemoteShareList[num2]);
                continue;
            }

            Log.Warning("Unknown command");
        }

        void UpdateProgress(SyncProgress? syncProgress)
        {
            progress = syncProgress;
        }

        void TimerCallback(object? state = null)
        {
            if (progress == null)
            {
                return;
            }

            Log.Information("Sync progress: {num}%", progress.Percent);
            if (progress.Percent == 100)
            {
                progress = null;
            }
        }
    }

    if (args[0] == "server")
    {
        var server = new SyncServer();
        server.Start();

        while (true)
        {
            var selection = Console.ReadLine();
            if (selection == "connect")
            {
                Log.Information("Waiting new client");
                await server.WaitClient();
            }

            if (selection == "getList")
            {
                await server.RequestShareList();
                continue;
            }

            if (selection.StartsWith("add "))
            {
                var path = selection.Substring("add ".Length);
                path = path.Replace("\"", "");

                if (!Directory.Exists(path))
                {
                    Log.Warning("Can't add share of {path}", path);
                    continue;
                }

                await server.AddShare(path);
                continue;
            }

            if (selection == "list")
            {
                if (server.LocalShareList.Count > 0)
                {
                    Log.Information("{x}", nameof(server.LocalShareList));
                    for (int i = 0; i < server.LocalShareList.Count; i++)
                    {
                        Log.Information("[{i}]: {path}", i, server.LocalShareList[i].Path);
                    }
                }

                if (server.LocalShareList.Count > 0)
                {
                    Log.Information("{x}", nameof(server.RemoteShareList));
                    for (int i = 0; i < server.RemoteShareList.Count; i++)
                    {
                        Log.Information("[{i}]: {path}", i, server.RemoteShareList[i].Path);
                    }
                }

                continue;
            }

            if (selection.StartsWith("sync "))
            {
                var split = selection.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                var num1 = 0;
                var num2 = 0;

                if (split.Length != 3)
                {
                    Log.Warning("Invalid command");
                    continue;
                }

                if (!int.TryParse(split[1], out num1) || num1 < 0 && num1 >= server.LocalShareList.Count)
                {
                    Log.Warning("Wrong local share number entered");
                    continue;
                }

                if (!int.TryParse(split[2], out num2) || num2 < 0 && num2 >= server.RemoteShareList.Count)
                {
                    Log.Warning("Wrong local share number entered");
                    continue;
                }

                await server.RequestSync(server.LocalShareList[num1], server.RemoteShareList[num2]);
            }
        }
    }
}
else
{
    Log.Warning("No mode selected, returning");

    Console.Read();
    return;
}
