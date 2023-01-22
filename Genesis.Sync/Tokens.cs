using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Sync
{
    public static class Tokens
    {
        public static Guid Accepted => Guid.Parse("8d5ca768-bfa0-4d8b-bbed-89ba5d37b5aa");
        public static Guid Rejected => Guid.Parse("6f98f18d-000b-43dd-9f07-e92d503645a3");

        public static Guid GetShares => Guid.Parse("52f01291-1e51-4dac-871a-f2cf10b08ca6");
        public static Guid Disconnect => Guid.Parse("6054c0e4-a59b-4f6c-8921-74cfe18009a8");
    }
}
