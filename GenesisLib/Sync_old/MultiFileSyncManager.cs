﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib.Sync_old
{
    public class MultiFileSyncManager
    {
        public MultiFileSyncManager() 
        {
            InboundFileSyncList = new ItemList<FileSync>();
        }

        public ItemList<FileSync> InboundFileSyncList { get; set; }
        public ItemList<FileSync> OutboundFileSyncList { get; set; }

        public async Task HandleInboundBlocks(params FileData[] blocks)
        {
            var tasks = new List<Task>();
            foreach (var block in blocks)
            {
                tasks.Add(Task.Run(() => SyncIncomingBlock(block)));
            }

            await Task.WhenAll(tasks);
            CheckInboundList();
        }

        public async Task HandleOutboundBlocks(string root, PathData data)
        {
            var path = Path.Combine(root, data.Name);
            var blocks = new List<FileData>();

            //foreach (var )
        }

        async Task SyncIncomingBlock(FileData data)
        {
            var temp = InboundFileSyncList[data.ID];
            if (temp == null)
            {
                var fileSync = new FileSync(data.ID, data.Path, FileSyncMode.Destination, data.Size);
                InboundFileSyncList.Add(fileSync);

                await fileSync.WriteBlock(data.Data);
            }
            else
            {
                await temp.WriteBlock(data.Data);
            }
        } 

        void CheckInboundList()
        {
            var removalList = InboundFileSyncList.Where(x => x.Completed);
            foreach (var item in removalList)
            {
                item.Dispose();
                InboundFileSyncList.Remove(item);
            }
        }

        void CheckOutboundList()
        {
            var removalList = OutboundFileSyncList.Where(x => x.Completed);
            foreach (var item in removalList)
            {
                item.Dispose();
                OutboundFileSyncList.Remove(item);
            }
        }
    }
}
