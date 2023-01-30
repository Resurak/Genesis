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
        var client = new SyncClient();

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
                if (!Directory.Exists(path))
                {
                    Log.Warning("Can't add share of {path}", path); 
                    continue;
                }

                await client.AddShare(path);
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
            }
        }

        //Log.Information("Starting client");

        //var client = new SyncClient();
        //await client.LoadClientShares();

        //while (true)
        //{
        //    var command = Console.ReadLine();
        //    if (command?.StartsWith("connect ") ?? false)
        //    {
        //        var split = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        //        if (split.Length == 2)
        //        {
        //            await client.Connect(split[1]);
        //            Log.Information("Connected to server");
        //        }
        //        else
        //        {
        //            Log.Warning("Invalid connect command");
        //        }

        //        continue;
        //    }

        //    if (command?.Equals("getList") ?? false)
        //    {
        //        await client.GetShareList();
        //    }

        //    if (command?.Equals("list") ?? false)
        //    {
        //        if (!client.ClientConnected)
        //        {
        //            Log.Warning("Not connected, skipping");
        //            continue;
        //        }

        //        Log.Information("LocalShares:");
        //        for (int i = 0; i < client.LocalShares.Count; i++)
        //        {
        //            Log.Information("\t[{i}]: {root}", i, client.LocalShares[i].Root);
        //        }

        //        Log.Information("RemoteShares:");
        //        for (int i = 0; i < client.RemoteShares.Count; i++)
        //        {
        //            Log.Information("\t[{i}]: {root}", i, client.RemoteShares[i].Root);
        //        }

        //        continue;
        //    }

        //    if (command?.StartsWith("sync ") ?? false)
        //    {
        //        if (!client.ClientConnected)
        //        {
        //            Log.Warning("Not connected, skipping");
        //            continue;
        //        }

        //        var split = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        //        if (split.Length == 3)
        //        {
        //            Share? local = null;
        //            Share? source = null;

        //            var localNum = -1;
        //            var remoteNum = -1;

        //            if (int.TryParse(split[1], out localNum) && localNum >= 0 && localNum < client.LocalShares.Count)
        //            {
        //                local = client.LocalShares[localNum];
        //            }
        //            else
        //            {
        //                Log.Warning("Invalid local share number, skipping");
        //                continue;
        //            }

        //            if (int.TryParse(split[2], out remoteNum) && remoteNum >= 0 && remoteNum < client.RemoteShares.Count)
        //            {
        //                source = client.RemoteShares[remoteNum];
        //            }
        //            else
        //            {
        //                Log.Warning("Invalid remote share number, skipping");
        //                continue;
        //            }

        //            await client.ShareSync(local, source);
        //            continue;
        //        }
        //    }

        //    if (command?.StartsWith("add ") ?? false)
        //    {
        //        var folder = command.Substring("add ".Length).Replace("\"", "");
        //        if (Directory.Exists(folder))
        //        {
        //            Log.Information("test");
        //            await client.AddShare(folder);
        //        }
        //        else
        //        {
        //            Log.Warning("Can't add {root} to local share because folder doesn't exist", folder);
        //        }

        //        continue;
        //    }

        //    if (command?.Equals("disconnect") ?? false)
        //    {
        //        if (!client.ClientConnected)
        //        {
        //            Log.Warning("Not connected, skipping");
        //        }
        //        else
        //        {
        //            Log.Warning("Disconnecting client");
        //            await client.Disconnect();
        //        }

        //        continue;
        //    }

        //    Log.Warning("Invalid command");
        //}
    }

    if (args[0] == "server")
    {

        //Log.Information("Starting server");

        //var server = new SyncServer();
        //server.Start();

        //await server.LoadServerShares();
        //while (server.Active)
        //{
        //    var command = Console.ReadLine();
        //    if (command?.Equals("accept") ?? false)
        //    {
        //        await server.WaitClient();
        //        Log.Information("Client connected");

        //        while (server.ClientConnected)
        //        {
        //            await server.ReceiveRequest();
        //        }

        //        continue;
        //    }

        //    if (command?.Equals("getList") ?? false)
        //    {
        //        //await server.GetShareList();
        //        continue;
        //    }

        //    if (command?.Equals("list") ?? false)
        //    {
        //        if (!server.ClientConnected)
        //        {
        //            Log.Warning("Not connected, skipping");
        //            continue;
        //        }

        //        Log.Information("LocalShares:");
        //        for (int i = 0; i < server.LocalShares.Count; i++)
        //        {
        //            Log.Information("\t[{i}]: {root}", i, server.LocalShares[i].Root);
        //        }

        //        Log.Information("RemoteShares:");
        //        for (int i = 0; i < server.RemoteShares.Count; i++)
        //        {
        //            Log.Information("\t[{i}]: {root}", i, server.RemoteShares[i].Root);
        //        }

        //        continue;
        //    }

        //    if (command?.StartsWith("add ") ?? false)
        //    {
        //        var folder = command.Substring("add ".Length).Replace("\"", "");
        //        if (Directory.Exists(folder))
        //        {
        //            Log.Information("test");
        //            await server.AddShare(folder, false);
        //        }
        //        else
        //        {
        //            Log.Warning("Can't add {root} to local share because folder doesn't exist", folder);
        //        }
        //    }

        //    if (command?.Equals("disconnect") ?? false)
        //    {
        //        Log.Warning("Disconnecting client");
        //        await server.Disconnect();

        //        continue;
        //    }

        //    Log.Warning("Invalid command");
        //}
    }
}
else
{
    Log.Warning("No mode selected, returning");

    Console.Read();
    return;
}
