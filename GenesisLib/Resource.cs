using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib
{
    public class Resource : IToken
    {
        public Resource()
        {
            this.ID = Guid.Empty;
            this.Type = ResourceType.Empty;
        }

        public Resource(Guid id, ResourceType type)
        {
            this.ID = id;
            this.Type = type;
        }

        public Resource(Guid iD, ResourceType type, byte[] data, params string[] headers) : this(iD, type)
        {
            this.Data = data;
            this.Headers = headers;
        }   

        public Guid ID { get; set; }
        public ResourceType Type { get; set; }

        public byte[]? Data { get; set; }
        public string[]? Headers { get; set; }

        public bool IsEmpty => Type == ResourceType.Empty;
        public bool HasData => Data != null;
        public bool HasHeaders => Headers != null && Headers.Length > 0;
    }

    public enum ResourceType
    {
        Empty,
        Custom,

        Message,
        Command,

        FileData,
        FileMetadata,

        ShareData,
        ShareMetadata
    }
}
