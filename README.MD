# CDCRUSH dot NET

**Name**: CDCrush, *Highy compress cd-image games*
**Author:** John Dimi, <johndimi@outlook.com> *twitter*: [@jondmt](https://twitter.com/jondmt)
**Project Page and Sources:** [https://github.com/johndimi/cdcrush.net](https://github.com/johndimi/cdcrush.net)
**Language:** C# dotNET 4.5, VS2017 **Licence:** MIT
**Version:** 1.2 **Platform:** Windows

## Download
To get the latest version check [the releases page here on github](https://github.com/johndimi/cdcrush.net/releases). Available for **windows** only.

## What is it

**Cdcrush** is a tool that can highly compress **CD based games** *( playstation 1, Pc-engine, etc )* for storage / sharing purposes. 

**How it is done:** 

- The program separates the tracks of the cd image and compress them separately.
- For **data** tracks it will use **ecm tools** to remove Error Correction/Detection Codes (ECC/EDC) data from the sectors. *( redundant data )*
- For  **audio** tracks it willl use the **ogg vorbis** audio codec to produce a decent quality audio file ( custom bitrate 64-500kbps) with a much smaller filesize compared to the uncompressed PCM audio original.
- Finally it compresses everything into a single archive using the **FreeArc** archiver.

> **NOTE** : Cdcrush is NOT for creating a 1:1 copy of the original CD, as the audio tracks are being converted to lossy audio, and then converted back to PCM.
> **BUT** : For normal home use and just playing games the audio quality is still very good and you won't even notice the difference.

 
**Then**  it can  **restore** the crushed cd image back to it's original form, a **.bin/.cue** image that is ready to be used however you like.
 
### Some example of game sizes archived with 7zip and cdcrush:

| Title             |  Raw    | 7z      | cdcrush |
| ----------------- | ------- | ------  | ------  |
| Wipeout 3         |  577mb  |  ~490mb  | **77mb**   |
| Die Hard Trilogy  |  532mb  |  ~450mb  | **71mb**   |
| Alien Trilogy     |  507mb  |  ~440mb  | **88mb**   |

<sup>Those games feature many CD-DA audio tracks</sup>

## USAGE

### General Infos

- **cdcrush** is only compatible with `.cue/.bin` type CD images
- Supports dragging and dropping of files into the main window.
- When selecting an **output folder** you will be asked to save a dummy file, that file will not ever be written and the program will select the folder of that file.
- The progress bar is a bit clunky, if it gets stuck just wait a bit. It's working.
- You can download the release without **ffmpeg** if you already have it, just be sure to set it up in the `settings` tab.

### Crushing  (*Compressing* )

![](https://i.imgur.com/SGJcHUo.png)
- Select an input file ( **.cue** ). You can drop it in the window or click the [...] button to select one
- Optionally set an output folder, the final **.arc** file will be created there, auto-named after the CD title
- Optionally set a cover image by dropping a **.png** file in the window, or by clicking on the image placeholder
- Choose the audio bitrate to compress the audio tracks (*if any audio tracks on the cd*)
- You can optionally change the Cd Title,
- Press the **CRUSH** button
- Wait for the operation to end

### Restoring

![](https://i.imgur.com/KtDuMuw.png)

- Select a previously crushed archive ( **.arc** ) to be restored. You can drop it in the window or click the [...] button to select one. 
- Optionally set an output folder, the (**.cue/.bin**) files will be created there, auto-named after the CD title.
- Check the `Create subfolder` checkbox to create a subfolder named after the CD title and put the cd image there
- Check the `Single bin/cue` to **force** restoring the cd back into a single bin file, even if the original source was a multi file bin, cue image.
- Press the **RESTORE** button
- Wait for the operation to end

### Settings

![](https://i.imgur.com/mtlI5DR.png)

**Temp Folder** :
 You can set a custom temporary folder for the various background processes like ffmpeg. Useful if you have a RAMDRIVE. **NOTE** : Make sure the temp folder can hold up to 1.2GB of data!

**FFmpeg Path** :
Point to the location of `ffmpeg.exe` , it is needed for encoding/decoding the audio tracks. If you have `ffmpeg` set on the system/user PATH, it will be checked once you **clear** the custom path.

**Max Concurrent Tasks**:
How many tracks to process at the same time. *(For when compressing or restoring with ffmpeg and ecm tools)*


## CHANGELOG

### V1.2
 - First **dotNet** release, continuing the project from the [NPM cdcrush 1.12](https://www.npmjs.com/package/cdcrush)  release
 - Calculate and display the **MD5** of the first data track, useful to quickly identify the validity of the CD
 - Ability to add an image cover to a cd archive. The image will then be displayed when restoring a CD.
 - MultiThreaded. Background FFmpeg and ECM operations can execute in parallel. *(Custom amount of threads in the options tab)*

## Q&A

**Q** : Why?
**A** : I wanted to save space on my hard drive and I think it's a decent way to store cd images, better than just compressing with 7zip or Rar. Also It was a good programming practice.

**Q** : Does it support games from SegaCD, Jaguar, 3DO, X, Y?
**A** : Theoretically it should support all valid **.cue/.bin** files, try it out.

**Q** : I am worried about the audio quality.
**A** : The OGG vorbis codec is decent and it can produce very good results even at 96kbps. Also, implementing a **FLAC** codec is coming at a later version

**Q** : But compressing the cd audio over and over again, it will deteriorate the quality vastly (*That was a real question I got for the NPM version a while back*)
**A**: You are **NOT SUPPOSED** to compress a restored image.  Just compress once from the original cd source and keep the crushed archive.

**Q** : Why there are two versions of the program?
**A** : FFmpeg.exe is a small beast ( *around 36 megabytes*) and in some cases users already have it on their pc.


## Npm Version

This project is a continuation of [**cdcrush npm**](https://www.npmjs.com/package/cdcrush).  You can check it out if you want, but **note** that the NPM version is old and **NOT RECOMMENDED TO USE** until I update it.

## About

Feel free to provide feedback and contact me on social media and email. Donations are always welcome! :smile: 

[![Donate](https://www.paypalobjects.com/en_US/i/btn/btn_donate_LG.gif)](https://www.paypal.me/johndimi)

Thanks for checking this out,

John.
