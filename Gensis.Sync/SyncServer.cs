using Genesis.Commons;
using Genesis.Net;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gensis.Sync
{
    public class SyncServer : NetServer
    {
        public SyncServer()
        {
            ShareList = new List<InfoShare>();
            StorageList = new List<VirtualStorage>();
        }

        public List<InfoShare> ShareList { get; set; }
        public List<VirtualStorage> StorageList { get; set; }

        public new async Task StartServer()
        {
            await StartAndWaitClient();
            await Handshake();
        }

        public async Task ProcessRequest()
        {
            if (!Connected)
            {
                Log.Warning("Can't process request because client not connected");
                return;
            }

            try
            {
                if (await GetRequest() is Request request)
                {
                    Log.Information("Received new {code} request", request.Code);
                    switch (request.Code)
                    {
                        case RequestCode.Connection:
                            await SendResponse(ResponseCode.Accepted);
                            break;
                        case RequestCode.Disconnection:
                            await SendResponse(ResponseCode.Accepted);
                            Disconnect();
                            break;
                        case RequestCode.Authentication:
                            await SendResponse(ResponseCode.Unavailable);
                            break;
                        case RequestCode.EncryptConnection:
                            await SendResponse(ResponseCode.Unavailable);
                            break;
                        case RequestCode.GetAvailableShares:
                            await SendResponse(ResponseCode.Accepted, ShareList);
                            break;
                        case RequestCode.GetAvailableVirtualStorage:
                            await SendResponse(ResponseCode.Unavailable);
                            break;
                        case RequestCode.SyncShare:
                            await SendResponse(ResponseCode.Unavailable);
                            break;
                        default:
                            await SendResponse(ResponseCode.Unavailable);
                            break;
                    }
                }
                else
                {
                    Log.Debug("Received request was null, check logs");
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Exception thrown while processing request");
            }
        }

        public async Task CreateStorage(string name, string root)
        {
            if (StorageList.FirstOrDefault(x => x.RootFolder == root) != null)
            {
                Log.Warning("A virtual storage of {root} already exist");
                return;
            }

            var vs = await VirtualStorage.Create(name, root);
            StorageList.Add(vs);
        }

        public void CreateShare(VirtualStorage storage, string name, bool includeRoot = true)
        {
            if (ShareList.FirstOrDefault(x => x.StorageID == storage.ID) != null)
            {
                Log.Warning("A share of virtual storage [ ID : {id}, Name: {name} ] already exist", storage.ID, storage.Name);
                return;
            }

            var share = new InfoShare(storage, name, includeRoot);
            ShareList.Add(share);
        }

        async Task SyncShare(Guid token)
        {
            var storage = StorageList.FirstOrDefault(x => x.ID == token);
            if (storage == null)
            {
                Log.Warning("Requested storage is invalid, no such storage with id {id}", token);
                await SendResponse(ResponseCode.Error_BadToken);
            }
            else
            {
                Log.Information("Starting storage sync. ID: {id}", token);
                Log.Information("Sending all storage info");

                await SendResponse(ResponseCode.Accepted, storage);
            }
        }

        async Task Handshake()
        {
            if (await GetRequest() is Request request && request.Code == RequestCode.Connection)
            {
                await SendResponse(ResponseCode.Accepted);
            }
            else
            {
                await SendResponse(ResponseCode.Rejected);
            }
        }

        async Task<Request?> GetRequest()
        {
            if (!Connected)
            {
                Log.Warning("Can't receive request, client not connected");
                return null;
            }

            try
            {
                return await ReceiveObject<Request>();
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Exception thrown while receiving request");
                return null;
            }
        }

        async Task SendResponse(ResponseCode code, object? obj = null)
        {
            if (!Connected)
            {
                Log.Debug("Can't send response, client not connected");
                return;
            }

            try
            {
                if (obj == null)
                {
                    await SendObject(new Response(code));
                }
                else
                {
                    await SendObject(new Response(code, MsgPack.Serialize(obj)));
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Exception thrown while sending response");
            }
        }
    }
}
