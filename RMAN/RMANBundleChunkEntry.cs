using System;

namespace RMAN_Parse.RMAN
{
    class RMANBundleChunkEntry
    {
        public string ParentBundleID { get; set; }
        public int OffsetToChunk { get; set; }
        public int OffsetinFile { get; set; }
        public int TableOffsetOffset { get; set; }
        public int CompressedSize { get; set; }
        public int UncompressedSize { get; set; }
        public string ChunkId { get; set; }
        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj != null) || this.GetType().Equals(obj.GetType()))
            {
                RMANBundleChunkEntry chunk = (RMANBundleChunkEntry)obj;

                return chunk.ChunkId == ChunkId
                       && chunk.CompressedSize == CompressedSize
                       && chunk.OffsetinFile == OffsetinFile
                       && chunk.ParentBundleID == ParentBundleID
                       && chunk.OffsetinFile == OffsetinFile
                       && chunk.OffsetToChunk == OffsetToChunk;
            }
            else
            {
                return false;
            }
        }
        public override int GetHashCode()
        {
            return Convert.ToInt32(ChunkId, 16) ^ Convert.ToInt32(ParentBundleID, 16);
        }
    }
}
