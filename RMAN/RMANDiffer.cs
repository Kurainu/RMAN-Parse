using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMAN_Parse.RMAN
{
    public class RMANDiffer
    {
        private RMANFile Source { get; set; }
        private RMANFile Update { get; set; }
        public RMANDiffer(RMANFile Sourcefile, RMANFile Updatefile = null)
        {
            Source = Sourcefile;
            Update = Updatefile;
        }

        public List<RMANFileEntry> GetDiffFiles()
        {
            List<RMANFileEntry> Files = new List<RMANFileEntry>();
            if (Update != null)
            {
                foreach (RMANFileEntry Fileentry in Update.Manifest.Files)
                {
                    var Diffentry = Fileentry;
                    
                    var srcfileentry = Source.Manifest.Files.Where(f => f.FileId == Fileentry.FileId);
                    if (srcfileentry.Count() == 0)
                    {
                        Diffentry.Chunks = Fileentry.Chunks;
                    }
                    else
                    {
                        Diffentry.Chunks = GetDiffChunks(srcfileentry.FirstOrDefault(),Fileentry);
                    }

                    Files.Add(Diffentry);
                }
            }
            else
            {
                Files = Source.Manifest.Files;
            }

            return Files;
        }

        private List<RMANBundleChunkEntry> GetDiffChunks(RMANFileEntry src , RMANFileEntry update)
        {
            List<RMANBundleChunkEntry> chunks = new List<RMANBundleChunkEntry>();

            foreach (RMANBundleChunkEntry chunk in update.Chunks)
            {
                var srchunk = src.Chunks.FirstOrDefault(c => c.ChunkId == chunk.ChunkId);
                if (!chunk.Equals(srchunk))
                {
                    chunks.Add(chunk);
                }
            }

            return chunks;
        }
    }
}
