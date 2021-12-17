using Newtonsoft.Json;
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

        /// <summary>
        /// Downloads all Chunks and assembles Final File
        /// </summary>
        /// <param name="Outputpath">Full Outpath Name with Filename and extension</param>
        /// <param name="bundleurl">Bundle Url for the Chunks to be downloaded from</param>

        public void DownloadFile(string Outputpath, string bundleurl)
        {
            using (FileStream fs = new FileStream(Outputpath, FileMode.Append))
            {
                foreach (RMANBundleChunkEntry chunk in Chunks)
                {
                    byte[] decompressed = new byte[chunk.UncompressedSize];
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"{bundleurl}\\{chunk.ParentBundleID}.bundle");
                    request.AddRange(chunk.OffsetToChunk, (chunk.OffsetToChunk + chunk.CompressedSize));
                    WebResponse response = request.GetResponse();
                    ZstdSharp.ZstdStream zstdStream = new ZstdSharp.ZstdStream(response.GetResponseStream(), ZstdSharp.ZstdStreamMode.Decompress);
                    zstdStream.Read(decompressed);
                    fs.Write(decompressed);
                }
            }
        }

        /// <summary>
        /// Returns a Memory Stream for the File
        /// </summary>
        /// <param name="bundleurl">Bundle Url for the Chunks to be downloaded from</param>

        public MemoryStream GetStream(string bundleurl)
        {
            MemoryStream stream = new MemoryStream();
            foreach (RMANBundleChunkEntry chunk in Chunks)
            {
                byte[] decompressed = new byte[chunk.UncompressedSize];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"{bundleurl}\\{chunk.ParentBundleID}.bundle");
                request.AddRange(chunk.OffsetToChunk, (chunk.OffsetToChunk + chunk.CompressedSize));
                WebResponse response = request.GetResponse();
                ZstdSharp.ZstdStream zstdStream = new ZstdSharp.ZstdStream(response.GetResponseStream(), ZstdSharp.ZstdStreamMode.Decompress);
                zstdStream.CopyTo(stream);
            }

            return stream;
        }
    }
}
