using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;


namespace cdcrush.lib.app
{

public class FFmpegCodec
{	
	public string name;				// Name of codec
	public string ID;				// Identifier
	public string ext;				// Codec Extension
	public string codecString;		// Codec String to put in ffmpeg argument
	public bool ignoreQuality = false;
	public string[] qualityArg;		// Quality argument to pass onto ffmpeg
									// If NULL, will take 0... as parameters ( up to qualityinfo0.length )
	public int[] qualityInfo;		// 1:1 map to qualityArg[]. Information on Quality Index
									// If qualityInfo0 NOT SET, then info = qualityARG + qualityInfoPost
	public string qualityInfoPost;	// If set, quality Info is qualityInfo0 + qualityInfoPost
	public int qualityDefault;		// default quality INDEX ( 0...N )
	
	// --
	private int sanitizeQuality(int q)
	{
		if(q<0) return 0;
		if(qualityArg!=null) {
			if(q>=qualityArg.Length) return qualityArg.Length-1;
		}else {
			if(qualityInfo!=null && q>=qualityInfo.Length) return qualityInfo.Length-1;
		}
		return q;
	}// ---------------

	/// <summary>
	/// Return the ffmpeg encode string with quality baked
	/// </summary>
	/// <param name="q"></param>
	/// <returns></returns>
	public string getCodecString(int q)
	{
		if(ignoreQuality) return codecString;
		q = sanitizeQuality(q);
		if(qualityArg==null) return $"{codecString}{q}"; // Quality + Q Index
		return $"{codecString}{qualityArg[q]}";
	}// ---------------

	// --
	public string[] getQualityInfos()
	{
		List<string> l = new List<string>();
		if(!ignoreQuality)
		{
			if(qualityInfo!=null)
			{
				foreach(var i in qualityInfo) l.Add(i + qualityInfoPost);
			}else
			{
				foreach(var i in qualityArg) l.Add(i + qualityInfoPost);
			}
		}

		return l.ToArray();
	}// ---------------

}




/// <summary>
/// Simple wrapper for FFmpeg
/// Currently just supports Audio Compression for use with the CDCRUSH project
/// 
///		"onProgress" => Reports percentage 0 to 100
///		"onComplete" => Exit code 0 for OK, other for ERROR
///	
/// </summary>
class FFmpeg:IProcessStatus
{
	const string EXECUTABLE_NAME = "ffmpeg.exe";
	private CliApp proc;

	// # USER SET ::
	public Action<int> onProgress  { get; set; }	// IProcessReport
	public Action<bool> onComplete { get; set; }	// IProcessReport
	public string ERROR {get; private set;}			// IProcessReport

	// Percentage Helpers
	int secondsConverted, targetSeconds;
	public int progress {get; private set;} // Current progress % of the current conversion

	// Supported codecs to use
	public static FFmpegCodec[] codecs;

	public static FFmpegCodec getCodecByID(string id) {
		return Array.Find(codecs,(s)=>s.ID==id);
	}

	// -----------------------------------------

	/// <summary>
	/// Constructor for the static things, this will only get called once
	/// </summary>
	static FFmpeg()
	{
		codecs = new[]
		{
			new FFmpegCodec() {
				name = "Vorbis", ID = "VORBIS", ext = ".ogg",
				qualityDefault = 3,
				// qualityArg not set, meaning it accepts 0->10 (info.length)
				qualityInfo = new [] { 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, 500 },
				qualityInfoPost = "k Vbr",
				codecString = "-c:a libvorbis -q " // string ends where it expects quality string
			},

			new FFmpegCodec() {
				name = "Opus", ID = "OPUS", ext = ".ogg",
				qualityDefault = 4,
				// qualityInfo not set, meaning final info = qualityArg + qualitInfoPost // e.g. 32k Vbr"
				qualityArg = new [] { "32k", "48k", "64k", "80k", "96k", "112k", "128k", "160k", "320k" },
				qualityInfoPost = " Vbr",
				codecString = "-c:a libopus -vbr on -compression_level 10 -b:a "
			},

			new FFmpegCodec() {
				name = "Mp3", ID = "MP3", ext = ".mp3",
				qualityDefault = 3,
				qualityArg = new [] { "9","8","7","6","5","4","3","2","1","0" },
				qualityInfo = new [] { 65, 85, 100, 115, 130, 165, 175, 190, 225, 245 },
				qualityInfoPost = " Vbr",
				codecString = "-c:a libmp3lame -q:a "
			},

			new FFmpegCodec() {
				name = "Flac lossless", ID = "FLAC", ext = ".flac",
				qualityDefault = 0,
				// No quality settings, means that quality is ignored
				codecString = "-c:a flac ",
				ignoreQuality = true
			}
		};
	}// -----------

	/// <summary>
	/// FFMPEG wrapper
	/// </summary>
	/// <param name="executablePath">Set the path of ffmpeg if not on path already</param>
	public FFmpeg(string executablePath = "")
	{
		proc = new CliApp(Path.Combine(executablePath,EXECUTABLE_NAME));

		proc.onComplete = (code) =>
		{
			if (code == 0) {
				onComplete?.Invoke(true);
			}
			else {
				ERROR = "Something went wrong with FFMPEG";
				onComplete?.Invoke(false);
			}
		};


		// -- FFMPEG writes Status to StdErr
		// Gets current operation progress (
		proc.onStdErr = (s) =>
		{
			if (targetSeconds == 0) return;
			secondsConverted = readSecondsFromOutput(s, @"time=(\d{2}):(\d{2}):(\d{2})");
			if (secondsConverted == -1) return; 

			progress = (int)Math.Ceiling(((double)secondsConverted / (double)targetSeconds) * 100f);
			// LOG.log("[FFMPEG] : {0} / {1} = {2}", secondsConverted, targetSeconds, progress);

			if (progress > 100) progress = 100;
			onProgress?.Invoke(progress);
		};

	}// -----------------------------------------

	// --
	public void kill() => proc?.kill();

	/// <summary>
	/// Read a file's duration, used for when converting to PCM
	/// </summary>
	/// <param name="file"></param>
	private int getSecondsFromFile(string input)
	{
		int i = 0;
		var s = CliApp.quickStartSync(proc.executable,$"-i \"{input}\" -f null -");
		if(s[2]=="0") // ffmpeg success 
		{
			i = readSecondsFromOutput(s[1], @"\s*Duration:\s*(\d{2}):(\d{2}):(\d{2})");
		}
		return i;
	}// -----------------------------------------


	/// <summary>
	/// Returns FFMPEG time to seconds. HELPER FUNCTION
	/// </summary>
	/// <param name="input">The string to check the regex</param>
	/// <param name="expression">Needs to be a regexp with 3 capture groups</param>
	/// <returns></returns>
	private int readSecondsFromOutput(string input,string expression)
	{
		var m = Regex.Match(input,expression);
		var seconds = -1;
		if(m.Success){
			var hh = int.Parse(m.Groups[1].Value);
			var mm = int.Parse(m.Groups[2].Value);
			var ss = int.Parse(m.Groups[3].Value);
			seconds = (ss + (mm * 60) + (hh * 360));
		}
		return seconds;
	}// -----------------------------------------



	/// <summary>
	/// Encode a PCM file with an ENCODER / QUALITY combo to another File
	/// </summary>
	/// <param name="encoderID">OPUS, VORBIS, FLAC, MP3</param>
	/// <param name="quality">Use -1 for default, Starts from 0 ... MAX = dependent on codec </param>
	/// <param name="input">Input filepath to encode </param>
	/// <param name="output">Output path, if not set will create a file at same folder as input file</param>
	/// <returns></returns>
	public bool encodePCM(string encoderID, int quality, string input, string output = null)
	{
		var cod = getCodecByID(encoderID);

		if(cod == null) { 
			ERROR = $"CodecID {encoderID} does not exist";
			return false;
		}
		if(quality==-1) quality = cod.qualityDefault;

		if(string.IsNullOrEmpty(output)) {
			output = Path.ChangeExtension(input,cod.ext);
		}else{
			//[safeguard] Make sure it is valid extension
			if(!output.ToLower().EndsWith(cod.ext)) {
				output += cod.ext; // try to fix it
			}
		}
		
		// Init progress variables:
		var fsize = (int)new FileInfo(input).Length;
		secondsConverted = progress = 0;
		targetSeconds = (int)Math.Floor((double)fsize / 176400); // PCM is 176400 bytes per second

		// -
		LOG.log("[FFMPEG] : Encoding [{0}] with {1} , {2}",input, cod.name, cod.getCodecString(quality));
		proc.start($"-y -f s16le -ar 44.1k -ac 2 -i \"{input}\" {cod.getCodecString(quality)} \"{output}\"");
		
		return true;
	}// ---------------


	/// <summary>
	/// Takes a STREAM and encodes to a file
	/// - callback onPipeReady() will fire with the Stream
	/// - write to that stream with PCM Data
	/// - Don't forget to close the STREAM once you've done writing to it
	/// </summary>
	/// <returns></returns>
	public bool encodePCMStream(string encoderID, int quality, string output, Action<Stream> onIOReady)
	{
		var cod = getCodecByID(encoderID);
		if(cod == null) return false;
		if(quality==-1) quality = cod.qualityDefault;
		if(!output.ToLower().EndsWith(cod.ext)) 
		{
			output += cod.ext; // try to fix extension if not already set
		}

		proc.onStdIOReady = () => 
		{
			onIOReady(proc.stdIn);
		};

		LOG.log("[FFMPEG] : Encoding PCM STREAM [{0}] with {1} , {2}",output, cod.name, cod.getCodecString(quality));
		proc.start($"-y -f s16le -ar 44.1k -ac 2 -i pipe:0 {cod.getCodecString(quality)} \"{output}\"");

		return true;
	}// -----------------------------------------


	/// <summary>
	/// Take a PCM Stream, and push it to another STREAM as WAV 
	/// - callback onPipeReady() will fire with the Streams
	/// - Don't forget to close the inStream once you've done writing to it
	/// </summary>
	/// <param name="input"></param>
	/// <param name="onIOReady">(thisGetsData, thisPushesData)</param>
	public void convertPCMStreamToWavStream(Action<Stream,Stream> onIOReady)
	{
		proc.onStdIOReady = () => 
		{
			onIOReady(proc.stdIn,proc.stdOut);
		};

		proc.flag_disable_user_stdout = true;
		LOG.log("[FFMPEG] : Converting PCM Stream to WAV Stream");
		proc.start($"-f s16le -ar 44.1k -ac 2 -i pipe:0 -f wav -");
	}// -----------------------------------------

	/// <summary>
	/// Take a WAV stream, and save it as a PCM File
	/// </summary>
	/// <param name="output"></param>
	/// <param name="onReady"></param>
	/// <returns></returns>
	public bool convertWavStreamToPCM(string output,Action<Stream> onReady)
	{
		proc.onStdIOReady = ()=> {
			onReady(proc.stdIn);
		};
		// note -y overwrites output file
		LOG.log("[FFMPEG] : Converting STDIN to PCM file {0}", output);
		proc.start($"-f wav -i pipe:0 -y -f s16le -acodec pcm_s16le {output}");
		return true;
	}// -----------------------------------------



	/// <summary>
	/// Convert an audio file to a PCM file for use in a CD audio
	/// ! Does not check INPUT file !
	/// ! Overwrites all generated files !
	/// </summary>
	/// <param name="input"></param>
	/// <param name="output">If ommited, will be automatically set</param>
	/// <returns></returns>
	public bool audioToPCM(string input,string output = null)
	{
		if(string.IsNullOrEmpty(output)) {
			output = Path.ChangeExtension(input,"pcm");
		}

		// Prepare progress variables
		secondsConverted = progress = 0;
		targetSeconds = getSecondsFromFile(input);

		LOG.log("[FFMPEG] : Converting \"{0}\" to PCM", input);

		proc.start($"-i \"{input}\" -y -f s16le -acodec pcm_s16le \"{output}\"");

		return true;
	}// -----------------------------------------



}// -- end class
}// --