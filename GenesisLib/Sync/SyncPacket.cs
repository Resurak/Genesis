using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib.Sync
{
    public class SyncPacket
    {
        public SyncPacket() 
        {

        }

        public SyncPacket(object? payload, SyncCommand command)
        {
            this.Payload = payload;
            this.Command = command;
        }

        public object? Payload { get; set; }
        public SyncCommand Command { get; set; }
    }
}
