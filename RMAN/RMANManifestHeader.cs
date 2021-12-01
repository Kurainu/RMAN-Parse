namespace RMAN_Parse.RMAN
{
    public class RMANManifestHeader
    {
        public int OffsetTableOffset { get; set; }
        public int BundleListOffset { get; set; }
        public int LanguageListOffset { get; set; }
        public int FileListOffset { get; set; }
        public int FolderListOffset { get; set; }
        public int KeyHeaderOffset { get; set; }
        public int UnknownOffset { get; set; }
    }
}
