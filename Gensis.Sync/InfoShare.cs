using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gensis.Sync
{
    public class InfoShare
    {
        public InfoShare()
        {

        }

        public InfoShare(VirtualStorage storage, string name, bool includeRoot = true)
        {
            this.ShareID = Guid.NewGuid();
            this.StorageID = storage.ID;
            this.Name = name;
            this.RootFolder = includeRoot ? storage.RootFolder : String.Empty;
        }

        public Guid ShareID { get; set; }
        public Guid StorageID { get; set; }
        public bool RequireAuth { get; set; }

        public string Name { get; set; }
        public string RootFolder { get; set; } 
    }
}
