using System.IO;
using System.Text.RegularExpressions;

namespace cdcrush.lib.app {



/// <summary>
/// 
/// FreeArc Wrapper
/// 
///		"onComplete" will be called automatically when the operation ends
///		"onProgress" callbacks progress percent
/// 
/// </summary>
class FreeArc : AbArchiver
{
	const string EXECUTABLE_NAME = "Arc.exe";

	// --
	public FreeArc(string exePath = "")
	{
		SOLID = false;
		proc = new CliApp(Path.Combine(exePath,EXECUTABLE_NAME));

		proc.flag_stdout_word_mode = true;
		proc.onStdOutWord = onStdOutWordGetProgress;

		proc.onComplete = (code) =>
		{
			COMPRESSED_SIZE = 0;

			if(code>0)
			{
				ERROR = proc.stdErrLog;
				onComplete?.Invoke(false);
				return;
			}
			
			if(operation=="compress")
			{
				// Try to read the stdout string and get final size
				// 'Compressed 2 files, 127,707 => 120,363 bytes. Ratio 94.2%'
				var m = Regex.Match(proc.stdOutLog,@"=> (.*) bytes");
				if(m.Success)
				{
					var ss = m.Groups[1].Value.Replace(",",string.Empty);
					COMPRESSED_SIZE = long.Parse(ss);
				}		
			}

			onComplete?.Invoke(true);
		};

		
	}// -----------------------------------------

	
	/// <summary>
	/// FreeArc compression strings are kind of complicated since every 
	/// compression level includes a "fast decompression" option
	/// </summary>
	/// <param name="level">1-9</param>
	/// <returns></returns>
	public string getCompressionString(int level)
	{
		if(level<1) level=1; else if(level>9) level=9;
		return $"-m{level}";
	}// -----------------------------------------

	/// <summary>
	/// Compress a bunch of files into an archive
	/// NOTE: FREEARC has a thing and all the input files need to be on the same folder.
	/// </summary>
	/// <param name="listOfFiles">ALL files must be in the same directory!!!</param>
	/// <param name="destinationFile">Final archive filename</param>
	/// /// <param name="compressionLevel">0-10 Set this or set compressionString Directly</param>
	/// <param name="compressionString">a Valid Compression String for FreeArc. E.g. "-m4x -md32m"</param>
	/// <returns>Return Preliminary Success</returns>
	public override bool compress(string[] listOfFiles,string destinationFile, int compressionLevel = -1, string compressionString=null)
	{
		operation = "compress";
		progress = 0;
		flag_is_capturing = false;
		capt_off_switch = "Compressed";

		foreach(string f in listOfFiles) {
				if (!File.Exists(f)) {
					ERROR = string.Format("File '{0}' does not exist",f); return false;
				}
			}

		var sourceFolder = Path.GetDirectoryName(listOfFiles[0]);

		var fstrFiles = "";
		foreach(var s in listOfFiles) fstrFiles += "\"" + Path.GetFileName(s) + "\" ";

		var fstrFull = "";
		foreach(var s in listOfFiles) fstrFull += "\"" + s + "\" ";
		
		if(compressionLevel<0)
		{
			compressionString = getCompressionString(compressionLevel);
		}

		LOG.log("[ARC] : Compressing {0} into '{1}' with compression '{2}'", fstrFiles, destinationFile, compressionString);

		// -md32m is dictionary size -- removed since 1.2.3
		// -m4 is the default compression
		// -s	= Solid Compression, To merge all files in one solid block
		// -s-  = NON solid
		// -i1	= Display progress info only
		// -mt0 = Automatic use of threads
		// -ep	= Don't store paths in files

		string solidSTR = SOLID?"-s":"-s-";

		//proc.start($"a {compressionString} {solidSTR} -mt0 -i1 --diskpath=\"{sourceFolder}\" \"{destinationFile}\" {filesStr}");
		proc.start($"a {compressionString} {solidSTR} -mt0 -i1 -ep \"{destinationFile}\" {fstrFull}");

		return true;
	}// -----------------------------------------

	/// <summary>
	/// Extract `Input file` to `Destination Folder`
	/// If `filesToExtract` is set, then will try to extract these files only from withing the archive
	/// </summary>
	/// <param name="inputFile"></param>
	/// <param name="destinationFolder"></param>
	/// <param name="filesToExtract"></param>
	/// <returns></returns>
	public override bool extract(string inputFile, string destinationFolder=null, string[] filesToExtract = null)
	{
		operation = "extract";
		progress = 0;
		flag_is_capturing = false;
		capt_off_switch = "Extracted";

		if(destinationFolder==null) {
			destinationFolder = Path.GetDirectoryName(inputFile);
		}

		if(filesToExtract!=null)
		{
			// -- EXTRACT SELECTED FILES
			string fstr = "";
			foreach(string s in filesToExtract) {
				fstr +="\"" + s + "\" ";	// Include in double quotes and a space at the end
			}

			LOG.log("[ARC] : Extracting from '{0}' files:[{2}] into '{1}'", inputFile, destinationFolder, fstr);
			proc.start($"e -o+ -i1 -mt0 -dp\"{destinationFolder}\" \"{inputFile}\" {fstr}");

		}else
		{
			// -- EXTRACT ALL
			LOG.log("[ARC] : Extracting '{0}' into '{1}'", inputFile, destinationFolder);
			// Actual extract ::
			// -o+ = Overwrite
			// -i1 = Display progress info only
			proc.start($"e -o+ -i1 -mt0 -dp\"{destinationFolder}\" \"{inputFile}\"");
		}

		return true;

	}// ------


	/// <summary>
	/// Append a bunch of files in an archive. These new files can then be extracted faster.
	/// </summary>
	/// <param name="files"></param>
	/// <param name="archive"></param>
	public override bool append(string archive,string[] files)
	{
		operation = "append";
		if(!File.Exists(archive)) {
			ERROR = "Archive doesn't exist " + archive;
			return false;
		}

		string filesStr = "";
		foreach(string s in files){
			if(s!=null)
			filesStr +="\"" + Path.GetFileName(s) + "\" ";	// Include in double quotes and a space at the end
		}

		LOG.log("[ARC] : Appending {0} into '{1}'", filesStr, archive);

		proc.start(string.Format("a --diskpath=\"{2}\" \"{0}\" {1} --append", archive, filesStr,Path.GetDirectoryName(files[0])));
		return true;

	}// -----------------------------------------


	// CAPTURING STDOUT WORD BY WORD
	// -------------------------------


	// Read words
	string capt_on_switch = "Processed"; // Same for all operations
	string capt_off_switch; // "Compressed" or "Extracted".
	bool flag_is_capturing; // Always start with false
	// --
	private void onStdOutWordGetProgress(string word)
	{
		if(flag_is_capturing)
		{
			if(word==capt_off_switch){
				flag_is_capturing = false;
				return;
			}
		}else
		{
			if(word==capt_on_switch){
				flag_is_capturing = true;
			}

			return; // Either not capturing, or just began to capture
		}

		// Is capturing ::
		// STDOUT example:
		//		`Compressing 1 file, 143,556,608 bytes. Processed  35.1%`
		// word can be like "23.4%" OR  "25%". so..
		float fp;
		if(float.TryParse(word.Replace(".","%").Split('%')[0],out fp)) // Keep the first digits up until . or %
		{
			int p = (int)System.Math.Round(fp);
			if(p>progress) progress = p;
		}
	}// -----------------------------------------

}// -- end class
}// -- end namespace
