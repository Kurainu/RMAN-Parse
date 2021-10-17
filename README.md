# RMAN-Parse

This is a RMAN Parser That Parses a given Manifest and Saves it as a json file in the Current Directory.

This a C# Port from the RMAN Parser from Stelar7

## Usage

```
RMAN-Parse.exe <Manifest file>
```

## RMAN File Structure 
This Tree Shows only Classes and Properties using Classes as Type.
```
RMAN-File
┣ File Header
┣ Manifest
┃ ┗ Manifest Header
┃ ┗ Bundles
┃   ┗ Chunks
┃ ┗ Languages
┃ ┗ Files (List)
┃   ┗ Chunks
┃ ┗ Folders
```
