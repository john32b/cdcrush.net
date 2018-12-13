using System.IO;
using System.Text.RegularExpressions;

namespace cdcrush.lib.app
{

class SevenZip : AbArchiver
{
	const string EXECUTABLE_NAME = "7za.exe";

	public SevenZip(string exePath = "")
	{
		SOLID = false;
		proc = new CliApp(Path.Combine(exePath,EXECUTABLE_NAME));
		
		proc.onStdOut = capture_stdout_percent;

		proc.onComplete = (code) =>
		{
			COMPRESSED_SIZE = 0;
			proc.onStdOut = null;

			if(code>0)
			{
				ERROR = proc.stdErrLog;
				onComplete?.Invoke(false);
				return;
			}
	
			if(operation=="compress")
			{
				// Get archive size
				// Archive size: (\d+)
				// STDOUT Example : `..Add new data to archive: 1 file, 544971 bytes (533 KiB)Files read from disk: 1Archive size: 544561 bytes (532 KiB)Everything is Ok`
				var m = Regex.Match(proc.stdOutLog,@"Archive size: (\d+)",RegexOptions.IgnoreCase);
				if(m.Success) {		
					COMPRESSED_SIZE = long.Parse(m.Groups[1].Value);
				}
			}

			onComplete?.Invoke(true);
		};
	}// ---


	// Gets set on the COMPRESS operation to decode stdout and get progress
	// STDOUT Example:
	// 24% 13 + Devil Dice (USA).cue
	void capture_stdout_percent(string data)
	{
		var m = Regex.Match(data,@"(\d+)%");
		if(m.Success)
		{
			int p = int.Parse(m.Groups[1].Value);
			if(p>progress) progress = p;
		}

	}// -----------

	/// <summary>
	/// FreeArc compression strings are kind of complicated since every 
	/// compression level includes a "fast decompression" option
	/// </summary>
	/// <param name="level">1-9</param>
	/// <returns></returns>
	public string getCompressionString(int level)
	{
		if(level<1) level=1; else if(level>9) level=9;
		return $"-mx{level}";
	}// -----------------------------------------

	public override bool compress(string[] listOfFiles,string destinationFile, int compressionLevel=-1, string compressionString=null)
	{
		progress = 0;
		operation = "compress";

		foreach(string f in listOfFiles) {
				if (!File.Exists(f)) {
					ERROR = string.Format("File '{0}' does not exist",f); return false;
				}
			}

		var fstrFiles = "";
		foreach(var s in listOfFiles) fstrFiles += "\"" + Path.GetFileName(s) + "\" ";

		var fstrFull = "";
		foreach(var s in listOfFiles) fstrFull += "\"" + s + "\" ";
		
		if(compressionLevel<0)
		{
			compressionString = getCompressionString(compressionLevel);
		}

		LOG.log("[7ZIP] : Compressing {0} into '{1}' with compression '{2}'", fstrFiles, destinationFile, compressionString);


		string solidSTR = "";
		string str0;
		switch(Path.GetExtension(destinationFile).ToLower())
		{
			case ".zip": 
				// SOLID archives not compatible with .zip
				str0 = "-tzip"; 
				break;

			case ".7z": 
				solidSTR = SOLID?"-ms=on":"-ms=off";
				str0 = "-t7z"; 
				break;

			default: ERROR = "Unsupported file extension"; return false;
		}

		// -mmt		= Multithreaded
		// -bsp1	= Redirect PROGRESS outout to STDOUT <-- (this took me a while to find) 
		proc.start($"a {str0} -bsp1 -mmt {solidSTR} {compressionString} \"{destinationFile}\" {fstrFull}");

		return true;
	}// -------------------------------------

	/// <summary>
	/// Extract `Input file` to `Destination Folder`
	/// If `filesToExtract` is set, then will try to extract these files only from withing the archive
	/// </summary>
	/// <param name="inputFile"></param>
	/// <param name="destinationFolder"></param>
	/// <param name="filesToExtract"></param>
	/// <returns></returns>
	public override bool extract(string inputFile, string destinationFolder=null, string[] filesToExtract=null)
	{
		progress = 0;
		operation = "extract";

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

			// -aoa		= overwrite
			// -mmt		= multithread
			proc.start($"e \"{inputFile}\" -bsp1 -mmt -aoa -o\"{destinationFolder}\" {fstr}");

		}else
		{
			// -- EXTRACT ALL
			LOG.log("[ARC] : Extracting '{0}' into '{1}'", inputFile, destinationFolder);
			proc.start($"e \"{inputFile}\" -bsp1 -mmt -aoa -o\"{destinationFolder}\"");
		}

		return true;
	}// ------------
	
}// -- end class
}// -- end namespace
