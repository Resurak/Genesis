using Genesis.Commons;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Sync
{
    public class SyncServer : BaseSync
    {
        TcpServer? Server;
        TcpClient? Client;
        NetStream? NetStream;

        public event UpdateEventHandler? Started;
        public event UpdateEventHandler? Stopped;

        public event UpdateEventHandler? ClientConnected;
        public event UpdateEventHandler? ClientDisconnected;

        public bool IsStarted => Server != null && Server.Active;
        public bool IsConnected => Client != null && NetStream != null && NetStream.CanStream;

        public void Start()
        {
            if (IsStarted)
            {
                throw new InvalidOperationException("Server already started");
            }

            Server = new TcpServer();
            Server.Start();

            Started?.Invoke();
        }

        public void Stop()
        {
            if (!IsStarted)
            {
                return;
            }

            Server.Stop();
            Server = null;

            Stopped?.Invoke();
        }

        public async Task WaitClient()
        {
            if (IsConnected)
            {
                throw new InvalidOperationException("Client already connected");
            }

            Client = await Server.AcceptTcpClientAsync();
            NetStream = new NetStream(Client);

            ClientConnected?.Invoke();
        }

        public async Task ReceiveRequests()
        {
            while (IsConnected)
            {
                var obj = await NetStream.ReceiveObject();
                if (obj == null)
                {
                    continue;
                }

                if (obj is Guid id)
                {
                    if (id == Tokens.GetShares)
                    {
                        await NetStream.SendObject(LocalShares);
                        continue;
                    }

                    if (id == Tokens.Disconnect)
                    {
                        Disconnect();
                        continue;
                    }
                }

                if (obj is FileData data)
                {
                    await SendFile(data);
                    continue;
                }

                Log.Warning("Received unknown object, skipping");
            }

            Disconnect();
        }

        async Task SendFile(FileData data)
        {
            var share = LocalShares[data.ShareID];
            if (share == null)
            {
                await SendRejected();
                return;
            }

            var file = share[data.FileID];
            if (file == null)
            {
                await SendRejected();
                return;
            }

            var path = file.AbsolutePath(share.Root);
            Log.Information("Sending {path}", path);

            try
            {
                await SendAccepted();
                await NetStream.SendFile(path);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Error while sending {path}", path);
            }
        }

        public void Disconnect()
        {
            if (Client == null && NetStream == null)
            {
                return;
            }

            NetStream?.Dispose();
            Client?.Close();

            NetStream = null;
            Client = null;

            ClientDisconnected?.Invoke();
        }

        async Task SendAccepted() =>
            await NetStream.SendObject(Tokens.Accepted);

        async Task SendRejected() =>
            await NetStream.SendObject(Tokens.Rejected);
    }
}
