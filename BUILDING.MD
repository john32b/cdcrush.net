
# Building cdcrush dot NET


### Requirements

- Visual Studio 2017 (*I used community edition, which is free*)\
https://www.visualstudio.com/downloads/
- The code makes use of some C#6 features, Visual Studio 2015, might work.

### Running Requirements

These tools are needed to **run** cdcrush:

- **FreeArc**, an open source archiver. **Included** in the project files.\
Project Site : https://sourceforge.net/projects/freearc
- **Ecmtools**, open source CD tools. **Included** in the project files. \
Project Site :  https://github.com/kidoz/ecm
- **FFmpeg**, open source video/audio codecs. :warning: **NOT INCLUDED** \
Project Site : https://www.ffmpeg.org


### Building `release`

Create a subfolder named `tools` and put all the external executables there `[Arc.exe , ecm.exe , unecm.exe, ffmpeg.exe ]` <sup>*ffmpeg is optional*</sup>


### Building `debug`

The tool executables are fetched from the root repo folder `/tools/`. All the tools are provided in the repo except `FFmpeg` which you need to download and place it on your system/user path.



### `cdinfos.dll`


This file was build using **HAXE**, compiled code from [**cdcrush nodejs**](https://github.com/johndimi/cdcrush) version. 
Basically it's 2 classes, `CDInfo` and `CDTrack` compiled into a DLL so that I can have a single codebase for the CueParse for both cdcursh versions

You can find the [source code for there files here.](https://github.com/johndimi/cdcrush/tree/master/src/cd)

> :warning: You don't really have to build it yourself, you can just grab `cdinfos.dll` that is inside the project folder.

#### To compile the cdinfos DLL with HAXE run this command:
```haxe
haxe -cp src/cd -D dll -D no-root -cs cdinfos cd.CDInfos cd.CDTrack
```

- `src/cd` must point to the nodejs version folder where the CD classes are
- It will create a directory `cdinfos/` and in there you can find the .dll file

