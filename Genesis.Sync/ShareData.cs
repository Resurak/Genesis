using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Sync_Old
{
    public class ShareData
    {
        public ShareData(Guid id, bool token, bool encryption, string sharePath = "", string shareName = "")
        {
            this.StorageID = id;

            this.RequireToken = token;
            this.RequireEncryption = encryption;

            this.SharePath = sharePath;
            this.ShareName = shareName;
        }

        public Guid StorageID { get; set; }

        public bool RequireToken { get; set; }
        public bool RequireEncryption { get; set; }
        
        public string SharePath { get; set; }
        public string ShareName { get; set; }
    }
}
