## CHANGELOG

### V1.4.2
	- Program will check for `ffmpeg` in the `tools` folder. Now the full release package will run out of the box.
    - Removed ffmpeg clear button from settings. It didn't provide any meaningful functionality

### V1.4.1
	- Fixed Crash when loading a CD with any track having a pregap

### V1.4
- New CD Parser class
	- Fixed times bug when converting multifile CD images to a single .bin
	- built with **HAXE** and is included as a DLL file
	- is the exact same class used in the **nodejs** version of cdcrush *(same codebase)*
- Fixed bug, when setting a custom TEMP folder, it now sets and restores it correctly.
- When **crushing** and the output archive already exists, it doesn't overwrite it, instead it renames the new archive by adding a `_` at the end of the file. 

### V1.3
- Added LOG window. You can now see a realtime log by pressing the "LOG" button on the bottom right of the window
- Upgraded version to 1.3, because many new features were added since 1.2

### V1.2.4
- Progress Bar now reports total progress correctly and updates more frequently 
- Progress Report on taskbar and window title
- Detailed CD report with individual track infos and checksums
- MP3 support *(In cases you want to convert to cue/mp3 for use in specific emulators, like KEGA Fusion)*
- Small usability tweaks
- If output folder exists, program puts a "_" at the end of the folder to avoid overwriting

### V1.2.3
- Added ability to select the compression level of the final archive from 1-9
- Function to convert a `.cue/.bin` to a .cue with encoded audio files. This can be really useful as some emulators support this .cue format.
- Re-added ogg vorbis support and now user can choose between (flac, opus, vorbis). This is for when creating cue/encoded audio, some emulators might only support ogg vorbis (*like mednafen*)

### V1.2.2
- Replaced the vorbis audio codec with the **opus** codec. It seems to be a better encoder and even supports lower bitrates (*like 32k*)

### V1.2.1
- Added **FLAC** support. Audio tracks can be encoded and then restored to an exact copy of their original source, to the last byte. Now the full CD can be preserved byte for byte.

### V1.2
 - First **dotNet** release, continuing the project from the [NPM cdcrush 1.12](https://www.npmjs.com/package/cdcrush)  release
 - Calculate and display the **MD5** of the first data track, useful to quickly identify the validity of the CD
 - Ability to add an image cover to a CD archive. The image will then be displayed when restoring a CD.
 - MultiThreaded. Background FFmpeg and ECM operations can execute in parallel. *(Custom amount of threads in the options tab)*
 - Temporary files will be deleted upon user program exit.
