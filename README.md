# SdsgcStudio

**SdsgcStudio** is a modified version of AssetStudio designed for The Seven Deadly Sins: Grand Cross. Based on [AssetStudioMod](https://github.com/aelurum/AssetStudio).

**Neither the repository, nor the tool, nor the author of the tool, nor the author of the modification is affiliated with, sponsored, or authorized by Unity Technologies, Netmarble or their affiliates.**

## SdsgcStudio Usage

A new `Sdsgc` menu allows downloading and loading Unity bundles from the game without requiring the game to be installed.  
Open the `Sdsgc` menu, set the `Patch` code, `Version` and decryption `Key` and click on `Search bundles`.  

<img width="346" height="244" alt="image" src="https://github.com/user-attachments/assets/8c497e3e-98e9-4e8b-b345-1147d476bb29" />

To find the values to input, go to [7dsHub](https://7dshub.com/), scroll to the bottom and paste the 3 lines in the same order.  

<img width="328" height="67" alt="image" src="https://github.com/user-attachments/assets/dfcb775c-9e25-4be1-a6fb-793e9b524412" />

A new form will appear allowing you to search assets with several options:
- Name
- Container
- Bundle Name
- Regex

After performing a search, bundles that have at least one asset matching your query will appear, these bundles can be selected.  
Once you are done selecting, click on `Download x Bundles` and wait for the bundles to finish importing.  
You can then use AssetStudio like you normally would with the files.  

<img width="814" height="495" alt="image" src="https://github.com/user-attachments/assets/cde99b9d-a776-4258-b40e-d153f7cc3594" />

Notes:
- An internet connection is required to use the Sdsgc menu.
- Bundles are kept in memory and never written to disk, so `Show original file` does nothing for them.
- AssetStudio can still be used like the original one for other Unity bundles, this fork only adds a menu to make looking for Sdsgc files more convenient.
- AssetStudioCLI has no additional functionality for Sdsgc.

## Download

[SdsgcStudio-v1.0.0_portable-win-x64.zip](https://github.com/Alexis3857/SdsgcStudio/releases/download/v1.0.0/SdsgcStudio-v1.0.0_portable-win-x64.zip) small download, requires the [.NET 10 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/10.0).  
[SdsgcStudio-v1.0.0_contained-win-x64.zip](https://github.com/Alexis3857/SdsgcStudio/releases/download/v1.0.0/SdsgcStudio-v1.0.0_contained-win-x64.zip) larger download, works without installing anything. Pick this one if you're not sure.

## Build

* Visual Studio 2026 or newer (C++ `v145` toolset, .NET desktop development workload)
* .NET 10 SDK
* **AssetStudioFBXNative** uses [FBX SDK 2020.2.1](https://www.autodesk.com/developer-network/platform-technologies/fbx-sdk-2020-2-1), before building, you need to install the FBX SDK and modify the project file, change include directory and library directory to point to the FBX SDK director

### Everything from this [README.md](https://github.com/aelurum/AssetStudioMod/blob/6de33d04379828abfcd35e0965b23d03b88f4751/README.md) that doesnt contradict what I said above is valid
