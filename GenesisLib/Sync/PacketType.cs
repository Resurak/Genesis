using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib.Sync
{
    public enum PacketType
    {
        Handshake,
        Disconnect,

        Upload_ShareList,
        Download_ShareList,

        FileData,
        FileDataList,

        ShareSync,

        ShareSync_Accepted,
        ShareSync_Rejected,

        ShareSync_Completed,
    }
}
