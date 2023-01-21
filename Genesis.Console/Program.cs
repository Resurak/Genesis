using Genesis.Commons;
using Serilog;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

Log.Information("Creating pseudo remote share");
var remoteShare = new Share(@"C:\Users\chiar\Desktop\test1");
await remoteShare.Update();
Log.Information("{@share}", remoteShare);

Log.Information("Creating pseudo local share");
var localShare = new Share(@"C:\Users\chiar\Desktop\test2");
await localShare.Update();
Log.Information("{@share}", localShare);

Log.Information("Comparing shares");
foreach (var file in localShare.EnumerateDifferentFiles(remoteShare))
{
    Log.Information("File that need sync: {@file}", file);
}

Log.Information("finish");
Console.ReadLine();
return;

Log.Information("1 for server, 2 for client");

var sel = Console.ReadLine();
if (sel == "1")
{
    var server = new TcpServer();
    server.Start();

    var client = await server.AcceptTcpClientAsync();
    var stream = client.GetStream();

    try
    {
        while (true)
        {
            var sizeBuffer = new byte[sizeof(int)];
            await stream.ReadExactlyAsync(sizeBuffer);

            var size = BitConverter.ToInt32(sizeBuffer);
            var dataBuffer = new byte[size];

            await stream.ReadExactlyAsync(dataBuffer);
            Log.Information("Received: {data}" + Encoding.UTF8.GetString(dataBuffer));
        }
    }
    catch (EndOfStreamException) 
    {
        Log.Warning("End of stream detected");
    }
    catch (Exception ex)
    {
        Log.Warning(ex, "Exception thrown");
    }
}
else if (sel == "2")
{
    var client = new TcpClient();
    await client.ConnectAsync("127.0.0.1", 6969);

    var stream = client.GetStream();
    try
    {
        for (int i = 0; i < 10; i++)
        {
            var rand = new Random();
            var message = Encoding.UTF8.GetBytes("Random number: " + rand.Next(1, 10000));

            var size = BitConverter.GetBytes(message.Length);
            await stream.WriteAsync(size);
            await stream.WriteAsync(message);
        }

        await stream.WriteAsync(new byte[3]);
        stream.Dispose();
        client.Dispose();
    }
    catch (EndOfStreamException)
    {
        Log.Warning("End of stream detected");
    }
    catch (Exception ex)
    {
        Log.Warning(ex, "Exception thrown");
    }
}
Log.Information("Finished");
Console.ReadLine();

Console.ReadLine();

