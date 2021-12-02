using System.Collections.Generic;

namespace RMAN_Parse.RMAN
{
    public class RMANBundleEntry
    {
        public int Offset { get; set; }
        public int TableoffsetOffset { get; set; }
        public int HeaderSize { get; set; }
        public string BundleId { get; set; }
        public List<RMANBundleChunkEntry> Chunks { get; set; }
    }
}
