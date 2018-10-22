using System;

using cdcrush.lib.task;
using System.IO;
using cdcrush.lib;
using cdcrush.lib.app;

namespace cdcrush.prog
{

// -
// Every Restore Job runs with these input parameters
public struct RestoreParams
{	
	//
	public string inputFile;		// The file to restore the CDIMAGE from
	public string outputDir;		// Output Directory. Will change to subfolder if `flag_folder`
	public bool flag_forceSingle;	// TRUE: Create a single cue/bin file, even if the archive was MULTIFILE
	public bool flag_folder;		// TRUE: Create a subfolder with the game name in OutputDir
	public bool flag_encCue;		// TRUE: Will not restore audio tracks. Will create a cue with enc audio
	public int expectedTracks;		// In order for the progress report to work. set num of CD tracks here.

	// : Internal Use :

	// Temp dir for the current batch, it's autoset by this object,
	// is a subfolder of the master TEMP folder
	internal string tempDir;
	internal cd.CDInfos cd;

}// --



/// <summary>
/// A collection of tasks, that will Restore a cd,
/// Tasks will run in order, and some will run in parallel
/// </summary>
class JobRestore: CJob
{
	// --
	public JobRestore(RestoreParams p) : base("Restore CD")
	{
		// Check for input files
		// --------------------

		if(!CDCRUSH.check_file_(p.inputFile, CDCRUSH.CDCRUSH_EXTENSION)) {
			fail(msg: CDCRUSH.ERROR);
			return;
		}

		if(string.IsNullOrEmpty(p.outputDir)) {
			p.outputDir = Path.GetDirectoryName(p.inputFile);
		}

		// -- Output folder check
		if(p.flag_folder) {
			p.outputDir = CDCRUSH.checkCreateUniqueOutput(p.outputDir, Path.GetFileNameWithoutExtension(p.inputFile));
			if(p.outputDir==null) {
				fail("Output Dir Error " + p.outputDir);
				return;
			}
		}else{
			if(!FileTools.createDirectory(p.outputDir)) {
				fail(msg: "Can't create Output Dir " + p.outputDir);
				return;
			}
		}

		// --
		p.tempDir = CDCRUSH.getSubTempDir();
		if(!FileTools.createDirectory(p.tempDir)) {
			fail(msg: "Can't create TEMP dir");
			return;
		}

		// Safeguard, even if the GUI doesn't allow it
		if(p.flag_encCue)
		{
			p.flag_forceSingle = false;
		}

		// IMPORTANT!! sharedData gets set by value, NOT A POINTER, do not make changes to p after this
		jobData = p;

		// --
		hack_setExpectedProgTracks(p.expectedTracks + 3);

		// - Extract the Archive
		// -----------------------
		add(new CTask((t) => {
			var arc = new FreeArc(CDCRUSH.TOOLS_PATH);
			t.handleProcessStatus(arc);
			arc.extractAll(p.inputFile, p.tempDir);
			arc.onProgress = (pr) => t.PROGRESS=pr;
			// In case the operation is aborted
			t.killExtra = () => {
				arc.kill();
			};

		}, "Extracting","Extracting the archive to temp folder"));

		//  - Read JSON data
		//  - Restore tracks
		//  - JOIN if it has to
		// -----------------------
		add(new CTask((t) => 
		{
			var CD = new cd.CDInfos(); jobData.cd = CD;

			try{
				CD.jsonLoad(Path.Combine(p.tempDir, CDCRUSH.CDCRUSH_SETTINGS));
			}catch(haxe.lang.HaxeException e){
				t.fail(msg:e.Message); return;
			}


			LOG.log("== Detailed CD INFOS ==");
			LOG.log(CD.getDetailedInfo());

			for(int i=0;i<CD.tracks.length;i++)
			{
				// Push TASK RESTORE tasks right after this one
				// Note: Task will take care of encoded cue case
				addNextAsync(new TaskRestoreTrack(CD.tracks[i] as cd.CDTrack)); 
			}//--

			t.complete();

		}, "-Preparing to Restore","Reading stored CD info and preparing track restore tasks"));



		// - Join Tracks, but only when not creating .Cue/Enc Audio
		// -----------------------
		if(!p.flag_encCue) 
		add(new CTask((t) => 
		{
			cd.CDInfos cd = jobData.cd;
			// -- Join tracks
			if(p.flag_forceSingle || !cd.MULTIFILE) {
				// The task will read data from the shared job data var
				// Will join all tracks in place into track01.bin
				// Note: Sets track.workingFile to null to moved track
				addNext(new TaskJoinTrackFiles());
			}//--

			t.complete();

		}, "-Preparing to Join"));


		// - Prepare tracks `trackfile` which is the track written to the CUE
		// - Convert tracks
		// - Move files to final destination
		// - Create CUE files
		// - Delete Temp Files
		// -----------------------
		add(new CTask((t) => 
		{
			cd.CDInfos CD = jobData.cd;

			int progressStep = (int)Math.Round(100.0f/CD.tracks.length);
			// --
			for(int i=0;i<CD.tracks.length;i++)
			{
				cd.CDTrack track = CD.tracks[i] as cd.CDTrack;

				if(p.flag_encCue)
				{
					string ext = Path.GetExtension(track.workingFile);
					if(CD.tracks.length==1) {
						track.trackFile = $"{CD.CD_TITLE}{ext}";
					}else {
						track.trackFile = $"{CD.CD_TITLE} (track {track.trackNo}){ext}";
					}
					if(!CD.MULTIFILE) track.rewriteIndexes_forMultiFile(); // :: CONVERTS SINGLE TO MULTI
				}
				else
				{

					if(p.flag_forceSingle && CD.MULTIFILE) // :: CONVERT MULTI TO SINGLE
					{
						track.rewriteIndexes_forSingleFile();
					}

					if(CD.MULTIFILE && !p.flag_forceSingle) {
						track.trackFile = CD.CD_TITLE + " " + track.getFilenameRaw();
					}

					if(!CD.MULTIFILE || p.flag_forceSingle) {
						if(track.trackNo == 1)
							track.trackFile = CD.CD_TITLE + ".bin";
						else
							track.trackFile = null;
					}

				}
				// --
				// Move ALL files to final output folder
				// NULL workingFile means that is has been deleted
				if(track.workingFile != null) {
					FileTools.tryMove(track.workingFile, Path.Combine(p.outputDir, track.trackFile));
					t.PROGRESS+=progressStep;
				}

			}// -- end track processing

			// --
			// Create CUE File
			try{
				CD.cueSave(
					Path.Combine(p.outputDir,CD.CD_TITLE + ".cue") ,
					new haxe.root.Array<object>( new [] {
						"CDCRUSH (dotNet) version : " + CDCRUSH.PROGRAM_VERSION,
						CDCRUSH.LINK_SOURCE
				}));
			}catch(haxe.lang.HaxeException e) {
				t.fail(msg:e.Message); return;
			}

			t.complete();

		}, "Moving, Finalizing","Calculating track data and creating .CUE"));

		// - Complete -

	}// -----------------------------------------


	// -
	public override void start()
	{
		RestoreParams p = jobData;
		LOG.line();
		LOG.log("=== RESTORING A CD with the following parameters :");
		LOG.log("- Input : {0}", p.inputFile);
		LOG.log("- Output Dir : {0}", p.outputDir);
		LOG.log("- Temp Dir : {0}", p.tempDir);
		LOG.log("- Force Single bin : {0}", p.flag_forceSingle);
		LOG.log("- Create subfolder : {0}", p.flag_folder);
		LOG.log("- Restore to encoded audio/.cue : {0}", p.flag_encCue);
		base.start();
	}// -----------------------------------------


	/// <summary>
	/// Called on FAIL / COMPLETE / PROGRAM EXIT
	/// Clean up temporary files
	/// </summary>
	protected override void kill()
	{
		base.kill();

		if(CDCRUSH.FLAG_KEEP_TEMP) return;

		RestoreParams p = jobData;
		if (p.tempDir != p.outputDir)  // NOTE: This is always a subdir of the master temp dir
		{ 
			try {
				Directory.Delete(p.tempDir, true);
			}catch(IOException){
				// do nothing
			}
		}// --			
	}// -----------------------------------------

}// --
}// --
