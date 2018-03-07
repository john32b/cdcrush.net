# CDCRUSH dot NET

**Name**: CDCrush, *Highy compress cd-image games*\
**Author:** John Dimi, <johndimi@outlook.com> *twitter*: [@jondmt](https://twitter.com/jondmt)\
**Project Page and Sources:** [https://github.com/johndimi/cdcrush.net](https://github.com/johndimi/cdcrush.net)\
**Language:** C# dotNET 4.5, VS2017 **Licence:** MIT\
**Version:** 1.2.2 **Platform:** Windows

## Download
To get the latest version check [the releases page here on github](https://github.com/johndimi/cdcrush.net/releases). Available for **windows** only.

## What is it

**Cdcrush** is a tool that can highly compress **CD based games** *( playstation 1, Pc-engine, etc )* for storage / sharing purposes. 

**How it is done:** 

- The program separates the tracks of the cd image and compress them separately.
- For **data** tracks it will use **ecm tools** to remove Error Correction/Detection Codes (ECC/EDC) data from the sectors. *( redundant data )*
- For  **audio** tracks it will use the **ogg OPUS** audio codec to produce a decent quality audio file (*custom bitrate 32-320kbps vbr*) with a much smaller filesize compared to the uncompressed PCM audio original.
- **NEW:** You can choose to encode the **audio tracks** with the **FLAC** lossless codec.
- Finally it compresses everything into a single archive using the **FreeArc** archiver.
 
**Then**  it can  **restore** the crushed CD image back to it's original form, a **.bin/.cue** image that is ready to be used however you like.

**NEW from V1.2.1**: Archives with the audio tracks encoded with **FLAC**, will be restored to a 1:1 copy, byte for byte, of the original source CD
 
### Some example of game sizes archived with 7zip and cdcrush:

| Title             |  Raw    | 7z      | cdcrush-ogg|  cdcrush-flac |
| ----------------- | ------- | ------  | -----------  | -------|
| Tomb Raider 2     |  710MB  |  ~ 455MB  | **150MB**  | **320MB** |
| Wipeout 3         |  577MB  |  ~ 490MB  | **77MB**   | .|
| Die Hard Trilogy  |  532MB  |  ~ 450MB  | **71MB**   | .|
| Alien Trilogy     |  507MB  |  ~ 440MB  | **88MB**   | .|

<sup>Those games feature many CD-DA audio tracks</sup>
<sup>7zip maximum compression default template</sup>

## USAGE

### General Infos

- **cdcrush** is only compatible with `.cue/.bin` type CD images
- Supports dragging and dropping of files into the main window.
- When selecting an **output folder** you will be asked to save a dummy file, that file will not ever be written and the program will select the folder of that file.
- **[!]** The progress bar is a bit clunky, if it gets stuck just wait a bit. It's working.
- You can download the release without **FFmpeg** if you already have it, just be sure to set it up in the `settings` tab.

### Crushing  (*Compressing* )

![](https://i.imgur.com/SGJcHUo.png)
- Select an input file ( **.cue** ). You can drop it in the window or click the **[...]** button to select one
- Optionally set an output folder, the final **.arc** file will be created there, auto-named after the CD title
- Optionally set a cover image by dropping a **.png** file in the window, or by clicking on the image placeholder
- Choose the audio bitrate to compress the audio tracks (*when audio tracks are present on the cd*) **NEW** You can choose the **FLAC** codec here
- You can optionally change the CD Title, in case the automatic name is wrong
- Press the **CRUSH** button
- Wait for the operation to end

### Restoring

![](https://i.imgur.com/KtDuMuw.png)

- Select a previously crushed archive ( **.arc** ) to be restored. You can drop it in the window or click the **[...]** button to select one
- Optionally set an output folder, the (**.cue/.bin**) files will be created there, auto-named after the CD title
- Check the `Create subfolder` checkbox to create a subfolder named after the CD title and put the cd image there
- Check the `Single bin/cue` to **force** restoring the CD back into a single bin file, even if the original source was a multi file bin/cue image
- Press the **RESTORE** button
- Wait for the operation to end

### Settings

![](https://i.imgur.com/mtlI5DR.png)

**Temp Folder** :
 You can set a custom temporary folder for the various background processes like FFmpeg. Useful if you have a RAMDRIVE. \
 **NOTE** : Make sure the temp folder can hold up to 1.2GB of data !

**FFmpeg Path** :
Point to the location of `ffmpeg.exe` , it is needed for encoding/decoding the audio tracks. If you have `FFmpeg` set on the system/user PATH, it will be checked once you **clear** the custom path.

**Max Concurrent Tasks**:
How many tracks to process at the same time. *(For when compressing or restoring with ffmpeg and ecm tools)*


## CHANGELOG

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

## Q&A

**Q** : Why?\
**A** : I wanted to save space on my hard drive and I think it's a decent way to store cd images, better than just compressing with 7zip or Rar. Also It was a good programming practice.

**Q** : Does it support games from SegaCD, Jaguar, 3DO, X, Y?\
**A** : Theoretically it should support all valid **.cue/.bin** files, try it out.

**Q** : I am worried about the audio quality.\
**A** : The OGG ~~vorbis~~ (new **OPUS**) codec is decent and it can produce very good results even at 96kbps. ~~Also, implementing a **FLAC** codec is coming at a later version.~~

**Q**: Is storing the entire CD with FLAC really lossless? I am worried about byte integrity.
**A**: YES, to the last byte. The filesize and checksums of the restored tracks are the same as the original ones. (data&audio). You can check for yourself by calculating the checksums of restored files vs original source. **ALSO Checksum reports** are coming at a later version.

**Q** : Why there are two versions of the program?\
**A** : FFmpeg.exe is a small beast ( *around 36 megabytes*) and in some cases users already have it on their pc. So I am publishing a version with and a version without it.


## Npm Version

This project is a continuation of [**cdcrush npm**](https://www.npmjs.com/package/cdcrush).  You can check it out if you want, but **note** that the NPM version is old and **NOT RECOMMENDED TO USE** until I update it.

## About

Feel free to provide feedback and contact me on social media and email. Donations are always welcome! :smile: \
[![Donate](https://www.paypalobjects.com/en_US/i/btn/btn_donate_LG.gif)](https://www.paypal.me/johndimi)

Thanks for checking this out,\
John.
