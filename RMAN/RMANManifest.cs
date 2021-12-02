using System.Collections.Generic;

namespace RMAN_Parse.RMAN
{
    public class RMANManifest
    {
        public RMANManifestHeader ManifestHeader { get; set; }
        public List<RMANBundleEntry> Bundles { get; set; }
        public List<RMANLanguageEntry> Languages { get; set; }
        public List<RMANFileEntry> Files { get; set; }
        public List<RMANFolderEntry> Folders { get; set; }
    }
}
