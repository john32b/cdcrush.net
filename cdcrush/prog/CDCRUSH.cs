using cdcrush.lib;
using cdcrush.lib.app;
using cdcrush.lib.task;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cdcrush.prog
{

	/**
	 * About Progress Report:
	 * ----------------------
	 * Since this engine can only do ONE JOB AT A TIME. i.e. (restoring, compressing)
	 * it's ok to have a single listener for progress updates
	 * sending progress (0 to 100) or (unknown)
	 * sending short text status messages e.g. ("Compressing 1/10")
	 * ----------------------------------------------------------------------
	 **/
	 

	// -- MAIN CLASS OF THE CDCRUSH PROGRAM
	public static class CDCRUSH
	{
		// -- Program Infos
		public const string AUTHORNAME = "John Dimi";
		public const string PROGRAM_NAME = "CD Crush";
		public const string PROGRAM_VERSION = "1.2.0";
		public const string PROGRAM_SHORT_DESC = "Highy compress cd-image games";
		public const string WEB_SITE = "https://github.com/johndimi/cdcrush.net";
		public const string CDCRUSH_SETTINGS = "crushdata.json";
		public const string CDCRUSH_COVER = "cover.jpg";
		public const string CDCRUSH_EXTENSION = ".arc";
		// -- Globals
		public static int MAX_TASKS = 3;			// Maximum concurrent tasks on CJobs
		public const int QUALITY_DEFAULT = 1;		// Number passed to FFMPEG by defauls ( 0 - 10 )
		// --
		private static bool isInited = false;
		// This is the GLOBAL temp folder used for ALL operations
		public static string TEMP_FOLDER {get; private set;}
		// General use Error Message, read this to get latest errors from functions
		public static string ERROR { get; private set; }

		// Lock any user interaction with the engine, allow only one operation at a time
		// NOTE: This is temporary, as more than one jobs are more than capable to exist
		public static bool LOCKED { get; private set; }

		// In addition to the completion callbacks, set this to get status reports
		// about the progress of each job. It's non crucial
		public static Action<CJobStatus, CJob> jobStatusHandler;
		
		// The temp folder name to create under `TEMP_FOLDER`
		// No other program in the world should have this unique name, right?
		private const string TEMP_FOLDER_NAME = "CDCRUSH_361C4202-25A3-4F09-A690";
		
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

			// TODO: Check for ffmpeg, freearc and ecm/unecm
			isInited = true;
			ERROR = null;
			LOCKED = false;
			return true;
		}// -----------------------------------------


		/// <summary>
		/// Set program global temp folder, unique every time the program starts
		/// </summary>
		/// <param name="path"></param>
		public static bool setTempFolder(string path = null)
		{
			if(path == null) path = Path.GetTempPath();

			try{
				TEMP_FOLDER = Path.Combine(path, TEMP_FOLDER_NAME);
			}catch(ArgumentException){
				ERROR = "TempFolder : Invalid path"; return false;
			}

			if(!FileTools.createDirectory(TEMP_FOLDER))
			{
				ERROR = "TempFolder : Can't create " + TEMP_FOLDER;
				return false;
			}

			if(!FileTools.hasWriteAccess(TEMP_FOLDER))
			{
				ERROR = "Temp Folder :: Don't have write access to " + TEMP_FOLDER;
				return false;
			}

			LOG.log("TEMP FOLDER = " + TEMP_FOLDER);

			return true;
		}// -----------------------------------------

		

		/// <summary>
		/// Compress a CD to output folder
		/// </summary>
		/// <param name="_Input">Input file, must be `.cue`</param>
		/// <param name="_Output">Output folder, If null, it will be same as input file folder</param>
		/// <param name="onComplete">Completed OK(true,false)</param>
		/// <returns></returns>
		public static bool crushCD(string _Input, string _Output, int _Audio, string _Cover, string _Title,
			Action<bool,CueReader,int> onComplete)
		{
			// NOTE : JOB checks for input file
			if (LOCKED) { ERROR="LOCKED"; return false; } LOCKED = true;

			var par = new CrushParams();
				par.inputFile = _Input;
				par.outputDir = _Output;
				par.audioQuality = _Audio;
				par.cover = _Cover;
				par.cdTitle = _Title;

			var j = new JobCrush(par);
				j.MAX_CONCURRENT = MAX_TASKS;

				j.onComplete = (s) =>
				{
					LOCKED = false;
					ERROR = j.ERROR[1];
					if (s)
						onComplete(s, j.jobData.cd , j.jobData.crushedSize); // Hack, send CDINFO and SIZE as well
					else
						onComplete(s, null, 0);
				};

				j.onJobStatus = jobStatusHandler;	// For status and progress updates
				j.start();

			return true;
		}// -----------------------------------------


		/// <summary>
		/// RESTORE an arc file to output folder
		/// </summary>
		/// <param name="_Input">Input file, Must be `.arc`</param>
		/// <param name="_Output">Output folder, If null, it will be same as input file folder</param>
		/// <param name="onComplete">Completed OK(true,false)</param>
		/// <returns></returns>
		public static bool restoreARC(string _Input, string _Output,
			bool flag_folder, bool flag_forceSingle,
			Action<bool> onComplete)
		{
			// NOTE : JOB checks for input file
			if (LOCKED) { ERROR="LOCKED"; return false; } LOCKED = true;

			var par = new RestoreParams();
				par.inputFile = _Input;		// Checked in the JOB
				par.outputDir = _Output;	// Checked in the JOB
				par.flag_folder = flag_folder;					//
				par.flag_forceSingle = flag_forceSingle;		// SINGLE FILE

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
		/// <param name="onComplete"></param>
		/// <returns>0:Locked, 1:OK, -1:Error</returns>
		public static bool loadQuickInfo(string arcFile, Action<Object> onComplete)
		{
			if (LOCKED) {
				ERROR = "LOCKED";
				return false;
			}

			if (!check_file_(arcFile,CDCRUSH_EXTENSION)) return false;

			LOCKED = true;

			// Move old files out of the way
			FileTools.tryDelete(Path.Combine(TEMP_FOLDER, CDCRUSH_SETTINGS));
			FileTools.tryDelete(Path.Combine(TEMP_FOLDER, CDCRUSH_COVER));

			var arc = new FreeArc();

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
						cd.tracks[0].md5,
						cover = Path.Combine(TEMP_FOLDER,CDCRUSH_COVER) // The file might be missing
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
		}
		// -----------------------------------------



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
