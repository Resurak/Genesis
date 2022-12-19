using Genesis.Commons;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gensis.Sync
{
    public class Response
    {
        public Response()
        {

        }

        public Response(ResponseCode code)
        {
            this.Code = code;
        }

        public Response(ResponseCode code, byte[] param)
        {
            this.Code = code;
            this.Param = param;
        } 

        public Guid Token { get; set; }
        public ResponseCode Code { get; set; }

        public byte[]? Param { get; set; }

        public T? DeserializeParam<T>() where T : class
        {
            try
            {
                return MessagePackUtils.Deserialize<T>(Param);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Exception thrown while deserializing response parameter");
                return null;
            }
        }
    }

    public enum ResponseCode
    {
        Unavailable,

        Accepted,
        Rejected,

        Error_BadToken,
        Error_InvalidToken,

        Error_NotEncrypted,
        Error_NotAuthenticated,
    }
}
