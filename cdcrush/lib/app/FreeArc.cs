using System.IO;

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
		proc = new CliApp(Path.Combine(exePath,EXECUTABLE_NAME));
		proc.flag_stdout_word_mode = true;

		proc.onComplete = (code) =>
		{
			if (code == 0)
			{
				onComplete?.Invoke(true);
			}
			else
			{
				ERROR = proc.stdErrLog;
				onComplete?.Invoke(false);
			}
		};

		proc.onStdOutWord = onStdOutWordGetProgress;
	}// -----------------------------------------


	/// <summary>
	/// Compress a bunch of files into an archive
	/// </summary>
	/// <param name="listOfFiles">ALL files must be in the same directory!!!</param>
	/// <param name="destinationFile">Final archive filename</param>
	/// <returns>Return Preliminary Success</returns>
	public override bool compress(string[] listOfFiles,string destinationFile, int compressionLevel = 4)
	{
		progress = 0;
		flag_is_capturing = false;
		capt_off_switch = "Compressed";

		foreach(string f in listOfFiles) {
				if (!System.IO.File.Exists(f)) {
					ERROR = string.Format("File '{0}' does not exist",f); return false;
				}
			}

		var sourceFolder = Path.GetDirectoryName(listOfFiles[0]);

		var filesStr = "";
		foreach(var s in listOfFiles) filesStr += "\"" + Path.GetFileName(s) + "\" ";
		
		LOG.log("[ARC] : Compressing {0} into '{1}'", filesStr, destinationFile);

		if(compressionLevel<0) compressionLevel = 0;
		if(compressionLevel>9) compressionLevel = 9;

		// -md32m is dictionary size -- removed since 1.2.3
		// -m4 is the default compression
		// -s	= Solid Compression, To merge all files in one solid block
		// -i1	= Display progress info only
		proc.start(string.Format("a -m{3} -s -i1 -o+ --diskpath=\"{1}\" \"{0}\" {2}", destinationFile, sourceFolder, filesStr, compressionLevel));

		// NOTE: -m4 requires 128Mb for packing and unpacking
		return true;
	}// -----------------------------------------

	/// <summary>
	/// Extracts all files,ingores pathnames
	/// </summary>
	/// <param name="inputFile">File to extract</param>
	/// <param name="destinationFolder">Defaults to same folder as archive</param>
	/// <returns>Return Preliminary Success</returns>
	public override bool extractAll(string inputFile, string destinationFolder = null)
	{
		progress = 0;
		flag_is_capturing = false;
		capt_off_switch = "Extracted";

		if(destinationFolder==null) {
			destinationFolder = Path.GetDirectoryName(inputFile);
		}

		LOG.log("[ARC] : Extracting '{0}' into '{1}'", inputFile, destinationFolder);

		// Actual extract ::
		// -o+ = Overwrite
		// -i1 = Display progress info only
		proc.start(string.Format("e -o+ -i1 \"{0}\" -dp\"{1}\"", inputFile, destinationFolder));
		
		return true;
	}// -----------------------------------------

	/// <summary>
	/// Append a bunch of files in an archive
	/// </summary>
	/// <param name="files"></param>
	/// <param name="archive"></param>
	public bool appendFiles(string[] files, string archive)
	{
		if(!File.Exists(archive))
		{
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

	/// <summary>
	/// Extract selected files from an archive
	/// </summary>
	/// <param name="inputFile">The Archive to extract files from</param>
	/// <param name="listOfFiles">A list of files, FILES MUST EXIST in the archive</param>
	/// <param name="destinationFolder">Where to extract the files</param>
	/// <returns>Return Preliminary Success</returns>
	public override bool extractFiles(string inputFile, string[] listOfFiles, string destinationFolder = null)
	{		
		progress = 0;
		flag_is_capturing = false;
		capt_off_switch = "Extracted";

		if(destinationFolder==null) {
			destinationFolder = Path.GetDirectoryName(inputFile);
		}

		string filesStr = "";
		foreach(string s in listOfFiles){
			filesStr +="\"" + s + "\" ";	// Include in double quotes and a space at the end
		}

		LOG.log("[ARC] : Extracting '{0}' files:[{2}] into '{1}'", inputFile, destinationFolder,filesStr);

		proc.start(string.Format("e -o+ -i1 \"{0}\" {2} -dp\"{1}\"", inputFile, destinationFolder, filesStr));
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
		float fp;
		if(float.TryParse(word.Split('%')[0],out fp)) 
		{
			int p = (int)System.Math.Round(fp);
			if(p>progress) progress = p;
		}
	}// -----------------------------------------

}// -- end class
}// -- end namespace
