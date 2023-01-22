using Genesis.Commons;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Sync
{
    public delegate void ShareEventHandler(ShareData data);
    public delegate void SyncProgressEventHandler(SyncProgress progress);

    public class BaseSync
    {
        public event ShareEventHandler? ShareCreated;
        public event ShareEventHandler? ShareDeleted;

        public event SyncProgressEventHandler? SyncProgress;

        public ShareList LocalShares { get; set; }

        public BaseSync() 
        {
            LocalShares = new ShareList();
        }

        public async Task CreateShare(string root) => await CreateShare("Placeholder", root);

        public async Task CreateShare(string name, string root)
        {
            var share = new ShareData(root);
            await share.Update();

            LocalShares.Add(share);
        }

        public void DeleteShare(Guid id) 
        {
            var share = LocalShares[id];
            if (share != null)
            {
                LocalShares.Remove(share);
            }
        }

        protected void OnProgress(SyncProgress progress)
        {
            SyncProgress?.Invoke(progress);
        }
    }
}
