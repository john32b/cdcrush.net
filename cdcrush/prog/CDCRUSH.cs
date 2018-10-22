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
		public const string PROGRAM_NAME = "cdcrush";
		public const string PROGRAM_VERSION = "1.4.3";
		public const string PROGRAM_SHORT_DESC = "Highy compress cd-image games";
		public const string LINK_DONATE = "https://www.paypal.me/johndimi";
		public const string LINK_SOURCE = "https://github.com/johndimi/cdcrush.net";
		public const string CDCRUSH_SETTINGS = "crushdata.json";
		public const string CDCRUSH_COVER = "cover.jpg";
		public const string CDCRUSH_EXTENSION = ".arc";

		// When restoring a cd to a folder, put this at the end of the folder's name
		public const string RESTORED_FOLDER_SUFFIX = " (r)";

		public const int FREEARC_DEF_COMPRESSION_INDEX = 3; // This is the index on the form. Actual level is + 1

		// -- Global

		// Keep temporary files, don't delete them
		// Currently for debug builds only
		public static bool FLAG_KEEP_TEMP = false;

		// Maximum concurrent tasks in CJobs (default value)
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
		// NOTE: Also means that the engine is currently working
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

		// Hacky way of pushing number of expected tracks on the jobs.
		// MUST BE SET RIGHT BEFORE CREATING AJOB.
		// Until I implement progress reporting in a better way, this works fine.
		public static int HACK_CD_TRACKS = 0;
		// -----------------------------------------

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

			// -
			#if DEBUG
				TOOLS_PATH = "../../../tools/";
			#else // release
				TOOLS_PATH = "tools";
			#endif

			// - Check for FFMPEG, first in `tools` folder then in system path
			FFMPEG_PATH = "";
			FFMPEG_OK = false;
			setFFMPEGPath(TOOLS_PATH);
			if(!FFMPEG_OK)
			{
				setFFMPEGPath();
				if(FFMPEG_OK) LOG.log("+ FFMPEG on path");
			}else 
			{
				LOG.log("+ FFMPEG on `/tools` folder");
			}

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
		public static void setFFMPEGPath(string ffmpeg_path = null)
		{
			if(ffmpeg_path == null) ffmpeg_path = "";
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
			LOG.log("+ TEMP FOLDER = " + TEMP_FOLDER);
			TEMP_FOLDER_IS_DEF = isDef;
			return true;
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
		public static bool startJob_ConvertCue(string _Input, string _Output, Tuple<string,int> _Audio, 
			string _Title, Action<bool,int,cd.CDInfos> onComplete)
		{
			if (LOCKED) { ERROR="Engine is working"; return false; } 
			if (!FFMPEG_OK) { ERROR="FFmpeg is not set"; return false; }

			LOCKED = true;

			var par = new CrushParams {
				inputFile = _Input,
				outputDir = _Output,
				audioQuality = _Audio,
				cdTitle = _Title,
				expectedTracks = HACK_CD_TRACKS
			};  // CovertCue shares params with Crush job

			var j = new JobConvertCue(par);
				j.MAX_CONCURRENT = MAX_TASKS;
				j.onComplete = (s) =>{
					LOCKED = false;
					ERROR = j.ERROR[1];
					onComplete(s, 0, j.jobData.cd); // Disregard final filesize, because it's not an archive
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
		public static bool startJob_CrushCD(string _Input, string _Output, Tuple<string,int> _Audio, 
			string _Cover, string _Title, int compressionLevel, Action<bool,int,cd.CDInfos> onComplete)
		{
			if (LOCKED) { ERROR="Engine is working"; return false; } 
			if (!FFMPEG_OK) { ERROR="FFmpeg is not set"; return false; }

			LOCKED = true;

			// Set the running parameters for the Crush (compress) job
			var par = new CrushParams {
				inputFile = _Input,
				outputDir = _Output,
				audioQuality = _Audio,
				cover = _Cover,
				cdTitle = _Title,
				compressionLevel = compressionLevel,
				expectedTracks = HACK_CD_TRACKS
			};

			// Create the job and set it up
			var j = new JobCrush(par);
				j.MAX_CONCURRENT = MAX_TASKS;
				j.onComplete = (s) => {
					LOCKED = false;
					ERROR = j.ERROR[1];
					// Note: job.jobData is a general use object that was set up in the job
					//		 I can read it and get things that I want from it
					if (s) {
						onComplete(s,j.jobData.crushedSize, j.jobData.cd); // Hack, send CDINFO and SIZE as well
					}
					else {
						onComplete(s, 0, null);
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
				flag_encCue = flag_encCue,
				expectedTracks = HACK_CD_TRACKS
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
			var cd = new cd.CDInfos();
			try { 
				cd.cueLoad(cueFile);
			}catch(haxe.lang.HaxeException e) {
				ERROR = e.Message; return null;
			}

			LOG.log("= QuickLoaded `{0}' - [OK]", cueFile);

			var info = new
			{
				title = cd.CD_TITLE,
				size1 = cd.CD_TOTAL_SIZE,
				tracks = cd.tracks.length
			};

			return info;

		}// -----------------------------------------


		/// <summary>
		/// Take a crushed archive and extract only the info file, Returns a customized object with some info
		/// </summary>
		/// <param name="arcFile"></param>
		/// <param name="onComplete">(null) on error,
		///		OBJECT = 
		///			cd:CueReader,
		///			cover:Path of image cover or null
		///			sizeArc:Size of ARC in bytes
		///		
		///	</param>
		/// <returns>Success</returns>
		public static bool loadQuickInfo(string arcFile, Action<Object> onComplete)
		{
			if (LOCKED) {
				ERROR = "LOCKED";
				return false;
			}

			if (!check_file_(arcFile,CDCRUSH_EXTENSION)) return false;

			LOCKED = true;

			// Delete old files from previous quickInfos, IF ANY
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
					var cd = new cd.CDInfos();
					try{
						cd.jsonLoad(Path.Combine(TEMP_FOLDER,CDCRUSH_SETTINGS));
					}catch(haxe.lang.HaxeException e){
						ERROR = e.Message;
						onComplete(null);
						return;
					}

					// This is the object with the info returned to user
					var info = new
					{
						cd,
						sizeArc = (int) new FileInfo(arcFile).Length,
						cover = Path.Combine(TEMP_FOLDER,CDCRUSH_COVER)
					};
					
					LOG.log("= QuickLoaded `{0}` - [OK]",arcFile);
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

		/// <summary>
		/// Check if path exists and create it
		/// If it exists, rename it to a new safe name, then return the new name
		/// </summary>
		/// <returns></returns>
		public static string checkCreateUniqueOutput(string partA, string partB = "")
		{
			string path;

			// -
			try{
				path = Path.Combine(partA, partB);
			}catch(ArgumentException) {
				return null;
			}

			// Get unique path
			while(Directory.Exists(path)) {
				path = path + "_";
				LOG.log("! OutputFolder Exists, new name: {0}", path);
			}
			// Path now is unique
			if(!FileTools.createDirectory(path)) {
				return null;
			}
			// Path is created OK
			return path;
		}// -----------------------------------------

		/// Get a unique named temp folder ( inside the main temp folder )
		public static string getSubTempDir()
		{
			return Path.Combine(CDCRUSH.TEMP_FOLDER,Guid.NewGuid().ToString().Substring(0, 12));
		}// -----------------------------------------

	}// -- end class

}// -- end namespace
