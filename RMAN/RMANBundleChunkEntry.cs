namespace RMAN_Parse.RMAN
{
    class RMANBundleChunkEntry
    {
        public string ParentBundleID { get; set; }
        public int OffsetToChunk { get; set; }
        public int TableOffsetOffset { get; set; }
        public int CompressedSize { get; set; }
        public int UncompressedSize { get; set; }
        public string ChunkId { get; set; }
    }
}
