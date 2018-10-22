using System;
using System.IO;

namespace cdcrush.lib.app {


/// <summary>
/// 
/// Tak audio encoder/decoder
/// 
///		"onComplete" will be called automatically when the operation ends.
///		"onProgress" doesn't work
/// 
/// </summary>
class Tak : IProcessStatus
{
	const string EXECUTABLE_NAME = "Takc.exe";
	private CliApp proc;

	// # USER SET ::
	public Action<int> onProgress  { get; set; }
	public Action<bool> onComplete { get; set; }
	public string ERROR {get; private set;}

	// --
	public Tak(string exePath = "")
	{
		proc = new CliApp(Path.Combine(exePath,EXECUTABLE_NAME));

		proc.onComplete = (code) =>
		{
			if (code == 0) {
				onComplete?.Invoke(true);
			}
			else
			{
				ERROR = proc.stdErrLog;
				onComplete?.Invoke(false);
			}
		};
	}// -----------------------------------------

	public void kill() => proc?.kill();

	/// <summary>
	/// Will encode a WAV file to TAK
	/// </summary>
	/// <param name="input">PAth of the `.wav` file</param>
	/// <param name="output">If ommited, it will be created on same dir as source file</param>
	public bool encode(string input,string output = "")
	{
		LOG.log("[TAK] : Encoding [{0}]",input);

		if(string.IsNullOrEmpty(output))
		{
			proc.start($"-e \"{input}\"");
		}else
		{
			proc.start($"-e \"{input}\" \"{output}\"");
		}
		return true;
	}// -----------------------------------------


	/// <summary>
	/// Restores a TAK file back to a WAV file
	/// </summary>
	/// <param name="input">Path of the `.tak` file</param>
	/// <param name="output">If ommited, it will be created on same dir as source file</param>
	public bool decode(string input,string output = "")
	{
		LOG.log("[TAK] : Decoding [{0}]",input);

		if(string.IsNullOrEmpty(output))
		{
			proc.start($"-d \"{input}\"");
		}else
		{
			proc.start($"-d \"{input}\" \"{output}\"");
		}
		return true;
	}// -----------------------------------------

	/// <summary>
	/// Take a WAV Stream and Convert it to a TAK file
	/// Callbacks the `onReady` when it is ready to receive stdIn data
	/// </summary>
	/// <param name="output"></param>
	/// <param name="onReady"></param>
	/// <returns></returns>
	public bool encodeFromStream(string output,Action<Stream> onReady)
	{
		// E.G : c:\temp\TAK\Takc.exe -e -ihs - c:\temp\out.TAK
		proc.onStdIOReady = ()=> {
			onReady(proc.stdIn);
		};

		LOG.log("[TAK] : Encoding Stream to [{0}]",output);
		proc.start($"-e -ihs - \"{output}\"");
		return true;
	}// -----------------------------------------

	/// <summary>
	/// Decode a .TAK file and output to stdout (as WAV)
	/// Listen to Callback onReady and get stream from there
	/// </summary>
	/// <param name="input"></param>
	/// <param name="onReady"></param>
	/// <returns></returns>
	public bool decodeToStream(string input,Action<Stream> onReady)
	{
		//E.G : Takc.exe -d input.tak - | flac.exe -8 - -o output.flac
		proc.flag_disable_user_stdout = true;
		proc.onStdIOReady = ()=> {
			onReady(proc.stdOut);
		};
		LOG.log("[TAK] : Decoding [{0}] to Stream",input);
		proc.start($"-d \"{input}\" -");
		return true;
	}// -----------------------------------------
	
}// -- end class
}// -- end namespace