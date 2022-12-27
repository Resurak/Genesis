using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Commons
{
    public class DataShare
    {
        public Guid ID { get; set; } 
        public string? Name { get; set; }

        public Guid StorageID { get; set; }
        public string? StorageRoot { get; set; }
    }
}
