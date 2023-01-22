using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Sync
{
    public class FileData
    {
        public FileData(Guid fileID, Guid shareID)
        {
            this.FileID = fileID;
            this.ShareID = shareID;
        }

        public Guid FileID { get; set; }
        public Guid ShareID { get; set; }
    }
}
