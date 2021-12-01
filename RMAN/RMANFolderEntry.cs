using System;

namespace RMAN_Parse.RMAN
{
    public class RMANFolderEntry
    {
        public int Offset { get; set; }
        public int TableoffsetOffset { get; set; }
        public short FolderIdOffset { get; set; }
        public short ParentIdOffset { get; set; }
        public int NameOffset { get; set; }
        public String Name { get; set; }
        public long FolderId { get; set; }
        public long ParentId { get; set; }
    }
}
