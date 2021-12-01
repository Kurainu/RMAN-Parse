using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RMAN_Parse.RMAN
{
    public class RMANFileEntry
    {
        public int Offset { get; set; }
        public int TableoffsetOffset { get; set; }
        public int CustomNameOffset { get; set; }
        public int FiletypeFlag { get; set; }
        public int NameOffset { get; set; }
        public String Name { get; set; }
        public int Structsize { get; set; }
        public int SymlinkOffset { get; set; }
        public String Symlink { get; set; }
        public long FileId { get; set; }
        public long DirectoryId { get; set; }
        public string FullPath { get; set; }
        public int FileSize { get; set; }
        public int Permissions { get; set; }
        public int LanguageId { get; set; }
        public int Unknown2 { get; set; }
        public int Unknown3 { get; set; }
        public Boolean IsSingleChunk { get; set; }
        public List<RMANBundleChunkEntry> Chunks { get; set; }

        public void DownloadFile(string Outputpath)
        {
            foreach (RMANBundleChunkEntry chunk in Chunks)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("");
                request.AddRange(0, 600000);
                using (WebResponse response = request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (FileStream output = File.Create(Outputpath))
                {
                    stream.CopyToAsync(output);
                }
            }
        }
    }
}
