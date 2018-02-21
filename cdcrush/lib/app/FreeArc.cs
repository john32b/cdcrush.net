using System.Diagnostics;
using System.IO;

namespace cdcrush.lib.app {



/// <summary>
/// 
/// FreeArc Wrapper
/// 
///		"onComplete" will be called automatically whenever the operation ends
///		"onProgress" is BROKEN
/// 
/// </summary>
class FreeArc : AbArchiver
{
	public FreeArc(string toolsPath = "")
	{
		proc = new CliApp(Path.Combine(toolsPath,"Arc.exe"));

		proc.onComplete = (code) =>
		{
			if (code == 0)
			{
				if (onComplete != null) onComplete(true);
			}
			else
			{
				ERROR = proc.stdErrLog;
				onComplete(false);
			}
		};

		// FREEARC writes to stdout
		proc.onStdOut = (s) =>
		{
			// Try to read the progress, but can't
		};

	}// -----------------------------------------


	/// <summary>
	/// Compress a bunch of files into an archive
	/// </summary>
	/// <param name="listOfFiles">ALL files must be in the same directory!!!</param>
	/// <param name="destinationFile">Final archive filename</param>
	/// <returns>Return Preliminary Success</returns>
	public override bool compress(string[] listOfFiles,string destinationFile)
	{
		progress = 0;
		foreach(string f in listOfFiles) {
				if (!System.IO.File.Exists(f)) {
					ERROR = string.Format("File '{0}' does not exist",f); return false;
				}
			}

		var sourceFolder = Path.GetDirectoryName(listOfFiles[0]);

		var filesStr = "";
		foreach(var s in listOfFiles) filesStr += "\"" + Path.GetFileName(s) + "\" ";
		
		LOG.log("[ARC] : Compressing {0} into '{1}'", filesStr, destinationFile);
		proc.start(string.Format("a -m4 -md32m -s -o+ --diskpath=\"{1}\" \"{0}\" {2}", destinationFile, sourceFolder, filesStr));
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
		if(destinationFolder==null) {
			destinationFolder = Path.GetDirectoryName(inputFile);
		}

		LOG.log("[ARC] : Extracting '{0}' into '{1}'", inputFile, destinationFolder);

		// Actual extract ::
		proc.start(string.Format("e -o+ -i0 \"{0}\" -dp\"{1}\"", inputFile, destinationFolder));
		
		return true;
	}// -----------------------------------------

	/// <summary>
	/// Append a bunch of files in an archive
	/// </summary>
	/// <param name="files"></param>
	/// <param name="archive"></param>
	/// <returns></returns>
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
		if(destinationFolder==null) {
			destinationFolder = Path.GetDirectoryName(inputFile);
		}

		string filesStr = "";
		foreach(string s in listOfFiles){
			filesStr +="\"" + s + "\" ";	// Include in double quotes and a space at the end
		}

		LOG.log("[ARC] : Extracting '{0}' files:[{2}] into '{1}'", inputFile, destinationFolder,filesStr);

		proc.start(string.Format("e -o+ \"{0}\" {2} -dp\"{1}\"", inputFile, destinationFolder, filesStr));
		return true;
	}// -----------------------------------------


}// -- end class
}// -- end namespace
