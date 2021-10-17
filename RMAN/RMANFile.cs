namespace RMAN_Parse.RMAN
{
    class RMANFile
    {
        public RMANFileHeader FileHeader { get; set; }
        public RMANManifest Manifest { get; set; }
    }
}
