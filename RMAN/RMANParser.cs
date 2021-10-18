using System;
using System.Collections.Generic;
using System.IO;
namespace RMAN_Parse.RMAN
{
    class RMANParser
    {
        private static Dictionary<string, RMANBundleChunkEntry> ChunkMap { get; set; } = new Dictionary<string, RMANBundleChunkEntry>();

        public RMANFile Parse(string path)
        {
            RMANFile file = new RMANFile();
            BinaryReader reader = new BinaryReader(File.OpenRead(path));
            file.FileHeader = ParseHeader(reader);
            if (file.FileHeader.Magic != "RMAN")
            {
                Console.WriteLine("No RMAN Magic Bytes found. Please Provide A Valid Manifest");
                Environment.Exit(1);
            }
            file.Manifest = ParseManifest(reader, file);
            return file;
        }

        private RMANManifest ParseManifest(BinaryReader reader, RMANFile file)
        {
            RMANManifest manifest = new RMANManifest();
            reader = new BinaryReader(DecompressZStandard(reader,file.FileHeader));

            manifest.ManifestHeader = ParseManifestHeader(reader);
            manifest.Bundles = GetBundles(reader, manifest);
            manifest.Languages = GetLanguages(reader, manifest);
            manifest.Files = GetFiles(reader, manifest);
            manifest.Folders = GetFolders(reader, manifest);

            return manifest;
        }

        private RMANFileHeader ParseHeader(BinaryReader reader)
        {
            RMANFileHeader header = new RMANFileHeader();

            header.Magic = new string(reader.ReadChars(4));
            header.Major = reader.ReadByte();
            header.Minor = reader.ReadByte();
            header.Unknown = reader.ReadByte();
            header.SignatureType = reader.ReadByte();
            header.Offset = reader.ReadInt32();
            header.Length = reader.ReadInt32();
            header.Manifestid = reader.ReadInt64();
            header.DecompressedLength = reader.ReadInt32();
            return header;
        }

        private Stream DecompressZStandard(BinaryReader reader, RMANFileHeader header)
        {
            reader.BaseStream.Seek(header.Offset,SeekOrigin.Begin);
            ZstdSharp.ZstdStream zstdStream = new ZstdSharp.ZstdStream(reader.BaseStream,ZstdSharp.ZstdStreamMode.Decompress);
            byte[] buffer = new byte[header.DecompressedLength];
            zstdStream.Read(buffer);
            MemoryStream stream = new MemoryStream(buffer);

            return stream;
        }

        private RMANManifestHeader ParseManifestHeader(BinaryReader reader)
        {
            RMANManifestHeader header = new RMANManifestHeader();

            var HeaderOffset = reader.ReadInt32();
            reader.BaseStream.Seek(HeaderOffset, SeekOrigin.Begin);

            header.OffsetTableOffset = reader.ReadInt32();

            var Offset = reader.ReadInt32();
            header.BundleListOffset = (int)(reader.BaseStream.Position + Offset) - 4;

            Offset = reader.ReadInt32();
            header.LanguageListOffset = (int)(reader.BaseStream.Position + Offset) - 4;

            Offset = reader.ReadInt32();
            header.FileListOffset = (int)(reader.BaseStream.Position + Offset) - 4;

            Offset = reader.ReadInt32();
            header.FolderListOffset = (int)(reader.BaseStream.Position + Offset) - 4;

            Offset = reader.ReadInt32();
            header.KeyHeaderOffset = (int)(reader.BaseStream.Position + Offset) - 4;

            Offset = reader.ReadInt32();
            header.UnknownOffset = (int)(reader.BaseStream.Position + Offset) - 4;

            return header;
        }

        public static List<RMANBundleEntry> GetBundles(BinaryReader reader, RMANManifest manifest)
        {
            reader.BaseStream.Seek(manifest.ManifestHeader.BundleListOffset, SeekOrigin.Begin);

            int bundlecount = reader.ReadInt32();

            List<RMANBundleEntry> Bundles = new List<RMANBundleEntry>();
            for (int i = 0; i < bundlecount; i++)
            {
                RMANBundleEntry Bundle = new RMANBundleEntry();
                Bundle.Offset = reader.ReadInt32(); //Bundle Offset
                int CurrentBundlePosition = (int)reader.BaseStream.Position; // Position where the next BundleOffset is
                reader.BaseStream.Seek(CurrentBundlePosition + Bundle.Offset - 4, SeekOrigin.Begin);

                Bundle.TableoffsetOffset = reader.ReadInt32();
                Bundle.HeaderSize = reader.ReadInt32();
                Bundle.BundleId = reader.ReadInt64().ToString("X");

                if (Bundle.HeaderSize > 12)
                {
                    reader.BaseStream.Position += (Bundle.HeaderSize - 12);
                }

                int ChunkCount = reader.ReadInt32();
                List<RMANBundleChunkEntry> chunks = new List<RMANBundleChunkEntry>();
                int OffsettoChunk = 0;
                for (int j = 0; j < ChunkCount; j++)
                {
                    RMANBundleChunkEntry chunk = new RMANBundleChunkEntry();
                    int Chunkoffset = reader.ReadInt32();
                    int CurrentChunkPosition = (int)reader.BaseStream.Position; // Saving Current Position for Seeking Back for the next Chunk
                    reader.BaseStream.Seek(CurrentChunkPosition + Chunkoffset - 4, SeekOrigin.Begin);

                    chunk.ParentBundleID = Bundle.BundleId;
                    chunk.TableOffsetOffset = reader.ReadInt32();
                    chunk.CompressedSize = reader.ReadInt32();
                    chunk.UncompressedSize = reader.ReadInt32();
                    chunk.ChunkId = reader.ReadInt64().ToString("X");
                    chunk.OffsetToChunk = OffsettoChunk;
                    
                    reader.BaseStream.Seek(CurrentChunkPosition, SeekOrigin.Begin);

                    ChunkMap[chunk.ChunkId] = chunk;

                    chunks.Add(chunk);
                    OffsettoChunk += chunk.CompressedSize;
                }
                Bundle.Chunks = chunks;
                Bundles.Add(Bundle);
                reader.BaseStream.Seek(CurrentBundlePosition, SeekOrigin.Begin);
            }
            return Bundles;
        }

        public static List<RMANLanguageEntry> GetLanguages(BinaryReader reader, RMANManifest manifest)
        {
            reader.BaseStream.Seek(manifest.ManifestHeader.LanguageListOffset, SeekOrigin.Begin);

            List<RMANLanguageEntry> langs = new List<RMANLanguageEntry>();

            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                RMANLanguageEntry lang = new RMANLanguageEntry();

                lang.Offset = reader.ReadInt32();
                int CurrentLangPosition = (int)reader.BaseStream.Position;
                reader.BaseStream.Seek(CurrentLangPosition + lang.Offset - 4, SeekOrigin.Begin);

                lang.TableoffsetOffset = reader.ReadInt32();
                lang.Id = reader.ReadInt32();
                lang.NameOffset = reader.ReadInt32();

                reader.BaseStream.Seek(reader.BaseStream.Position + lang.NameOffset - 4, SeekOrigin.Begin);
                lang.Name = new string(reader.ReadChars(reader.ReadInt32()));

                reader.BaseStream.Seek(CurrentLangPosition, SeekOrigin.Begin);
                langs.Add(lang);
            }

            return langs;
        }

        public static List<RMANFileEntry> GetFiles(BinaryReader reader, RMANManifest manifest)
        {
            List<RMANFileEntry> files = new List<RMANFileEntry>();
            reader.BaseStream.Seek(manifest.ManifestHeader.FileListOffset, SeekOrigin.Begin);

            int Count = reader.ReadInt32();

            for (int i = 0; i < Count; i++)
            {
                RMANFileEntry file = new RMANFileEntry();

                file.Offset = reader.ReadInt32();
                int CurrentFilePositon = (int)reader.BaseStream.Position;
                reader.BaseStream.Seek(CurrentFilePositon + file.Offset - 4, SeekOrigin.Begin);

                file.TableoffsetOffset = reader.ReadInt32();

                int temp = reader.ReadInt32();
                int restoreOffset = 4;

                file.CustomNameOffset = temp & 0xFFFFFF;
                file.FiletypeFlag = temp >> 24;
                Boolean HasInlineName = reader.ReadInt32() < 100;
                reader.BaseStream.Seek(reader.BaseStream.Position - 4, SeekOrigin.Begin);
                if (HasInlineName)
                {
                    file.NameOffset = file.CustomNameOffset;
                }
                else
                {
                    file.NameOffset = reader.ReadInt32();
                    restoreOffset = 8;
                }

                reader.BaseStream.Seek(reader.BaseStream.Position + file.NameOffset - 4, SeekOrigin.Begin);
                file.Name = new string(reader.ReadChars(reader.ReadInt32()));
                reader.BaseStream.Seek(CurrentFilePositon + file.Offset + restoreOffset, SeekOrigin.Begin);

                file.Structsize = reader.ReadInt32();

                file.SymlinkOffset = reader.ReadInt32();
                reader.BaseStream.Seek(reader.BaseStream.Position + file.SymlinkOffset - 4, SeekOrigin.Begin);
                file.Symlink = new string(reader.ReadChars(reader.ReadInt32()));
                reader.BaseStream.Seek(CurrentFilePositon + file.Offset + 8 + restoreOffset, SeekOrigin.Begin);

                file.FileId = reader.ReadInt64();

                if (file.Structsize > 28)
                {
                    file.DirectoryId = reader.ReadInt64();
                }

                file.FileSize = reader.ReadInt32();
                file.Permissions = reader.ReadInt32();

                if (file.Structsize > 36)
                {
                    file.LanguageId = reader.ReadInt32();
                    file.Unknown2 = reader.ReadInt32();
                }

                file.IsSingleChunk = reader.ReadInt32() > 0;

                if (!file.IsSingleChunk)
                {
                    List<RMANBundleChunkEntry> chunks = new List<RMANBundleChunkEntry>();
                    int ChunkidCount = reader.ReadInt32();
                    for (int j = 0; j < ChunkidCount; j++)
                    {
                        chunks.Add(ChunkMap.GetValueOrDefault(reader.ReadInt64().ToString("X")));
                    }

                    file.Chunks = chunks;
                }
                else
                {
                    file.Chunks = new List<RMANBundleChunkEntry> { ChunkMap.GetValueOrDefault(reader.ReadInt64().ToString("X")) };
                    file.Unknown3 = reader.ReadInt32();
                }

                reader.BaseStream.Seek(CurrentFilePositon, SeekOrigin.Begin);
                files.Add(file);
            }

            return files;
        }

        public static List<RMANFolderEntry> GetFolders(BinaryReader reader, RMANManifest manifest)
        {
            List<RMANFolderEntry> Folders = new List<RMANFolderEntry>();
            reader.BaseStream.Seek(manifest.ManifestHeader.FolderListOffset, SeekOrigin.Begin);

            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                RMANFolderEntry folder = new RMANFolderEntry();

                folder.Offset = reader.ReadInt32();
                int CurrentFolderPosition = (int)reader.BaseStream.Position;
                reader.BaseStream.Seek(CurrentFolderPosition + folder.Offset - 4, SeekOrigin.Begin);

                folder.TableoffsetOffset = reader.ReadInt32();

                int resumeOffset = (int)reader.BaseStream.Position;
                reader.BaseStream.Seek(reader.BaseStream.Position - folder.TableoffsetOffset, SeekOrigin.Begin);

                folder.FolderIdOffset = reader.ReadInt16();
                folder.ParentIdOffset = reader.ReadInt16();

                reader.BaseStream.Seek(resumeOffset, SeekOrigin.Begin);
                folder.NameOffset = reader.ReadInt32();
                reader.BaseStream.Seek(reader.BaseStream.Position + folder.NameOffset - 4, SeekOrigin.Begin);

                folder.Name = new string(reader.ReadChars(reader.ReadInt32()));
                reader.BaseStream.Seek(CurrentFolderPosition + folder.Offset + 4, SeekOrigin.Begin);

                if (folder.FolderIdOffset > 0)
                {
                    folder.FolderId = reader.ReadInt64();
                }

                if (folder.ParentIdOffset > 0)
                {
                    folder.ParentId = reader.ReadInt64();
                }

                reader.BaseStream.Seek(CurrentFolderPosition, SeekOrigin.Begin);
                Folders.Add(folder);
            }

            return Folders;
        }
    }
}
