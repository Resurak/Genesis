using Genesis.Commons;
using Genesis.Net;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Gensis.Sync
{
    public class SyncClient : NetClient
    {
        public async Task Connect()
        {
            await base.Connect("127.0.0.1"); // test
            await Handshake();
        }

        public List<InfoShare> ShareList { get; set; } = new List<InfoShare>();

        public async Task<List<InfoShare>?> GetShares()
        {
            await SendRequest(RequestCode.GetAvailableShares);
            if (await GetResponse() is Response response)
            {
                if (response.Code != ResponseCode.Accepted)
                {
                    Log.Warning("Response got: {code}", response.Code);
                }
                else
                {
                    var list = response.DeserializeParam<List<InfoShare>>();
                    if (list != null)
                    {
                        ShareList = list;
                        return list;
                    }
                }
            }
            else
            {
                Log.Warning("Failed to receive response...");
            }

            return null;
        }

        public async Task SyncShare(InfoShare share, string root)
        {
            await SendRequest(RequestCode.SyncShare, share.StorageID);
            if (await GetResponse() is Response response)
            {
                if (response.Code== ResponseCode.Accepted)
                {
                    Log.Information("Sync request accepted");

                    var storage = response.DeserializeParam<VirtualStorage>();
                    if (storage == null)
                    {
                        Log.Warning("Received invalid storage info, can't sync");
                        return;
                    }

                    Log.Information("Creating local storage");
                    var localStorage = await VirtualStorage.Create("temp", root);

                    Log.Information("Comparing files to sync");
                    foreach (var file in storage.Files)
                    {
                        var temp = localStorage.Files.FirstOrDefault(x => x.Path == file.Path);
                        if (temp == null)
                        {
                            await SendRequest(RequestCode.FileSync, file.ID);
                            if (await GetResponse() is Response fileResponse && fileResponse.Code == ResponseCode.Accepted)
                            {
                                Log.Information("Syncing {file}", file.Path);
                                await ReceiveFile(System.IO.Path.Combine(root, file.Path), file.Size);
                            }
                            else
                            {
                                Log.Warning("Could not sync file {file}", file.Path);
                            }
                        }
                    }

                    /*
                        TO CONTINUE
                    */

                    foreach (var file in localStorage.Files)
                    {
                        var temp = storage.Files.FirstOrDefault(x => x.Path == file.Path);
                        if (temp == null)
                        {

                        }
                    }
                }
            }
        }



        async Task SendRequest(RequestCode code, Guid token = default)
        {
            if (!Connected)
            {
                Log.Warning("Can't send request, client not connected");
                return;
            }

            try
            {
                await SendObject(new Request(code, token));
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Exception thrown while sending request");
            }
        }

        async Task<Response?> GetResponse()
        {
            if (!Connected)
            {
                Log.Warning("Can't send request, client not connected");
                return null;
            }

            try
            {
                return await ReceiveObject<Response>();
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Exception thrown while getting response");
                return null;
            }
        }

        async Task Handshake()
        {
            await SendRequest(RequestCode.Connection);
            if (await GetResponse() is Response response && response.Code == ResponseCode.Accepted)
            {
                return;
            }
            else
            {
                Log.Warning("Error while doing handshake with server, closing connection");
                base.Disconnect();
            }
        }

        public new async Task Disconnect()
        {
            await SendRequest(RequestCode.Disconnection);
            if (await GetResponse() is Response response && response.Code == ResponseCode.Accepted)
            {
                base.Disconnect();
            }
            else
            {
                Log.Warning("Error while disconnecting... forcing disconnection");
                base.Disconnect();
            }
        }
    }
}
