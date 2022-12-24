using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Networking
{
    public class NetClient : NetBase
    {
        public async Task<Status> Connect(string address, int port = 6969)
        {
            try
            {
                if (base.Connected)
                {
                    Disconnect();
                }

                var ip = IPAddress.Parse(address);
                var endPoint = new IPEndPoint(ip, port);

                base.client = new TcpClient();
                await base.client.ConnectAsync(endPoint);

                base.netStream = base.client.GetStream();
                return Status.Success;
            }
            catch (Exception ex) 
            {
                return new Status(StatusCode.ConnectionError, ex);
            }
        }
    }
}
