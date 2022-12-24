using Genesis.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Networking
{
    public class NetServer : NetBase
    {
        protected TcpServer Server;

        public bool Listening => Server != null && Server.Active;

        public Status Start(int port = 6969)
        {
            if (Listening)
            {
                return new Status(StatusCode.InvalidOperation);
            }

            try
            {
                Server = new TcpServer(port);
                Server.Start();

                return Status.Success;
            }
            catch (Exception ex)
            {
                return new Status(StatusCode.ConnectionError, ex);
            }
        }

        public async Task<Status> WaitClient()
        {
            if (!Listening)
            {
                return new Status(StatusCode.InvalidOperation);
            }

            try
            {
                base.client = await Server.AcceptTcpClientAsync();
                base.netStream = base.client.GetStream();

                return Status.Success;
            }
            catch (Exception ex)
            {
                return new Status(StatusCode.ConnectionError, ex);
            }
        }

        public void Stop()
        {
            if (!Listening)
            {
                return;
            }

            Disconnect();
            Server.Stop();
        }
    }

}