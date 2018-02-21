# CDCRUSH dot NET

**Version:** 1.2, **Platform:** Windows
**Author:** John Dimi, <johndimi@outlook.com>, twitter:[@jondmt](https://twitter.com/jondmt)
**Language:** C# NET 4.5, **Licence:** MIT

**NOTE** This repo is in development and rough around the edges, will make better in future commits.

## What is it

**Cdcrush** is a tool to highly compress **CD based games** *( playstation 1, Pc-engine, etc )* It works especially well with games that feature **CD-DA** audio. Used for storing and sharing, while saving space and bandwidth.

 **Cdcrush**  finds the audio tracks of the cd, and compresses them with modern audio codecs ( currently **ogg vorbis** ) resulting in tiny filesizes.  **ALSO** it removes **redundant** data from the cd **data** tracks *(with a tool called ECM tools, build in )* saving even more space.  **Finally** it compresses all those already compressed files in a single archive using the **FreeArc** archiver, 
 
### Some example of game sizes archived with file 7zip and cdcrush:

| Title             |  Raw    | 7z      | cdcrush |
| ----------------- | ------- | ------  | ------  |
| Wipeout 3         |  577mb  |  ~490mb  | **77mb**   |
| Die Hard Trilogy  |  532mb  |  ~450mb  | **71mb**   |
| Alien Trilogy     |  507mb  |  ~440mb  | **88mb**   |

<sup>Those games feature many CD-DA audio tracks</sup>



**CDCRUSH** can then  **restore** the crushed cd image back to it's original form, a **.bin/.cue** image that is ready to be used however you like.


## BUILDING

 - Visual Studio 2017
 - `ecm.exe` on path, or program folder
 - `unecm.exe` on path, or program folder
 - `Arc.exe` on path, or program folder
 - `ffmpeg` on path, or program folder

 > All of the above external programs will be included in later commits.



## CHANGELOG

### V1.2
 - First **dotNet** release, continuing the project from the [NPM cdcrush 1.12](https://www.npmjs.com/package/cdcrush)  release
 - Calculate and display the **MD5** of the first data track, useful to quickly identify the validity of the CD
 - Ability to add an image cover to a cd archive. The image will then be displayed when restoring a CD.
 - MultiThreaded. Background FFmpeg and ECM operations can execute up to **three** instances at a time.



## About

Please excuse the roughness of the code.

Feel free to open an issue/suggestion and contact me on social media and email. :smile: Donations are always welcome!

[![Donate](https://www.paypalobjects.com/en_US/i/btn/btn_donate_LG.gif)](https://www.paypal.me/johndimi)

Thanks for checking this out,

John.
