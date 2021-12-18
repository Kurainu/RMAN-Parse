# RMAN-Parse

This is a RMAN Parser That Parses a given Manifest and Saves it as a json file in the Current Directory.

This a C# Port with a handful additions from the RMAN Reader part from [Stelar7s lol-parser](https://github.com/stelar7/lol-parser)

## Usage

```
RMAN-Parse.exe <Manifest file>
```

## RMAN File Structure 
This Tree shows only Classes and Properties using Classes as Type. For all Properties please use the links below.\
The exported Json Manifest has also this Structure

RMAN-File \
┣ [File Header](RMAN/RMANFileHeader.cs) \
┗ [Manifest](RMAN/RMANManifest.cs) \
&nbsp;&nbsp; ┣ [Manifest Header](RMAN/RMANManifestHeader.cs) \
&nbsp;&nbsp; ┣ [Bundles](RMAN/RMANBundleEntry.cs) \
&nbsp;&nbsp; ┃┗ [Chunks](RMAN/RMANBundleChunkEntry.cs) \
&nbsp;&nbsp; ┣ [Languages](RMAN/RMANLanguageEntry.cs) \
&nbsp;&nbsp; ┣ [Files](RMAN/RMANFileEntry.cs) \
&nbsp;&nbsp; ┃┗ [Chunks](RMAN/RMANBundleChunkEntry.cs) \
&nbsp;&nbsp; ┗  [Folders](RMAN/RMANFolderEntry.cs)
