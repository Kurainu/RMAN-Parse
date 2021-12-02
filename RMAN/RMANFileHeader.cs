namespace RMAN_Parse.RMAN
{
    public class RMANFileHeader
    {
        public string Magic { get; set; }
        public byte Major { get; set; }
        public byte Minor { get; set; }
        public byte Unknown { get; set; }
        public byte SignatureType { get; set; }
        public int Offset { get; set; }
        public int Length { get; set; }
        public long Manifestid { get; set; }
        public int DecompressedLength { get; set; }
    }
}
