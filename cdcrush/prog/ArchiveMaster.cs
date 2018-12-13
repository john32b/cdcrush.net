using System;
using System.IO;
using System.Linq;


namespace cdcrush.prog
{

/// <summary>
/// 
/// An Archive generalized class with common functions
/// and archiver infos [7-zip, Freearc]
/// - Form asks this class about valid compression templates
/// - Jobs use this to compress/restore an archive
/// 
/// </summary>
class ArchiveMaster
{
	/// <summary>
	/// This is read by the form to present the available compressions
	/// </summary>
	public static readonly string[] compressionStrings = new [] {
		"Zip Fast",
		"Zip Normal",
		"7-zip Fast",
		"7-zip Normal",
		"7-zip High",
		"FreeArc Fast",
		"FreeArc Normal",
		"FreeArc High"
	};

	public static readonly int DEFAULT_INDEX = 6;


	public static readonly string[] validExtensionsForArchives = new [] {".zip",".7z",".arc"};

	/// <summary>
	/// Based on what the user selected from `compressionStrings`
	/// RETURN [ArchiverID, CompressionString]
	/// </summary>
	/// <param name="settingsIndex">0-compressionStrings.length</param>
	/// <returns></returns>
	public static Tuple<string,string> getCompressionSettings(int settingsIndex)
	{
		if(settingsIndex<0) settingsIndex = 0; else
		if(settingsIndex>=compressionStrings.Count()) settingsIndex = compressionStrings.Count()-1;

		switch (settingsIndex)
		{
			case 0 : return Tuple.Create("zip","-mx1");	// fast
			case 1 : return Tuple.Create("zip","-mx6");	// normal

			case 2 : return Tuple.Create("7z","-mx4");	// fast
			case 3 : return Tuple.Create("7z","-mx6");	// normal
			case 4 : return Tuple.Create("7z","-mx7");	// high

			case 5 : return Tuple.Create("arc","-m3"); // fast
			case 6 : return Tuple.Create("arc","-m4"); // normal
			case 7 : return Tuple.Create("arc","-m5x"); // high
		}

		// ERROR, never going to happen
		return Tuple.Create("","");
	}


	/// <summary>
	/// Return a proper archiver object based on filenmae 
	/// </summary>
	/// <param name="file"></param>
	/// <returns></returns>
	public static lib.app.AbArchiver getArchiver(string file)
	{
		switch(Path.GetExtension(file).ToLower())
		{
			case ".zip":
			case ".7z": return new lib.app.SevenZip(CDCRUSH.TOOLS_PATH);
			case ".arc": return new lib.app.FreeArc(CDCRUSH.TOOLS_PATH);
		}

		return null;
	}// --

}// end class
}// end namespace 
