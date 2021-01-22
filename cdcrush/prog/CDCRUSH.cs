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
	 
	static class CDCRUSH
	{
		// -- Program Infos
		public const string AUTHORNAME = "John32B";
		public const string PROGRAM_NAME = "cdcrush";
		public const string PROGRAM_VERSION = "1.5";
		public const string PROGRAM_SHORT_DESC = "Highy compress cd-image games";
		public const string LINK_DONATE = "https://www.paypal.me/johndimi";
		public const string LINK_SOURCE = "https://github.com/john32b/cdcrush.net";
		public const string CDCRUSH_SETTINGS = "crushdata.json";
		public const string CDCRUSH_COVER = "cover.jpg";
		public static readonly string[] CDCRUSH_EXTENSIONS = new [] {".zip",".7z",".arc"};

		// When restoring a cd to a folder, put this at the end of the folder's name
		public const string RESTORED_FOLDER_SUFFIX = " (r)";

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
		/// Starts either a CRUSH or CONVERT Job
		/// - Crush will encode all tracks and then create a .CRUSHED archive that can be restored lates
		/// - Convert will just encode all audio tracks and create new .CUE/.BIN/.ENCODED AUDIO FILES
		/// - [CONVERT] can create files in a folder or inside an Archive
		/// 
		/// </summary>
		/// <param name="_Mode">0:Crush, 1:Convert, 2:Convert + ARCHIVE</param>
		/// <param name="_Input">Must be a valid CUE file </param>
		/// <param name="_Output">Output Directory - IF EMPTY will be same dir as INPUT</param>
		/// <param name="_Audio">Audio Settings TUPLE. Check `AudioMaster`</param>
		/// <param name="_ArchSet">Archive Settings Index. Check `ArchiveMaster` -1 for no archive</param>
		/// <param name="_Title">Name of the CD</param>
		/// <param name="_Cover">Path of Cover Image to store in the archive - CAN BE EMPTY</param>
		/// <param name="onComplete">Complete Calback (completeStatus,jobData object)</param>
		/// <returns>Preliminary Success</returns>
		public static bool startJob_Convert_Crush(
			int _Mode,
			string _Input, 
			string _Output,
			Tuple<string,int> _Audio,
			int _ArchSet,
			string _Title,
			string _Cover,
			Action<bool,CrushParams> onComplete)
		{
			if (LOCKED) { ERROR="Engine is working"; return false; } 
			if (!FFMPEG_OK) { ERROR="FFmpeg is not set"; return false; }
			LOCKED = true;

			// Set the running parameters for the job
			var par = new CrushParams {
				inputFile = _Input,
				outputDir = _Output,
				audioQuality = _Audio,
				cover = _Cover,
				cdTitle = _Title,
				archiveSettingsInd = _ArchSet,
				expectedTracks = HACK_CD_TRACKS,
				mode = _Mode
			};

			var job = new JobCrush(par);
			
			job.MAX_CONCURRENT = MAX_TASKS;
			job.onJobStatus = jobStatusHandler;		// For status and progress updates, FORM sets this.
			job.onComplete = (s) => {
					LOCKED = false;
					ERROR = job.ERROR[1];
					onComplete(s, job.jobData);
				};

			job.start();

			return true;
		}

		

		/// <summary>
		/// RESTORE An Archive file to target output folder
		/// </summary>
		/// <param name="_Input">Input file, Must be `.arc`</param>
		/// <param name="_Output">Output folder, If null, it will be same as input file folder</param>
		/// <param name="_Flag_Folder">Extract files to a subfolder of `_output`</param>
		/// <param name="_Mode">0: Normal, 1:Merge, 2:Restore To EncAudio</param>
		/// <param name="onComplete">(completeStatus)</param>
		/// <returns>Preliminary  Success</returns>
		public static bool startJob_RestoreCD(
			string _Input, 
			string _Output,
			bool _Flag_Folder, 
			int _Mode,
			Action<bool> onComplete)
		{
			// NOTE : JOB checks for input file
			if (LOCKED) { ERROR="Engine is working"; return false; } 
			if (!FFMPEG_OK) { ERROR="FFmpeg is not set"; return false; }

			LOCKED = true;

			var par = new RestoreParams {
				inputFile = _Input,     // Checked in the JOB
				outputDir = _Output,    // Checked in the JOB
				flag_folder = _Flag_Folder,
				mode = _Mode,
				expectedTracks = HACK_CD_TRACKS
			};

			var j = new JobRestore(par);
				j.MAX_CONCURRENT = MAX_TASKS;
				j.onJobStatus = jobStatusHandler;	// For status and progress updates
				j.onComplete = (s) =>
				{
					LOCKED = false;
					ERROR = j.ERROR[1];
					onComplete(s);
				};

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

			if (!check_file_(arcFile,CDCRUSH_EXTENSIONS)) 
			{
				ERROR = "Invalid file extension. Supported  = (" + string.Join(" ",CDCRUSH.CDCRUSH_EXTENSIONS) + ")";
				return false;
			}


			LOCKED = true;

			// Delete old files from previous quickInfos, IF ANY
			FileTools.tryDelete(Path.Combine(TEMP_FOLDER, CDCRUSH_SETTINGS));
			FileTools.tryDelete(Path.Combine(TEMP_FOLDER, CDCRUSH_COVER));
			
			var arc = ArchiveMaster.getArchiver(arcFile);

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
						ERROR = "Not a valid CDCRUSH Archive.";
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


			// TODO: Make sure ARCHIVE contains proper files
			// : Actually extract
			arc.extract(arcFile,TEMP_FOLDER,new [] {CDCRUSH_SETTINGS, CDCRUSH_COVER});

			return true;
		}// -----------------------------------------
	

		/// <summary>
		/// Check if file EXISTS and is of VALID EXTENSION 
		/// </summary>
		/// <param name="arcFile"></param>
		/// <returns></returns>
		public static bool check_file_(string file,params string[] exts)
		{
			// --
			if(!File.Exists(file))
			{
				ERROR = "File does not exist , " + file;
				return false;
			}

			// --
			
			foreach(var ext in exts)
			{
				if(Path.GetExtension(file).ToLower() == ext)
				{
					return true;
				}
			}

			ERROR = "File, not valid extension , " + file;
			return false;
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
