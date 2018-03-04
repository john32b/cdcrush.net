# Building cdcrush dot NET


### Requirements

- Visual Studio 2017 (*I used community edition, which is free*)\
https://www.visualstudio.com/downloads/
- The code makes use of some C#6 features, Visual Studio 2015, might work.

### Running Requirements

These tools are needed to run cdcrush:

- **FreeArc**, an open source archiver. **Included** in the project files.\
Project Site : https://sourceforge.net/projects/freearc
- **Ecmtools**, open source CD tools. **Included** in the project files. \
Project Site :  https://github.com/kidoz/ecm
- **FFmpeg**, open source video/audio codecs. **NOT INCLUDED** \
Project Site : https://www.ffmpeg.org


### Building `release`

Create a subfolder named `tools` and put all the external executables there `[Arc.exe , ecm.exe , unecm.exe, ffmpeg.exe ]`

### Building `debug`

The tool executables are fetched from the root repo folder `/tools/`. All the tools are provided in the repo except `FFmpeg` which you need to download and place it on your system/user path.
