# CDCRUSH dot NET

**Name**: CDCRUSH, *Highly compress cd-image games*  
**Author:** John Dimi :computer:, <johndimi@outlook (.) com> *twitter*: [@jondmt](https://twitter.com/jondmt)  
**Project Page and Sources:** [https://github.com/johndimi/cdcrush.net](https://github.com/johndimi/cdcrush.net)  
**Language:** C# .NET 4.5, **Licence:** MIT  
**Version:** 1.5 **Platform:** Windows  
**Sister Project** : [cdcrush nodejs](https://www.npmjs.com/package/cdcrush)

## Download

To get the latest version check [the releases page here on github](https://github.com/johndimi/cdcrush.net/releases).  
*Available for **windows** only.*


## What is it

![CDCRUSH LOGO](images/logo.png)  

**Cdcrush** is a tool that can highly compress **CD based games**, *(PS1, PC-Engine, PC, etc.)*. Reduces size of data and audio tracks. Ideal for **storage/sharing**.  
Then it can **restore** a compressed archive back to a CD Image `.cue/.bin`. Also you can use a **lossless audio codec** to store/restore a `1:1` ,byte to byte, copy of a CD Image.

:hammer_and_wrench: **In Detail**:  

- Applies **ECM tools** to DATA tracks. This removes redundant data *(Error Correction/Detection Codes)* from the CD sectors. This data can then be reconstructed
- Encodes **CDDA** audio tracks with lossy or lossless codecs *(Vorbis, Opus, FLAC, TAK)*
- Puts everything into an archive *(7zip, FreeArc)*
- Then the process can be reversed and the original **.bin/.cue** file is reconstructed


### Use cases

- **BACKUP** a 1:1 copy of CD images for **cold storage**. If you choose to encode audio tracks with `FLAC` or `TAK`, then the restoration will be the same as the source down to the last byte. This method is **more efficient** than directly compressing a cd image with a data archiver (*like 7zip*)
- **Highly Compress** your personal collection of CD games for storage. You can choose a lossy audio codec to greatly reduce the filesize of some games that feature many audio tracks. The audio will still sound great because modern audio codecs do a decent job at compressing audio.
- **Convert** CD images that include multiple audio tracks to **.cue/.bin + encoded audio tracks**, using an audio codec of your choosing. Some emulators *like mednafen* support loading these type of `.cue` files.

### :chart: Compression comparisons of some games:

| Name        | Raw Size | 7-zip <sup>0</sup> | CDCRUSH Lossless <sup>1</sup> | CDCRUSH Lossy <sup>2</sup> |
| ----------- | -------- | ------------------ | ----------------------------- | -------------------------- |
| Wipeout XL  | 680MB    | 567MB              | **407MB**                     | **72MB**                   |
| Tomb Raider | 505MB    | 306MB              | **275MB**                     | **169MB**                  |
| PO'ed       | 139MB    | 50MB               | **39MB**                      | **18MB**                   |

<sup>**0**: Direct compression of the CD Image with 7zip. Profile : Maximum Compression</sup>  
<sup>**1**: Audio : TAK , Archive : FreeArc High </sup>  
<sup>**2**: Audio : Ogg Vorbis 64k , Archive : FreeArc High </sup>


![Convert to encoded audio/cue Example](images/convert_example.png)
<sup>**Converting** to encoded audio tracks example</sup>


## USAGE

### :paperclip: General Info and Features

- **cdcrush** is only compatible with `.cue/.bin` type CD images. Some programs that allow you to rip your CDs to this format are:
	- [cdrtfe](https://cdrtfe.sourceforge.io/cdrtfe/index_en.html), open source
	- [ImgBurn](http://imgburn.com) ,free but not open source.
- When selecting an **output folder** you will be asked to save a dummy file, that file will not ever be written and the program will select the folder of that file.
- You can download the release without **FFmpeg** if you already have it, just be sure to set it up in the `settings` tab.
- :star2: You can see the detailed info of a CD along with the tracks checksum. When restoring a CD, click the `Detailed Info` button. And for when crushing a CD, the button will become active AFTER the operation is complete.
- :scroll: Logging. You can click the `log` button on the bottom right of the window to open the main log. It's an advanced feature, use it in case something goes wrong to get more detailed info.
- When **BACKING UP**, cdcrush will use **ECM Tools** to the data tracks. A small program that removes unnecessary error recovery data, resulting in even smaller archives. *(The error data can then be recovered)*
- **Audio Codecs Supported** : Ogg Vorbis, Ogg Opus, Mp3, FLAC *lossless*, TAK *lossless*
- **Archive Types Supported** : FreeArc, 7zip, Zip
- **TIPS**
	- **FreeArc** offers the best compression results, *for cold storage*
	- **ZIP** is the most compatible for mounting and the fastest to extract.
	- **YOU CAN'T** restore cd images you have previously converted to **encoded audio tracks**. You can **only restore images** you have compressed using the *normal **backup*** option.
	- **TAK Audio Codec** Produces a good compression and is very fast. Try it out
	

## :cd: Crushing a CD (*Compressing* )

![Crushing a CD](images/tab_compress.png)  
- Select an input file `.cue`. You can **drop it** in the window or click the `[...]` button to select one
- Optionally set an **Destination folder**, the final archive  will be created there, auto-named after the CD title
- Optionally set a cover image by dropping a **.jpg** file in the window, or by clicking on the image placeholder
- Choose an **audio codec** and **bitrate** to encode the audio tracks with (*if any*)
- You can optionally change the **CD Title**
- **Compression**, Select an the archiver type to use
- ::star:: **Method** combo box
	- **Normal backup**: This will encode all tracks and create a cold storage archive. :exclamation: This is the archive type that can be restored to a full cd image later :exclamation:
	- **To cue/bin/encoded audio tracks**: This will just encode all audio tracks into separate files and create a new `.cue` file. Used for loading games with emulators that support this format
	- **To cue/bin/encoded audio tracks [ARCHIVE]** : Same as above, but will put the files into an **archive**. Make sure you select the compression settings above.


## :arrows_clockwise: Restoring a CD

![Restoring a CD](images/tab_restore.png)  

- Select a previously **BACKED UP** archive `.arc .zip .7z` to be restored. You can **drop it** in the window or click the `[...]` button to select one.
- Optionally set a **Destination Folder**, the files will be created there, auto-named after the CD title
- **Create Subfolder** - If you tick this, a new folder in `destination folder` will be created and files will be put there
- **METHOD**
	- **Restore** - This is a normal restore. Will restore an archive back to a full `.cue/.bin`
	- **Restore and Merge** - Same as normal *restore* but will **force** all tracks of the CD to be merged into a single `.bin`.
	- **Restore to cue/bin/encoded audio tracks** - This is the same as you would just **convert** a cd to this format from the crash menu. Will keep the audio tracks encoded and will restore data tracks. Useful to play on emulators that support .cue with encoded audio.

## :wrench: Settings tab

![Settings tab](images/tab_settings.png)  

**Temp Folder** :
 You can set a custom temporary folder for the various background processes like FFmpeg. Useful if you have a RAMDRIVE. \
 :warning: **WARNING** : Make sure the temp folder can hold up to 1.2GB of data !

**FFmpeg Path** :
Point to the location of `ffmpeg.exe` , it is needed for encoding/decoding the audio tracks. If you have `FFmpeg` set on the system/user PATH, it will be checked once you **clear** the custom path. 
 - Note: If you download the **full package** of cdcrush. FFMPEG is included, so don't worry about it.

**Max Concurrent Tasks**:
How many tracks to process at the same time. *(For when compressing or restoring with ffmpeg and ecm tools)*


## :exclamation: CHANGELOG
See [`CHANGELOG.MD`](CHANGELOG.MD)

## :clipboard: Q&A

**Q** : Why?  
 **A** : I wanted to save space on my hard drive and I think  it's a decent way to store CD images, better than just compressing with  7zip or Rar. Also It was a good programming practice.

**Q** : Does it support games from Sega CD, Jaguar, 3DO, X, Y?  
 **A** : Theoretically it should support all valid **.cue/.bin** files, try it out.

**Q** : I am worried about the audio quality.  
 **A** : Vorbis and Opus can produce very good results even at 96kbps. If you just want to keep the games for personal use it is fine. **However** if you don't want any compressed audio you can select the **FLAC** or **TAK** codecs, which are lossless.

**Q**: Is storing the entire CD with FLAC/TAK really lossless? I am worried about byte integrity.  
 **A**: YES, to the last byte. The filesize and checksums of  the restored tracks are the same as the original ones.  (data&audio).

**Q** : Why there are two versions of the program?  
**A** : FFmpeg.exe is a small beast ( *around 36 megabytes*) and in some cases users already have it on their pc. So I am publishing a version with and a version without it.


## NodeJS Version

This project is a continuation of [**cdcrush nodeJS**](https://www.npmjs.com/package/cdcrush).  

**NEW MAY 2019** : The NodeJS version is now **FULLY COMPATIBLE** with the .Net version. So archives made with one version can be processed by the other.

> :stars: NodeJS**** version support **batches** , so you can restore/compress multiple CD with one command.

You can checkout **NodeJS** version over at NPM : https://www.npmjs.com/package/cdcrush

## :triangular_flag_on_post: Finally

Feel free to provide feedback for this project   
donations are always welcome! [https://www.paypal.me/johndimi](https://www.paypal.me/johndimi)  
Thanks for checking this out,  
John.