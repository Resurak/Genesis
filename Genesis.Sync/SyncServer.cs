using Genesis.Networking;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Sync
{
    public class SyncServer : NetServer
    {
        public async Task StartService()
        {
            var status = Start();
            if (!status.OK)
            {
                Log.Warning(status.Exception, "Cannot start sync service. StatusCode: {code}", status.Code);
                return;
            }

            Log.Information("Service started. Waiting client");

            status = await WaitClient();
            if (!status.OK)
            {
                Log.Warning(status.Exception, "Cannot start sync service. StatusCode: {code}", status.Code);
                return;
            }

            Log.Information("Client connected");
            await Sync();
        }

        async Task Sync()
        {
            Log.Information("Starting automatic sync operations");

        }

        //async Task<Status> Handshake()
        //{

        //}
    }
}
