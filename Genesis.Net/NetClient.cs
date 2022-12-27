using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Net
{
    public class NetClient : NetBase
    {
        public async Task Connect(string address, int port = 6969)
        {
            try
            {
                base.Client = new TcpClient();
                var ip = IPAddress.Parse(address);
                var endPoint = new IPEndPoint(ip, port);

                await base.Client.ConnectAsync(endPoint);
                base.Stream = base.Client.GetStream();
            }
            catch
            {
                throw new ConnectionException();
            }
        }
    }
}
