using cdcrush.lib;
using cdcrush.lib.app;
using cdcrush.lib.task;
using System;
using System.IO;

namespace cdcrush.prog
{

	/**
	 * CDCRUSH.cs is the main engine of the program,
	 * offering tools to compress/restore/get infos/etc.
	 * ----------------------------------------------------
	 */
	 
	public static class CDCRUSH
	{
		// -- Program Infos
		public const string AUTHORNAME = "John Dimi";
		public const string PROGRAM_NAME = "CDCRUSH";
		public const string PROGRAM_VERSION = "1.2.3";
		public const string PROGRAM_SHORT_DESC = "Highy compress cd-image games";
		public const string LINK_DONATE = "https://www.paypal.me/johndimi";
		public const string LINK_SOURCE = "https://github.com/johndimi/cdcrush.net";
		public const string CDCRUSH_SETTINGS = "crushdata.json";
		public const string CDCRUSH_COVER = "cover.jpg";
		public const string CDCRUSH_EXTENSION = ".arc";


		public const string RESTORED_CUE_FOLDER_SUFFIX = " (r)";

		// -- Global

		// Keep temporary files, don't delete them
		// Currently for debug builds only
		public static bool FLAG_KEEP_TEMP = false;

		// Maximum concurrent tasks in CJobs
		public static int MAX_TASKS = 2;

		// FFmpeg executable name
		const string FFMPEG_EXE = "ffmpeg.exe";

		// Location of `ffmpeg.exe` null for global path
		public static string FFMPEG_PATH {get; private set;}

		// Is FFMPEG ready to go?
		public static bool FFMPEG_OK {get; private set;}

		// Relative directory for the external tools (Arc, EcmTools)
		public static string TOOLS_PATH {get; private set;}

		// This is the GLOBAL temp folder used for ALL operations
		public static string TEMP_FOLDER {get; private set;}

		// Is the TEMP_FOLDER the cdcrush default, or did user alter it
		public static bool TEMP_FOLDER_IS_DEF {get; private set;}

		// General use Error Message, read this to get latest errors from functions
		public static string ERROR { get; private set; }

		// Lock any user interaction with the engine, allow only one operation at a time
		// NOTE: This is temporary, as more than one jobs are more than capable to exist
		public static bool LOCKED { get; private set; }

		// In addition to the completion callbacks, set this to get status reports
		// about the progress of each job.
		// #USERSET
		public static Action<CJobStatus, CJob> jobStatusHandler; 
		
		// The temp folder name to create under `TEMP_FOLDER`
		// No other program in the world should have this unique name, right?
		private const string TEMP_FOLDER_NAME = "CDCRUSH_361C4202-25A3-4F09-A690";

		// --
		private static bool isInited = false;
		// -----------------------------------------

		// :: AUDIO QUALITY ::

		// Audio codecs as they appear on the form controls
		public static readonly string[] AUDIO_CODECS = {
			"FLAC",
			"Ogg Vorbis",
			"Ogg Opus"
		};

		// The quality options for encoding with OPUS OGG
		public static readonly int[] OPUS_QUALITY = { 32, 48, 64, 80, 96, 112, 128, 160, 320};

		// The quality options for encoding with VORBIS OGG
		// Ogg vorbis Quality Number to kbps.
		public static readonly int[] VORBIS_QUALITY = { 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, 500 };

		// -----------------------------------

		/// <summary>
		/// Init Variables program
		/// </summary>
		public static bool init()
		{
			if (isInited) return true;

			LOG.log("------------------");
			LOG.log("{0}, {1}" + Environment.NewLine + "{2}", PROGRAM_NAME, PROGRAM_VERSION, PROGRAM_SHORT_DESC); ;
			LOG.log("------------------");
			
			// - Set Temp Folder to default
			if (!setTempFolder()) return false;

			// - Check for FFMPEG, since it may not come with the program
			FFMPEG_PATH = "";
			FFMPEG_OK = false;
			setFFMPEGPath();

			// -
			#if DEBUG
				TOOLS_PATH = "../../../tools/";
			#else // release
				TOOLS_PATH = "tools";
			#endif

			ERROR = null; isInited = true; LOCKED = false;
			return true;
		}// -----------------------------------------

		/// <summary>
		/// Called on normal exit and forced exit
		/// </summary>
		public static void kill()
		{
			// - Check if anything is running and gracefully end it? 
			// - Running Jobs are gracefully killed by their destructors
		}// -----------------------------------------


		/// <summary>
		/// Sets and Checks a new FFMPEG PATH
		/// </summary>
		/// <param name="ffmpeg_path">Folder FFMPEG is in,</param>
		public static void setFFMPEGPath(string ffmpeg_path = "")
		{
			if(CliApp.exists(Path.Combine(ffmpeg_path,FFMPEG_EXE)))
			{
				FFMPEG_OK = true;
				FFMPEG_PATH = ffmpeg_path;
			}
		}// -----------------------------------------

		/// <summary>
		/// Set program global temp folder
		/// </summary>
		/// <param name="path"></param>
		public static bool setTempFolder(string path = null)
		{
			string TEST_FOLDER; bool isDef = path == null;

			if(isDef) path = Path.GetTempPath();

			try{
				TEST_FOLDER = Path.Combine(path, TEMP_FOLDER_NAME);
			}catch(ArgumentException){
				ERROR = "TempFolder : Invalid path"; return false;
			}

			if(!FileTools.createDirectory(TEST_FOLDER))
			{
				ERROR = "TempFolder : Can't create " + TEST_FOLDER;
				return false;
			}

			if(!FileTools.hasWriteAccess(TEST_FOLDER))
			{
				ERROR = "Temp Folder :: Don't have write access to " + TEST_FOLDER;
				return false;
			}

			// Final sets at the end, ensuring that temp folder is OK
			TEMP_FOLDER = TEST_FOLDER;
			LOG.log("TEMP FOLDER = " + TEMP_FOLDER);
			TEMP_FOLDER_IS_DEF = isDef;
			return true;
		}// -----------------------------------------

		
		/// <summary>
		/// Translate from an AudioSettings tuple to String Descriptor
		/// </summary>
		/// <param name="q">The index of the combobox. Or Quality passed to the crush job</param>
		public static string getAudioQualityString(Tuple<int,int> A)
		{
			string res = AUDIO_CODECS[A.Item1];
			
			switch(A.Item1)
			{
				case 0:	// FLAC;
					break;
				case 1: // VORBIS
					res += $" {VORBIS_QUALITY[A.Item2]}k Vbr";
					break;
				case 2: // OPUS
					res += $" {OPUS_QUALITY[A.Item2]}k Vbr";
					break;
			}

			return res;
		}// -----------------------------------------

		/// <summary>
		/// Convert from bin/cue to encoded audio/cue
		/// </summary>
		/// <param name="_Input">Input file, must be `.cue`</param>
		/// <param name="_Output">Output folder, If null, it will be same as input file folder</param>
		/// <param name="_Audio">Audio Quality to encode the audio tracks with.</param>
		/// <param name="_Title">Title of the CD</param>
		/// <param name="onComplete">Completed (completeStatus,final Size)</param>
		/// <returns></returns>
		public static bool startJob_ConvertCue(string _Input, string _Output, Tuple<int,int> _Audio, 
			string _Title, Action<bool,int> onComplete)
		{
			if (LOCKED) { ERROR="Engine is working"; return false; } 
			if (!FFMPEG_OK) { ERROR="FFmpeg is not set"; return false; }

			LOCKED = true;

			var par = new CrushParams();	// CovertCue shares params with Crush job
				par.inputFile = _Input;
				par.outputDir = _Output;
				par.audioQuality = _Audio;
				par.cdTitle = _Title;

			var j = new JobConvertCue(par);
				j.MAX_CONCURRENT = MAX_TASKS;
				j.onComplete = (s) =>{
					LOCKED = false;
					ERROR = j.ERROR[1];
					onComplete(s, 0); // Disregard final filesize
				};

				j.onJobStatus = jobStatusHandler;	// For status and progress updates
				j.start();

			return true;
		}// -----------------------------------------
		
		
		/// <summary>
		/// Compress a CD to output folder
		/// </summary>
		/// <param name="_Input">Input file, must be `.cue`</param>
		/// <param name="_Output">Output folder, If null, it will be same as input file folder</param>
		/// <param name="_Audio">Audio Quality to encode the audio tracks with.</param>
		/// <param name="_Cover">Cover Image to store in the archive</param>
		/// <param name="_Title">Title of the CD</param>
		/// <param name="onComplete">Completed (completeStatus,CrushedSize)</param>
		/// <returns></returns>
		public static bool startJob_CrushCD(string _Input, string _Output, Tuple<int,int> _Audio, 
			string _Cover, string _Title, int compressionLevel, Action<bool,int> onComplete)
		{
			if (LOCKED) { ERROR="Engine is working"; return false; } 
			if (!FFMPEG_OK) { ERROR="FFmpeg is not set"; return false; }

			LOCKED = true;

			var par = new CrushParams();
				par.inputFile = _Input;
				par.outputDir = _Output;
				par.audioQuality = _Audio;
				par.cover = _Cover;
				par.cdTitle = _Title;
				par.compressionLevel = compressionLevel;

			var j = new JobCrush(par);
				j.MAX_CONCURRENT = MAX_TASKS;
				j.onComplete = (s) => {
					LOCKED = false;
					ERROR = j.ERROR[1];
					if (s) {
						CueReader cd = (CueReader) j.jobData.cd;						
						onComplete(s,j.jobData.crushedSize); // Hack, send CDINFO and SIZE as well
					}
					else {
						onComplete(s, 0);
					}
				};

				j.onJobStatus = jobStatusHandler;	// For status and progress updates
				j.start();

			return true;
		}// -----------------------------------------


		/// <summary>
		/// RESTORE an arc file to target output folder
		/// </summary>
		/// <param name="_Input">Input file, Must be `.arc`</param>
		/// <param name="_Output">Output folder, If null, it will be same as input file folder</param>
		/// <param name="onComplete">(completeStatus)</param>
		/// <returns></returns>
		public static bool startJob_RestoreCD(string _Input, string _Output,
			bool flag_folder, bool flag_forceSingle, bool flag_encCue, Action<bool> onComplete)
		{
			// NOTE : JOB checks for input file
			if (LOCKED) { ERROR="Engine is working"; return false; } 
			if (!FFMPEG_OK) { ERROR="FFmpeg is not set"; return false; }

			LOCKED = true;

			var par = new RestoreParams {
				inputFile = _Input,     // Checked in the JOB
				outputDir = _Output,    // Checked in the JOB
				flag_folder = flag_folder,
				flag_forceSingle = flag_forceSingle,
				flag_encCue = flag_encCue 
			};

			var j = new JobRestore(par);
				j.MAX_CONCURRENT = MAX_TASKS;
				j.onComplete = (s) =>
				{
					LOCKED = false;
					ERROR = j.ERROR[1];
					onComplete(s);
				};

				j.onJobStatus = jobStatusHandler;	// For status and progress updates
				j.start();

			return true;
		}// -----------------------------------------



		/// <summary>
		/// Quickly load a CUE file, read it, check for validity and report back.
		/// Returns a customized object with some info
		/// </summary>
		/// <param name="cueFile"></param>
		/// <param name="onComplete"></param>
		/// <returns>null on ERROR, A valid info object otherwise</returns>
		public static object loadQuickCUE(string cueFile)
		{
			if (LOCKED) {
				ERROR = "LOCKED";
				return null;
			}

			if (!check_file_(cueFile,".cue")) return false;

			// Load the CUE file and try to parse it
			var cd = new CueReader();
			
			if(!cd.load(cueFile))
			{
				ERROR = cd.ERROR; return null;
			}
			
			var info = new
			{
				title = cd.CD_TITLE,
				size1 = cd.CD_TOTAL_SIZE,
				tracks = cd.tracks.Count
			};

			return info;

		}// -----------------------------------------


		/// <summary>
		/// Take a crushed archive and extract only the info file, Returns a customized object with some info
		/// </summary>
		/// <param name="arcFile"></param>
		/// <param name="onComplete">(null) if error</param>
		public static bool loadQuickInfo(string arcFile, Action<Object> onComplete)
		{
			if (LOCKED) {
				ERROR = "LOCKED";
				return false;
			}

			if (!check_file_(arcFile,CDCRUSH_EXTENSION)) return false;

			LOCKED = true;

			// Delete old files from previous quickInfos
			FileTools.tryDelete(Path.Combine(TEMP_FOLDER, CDCRUSH_SETTINGS));
			FileTools.tryDelete(Path.Combine(TEMP_FOLDER, CDCRUSH_COVER));

			var arc = new FreeArc(TOOLS_PATH);

			// --
			arc.onComplete = (success) =>
			{
				LOCKED = false;

				if(success) // OK
				{
					// Continue
					var cd = new CueReader();
					if(!cd.loadJson(Path.Combine(TEMP_FOLDER,CDCRUSH_SETTINGS)))
					{
						ERROR = cd.ERROR;
						onComplete(null);
						return;
					}

					var info = new
					{
						title = cd.CD_TITLE,
						size0 = (int) new FileInfo(arcFile).Length,
						size1 = cd.CD_TOTAL_SIZE,
						audio = cd.CD_AUDIO_QUALITY,
						tracks = cd.tracks.Count,
						md5 = cd.getFirstDataTrackMD5(),
						cover = Path.Combine(TEMP_FOLDER,CDCRUSH_COVER)
					};
					
					onComplete(info);

				}else
				{
					ERROR = arc.ERROR;
					onComplete(null);
				}

			};

			// : Actually extract
			arc.extractFiles(arcFile, new[] { CDCRUSH_SETTINGS, CDCRUSH_COVER },TEMP_FOLDER);

			return true;
		}// -----------------------------------------
	

		/// <summary>
		/// Check if file EXISTS and is of VALID EXTENSION 
		/// </summary>
		/// <param name="arcFile"></param>
		/// <returns></returns>
		public static bool check_file_(string file,string ext)
		{
			// --
			if(!File.Exists(file))
			{
				ERROR = "File does not exist , " + file;
				return false;
			}

			// --
			if(Path.GetExtension(file).ToLower() != ext)
			{
				ERROR = "File, not valid extension , " + file;
				return false;
			}

			return true;
		}// -----------------------------------------

	}// --
}// --
