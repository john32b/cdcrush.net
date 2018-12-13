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
	public bool flag_folder;		// TRUE: Create a subfolder with the game name in OutputDir
	public int expectedTracks;		// In order for the progress report to work. set num of CD tracks here.
	public int mode;				// 0:Normal, 1:Merge All, 2:To Encoded Audio Tracks
//	public bool flag_forceSingle;	// TRUE: Create a single cue/bin file, even if the archive was MULTIFILE
//	public bool flag_encCue;		// TRUE: Will not restore audio tracks. Will create a cue with enc audio

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
	RestoreParams p;

	// --
	public JobRestore(RestoreParams par) : base("Restore CD")
	{
		p = par;

		// --
		hack_setExpectedProgTracks(p.expectedTracks + 3);

		// - Extract the Archive
		// -----------------------
		add(new CTask((t) => {
			var arc = ArchiveMaster.getArchiver(p.inputFile);
			t.handleProcessStatus(arc);
			arc.extract(p.inputFile, p.tempDir);
			arc.onProgress = (pr) => t.PROGRESS = pr;
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
			p.cd = new cd.CDInfos();

			try{
				p.cd.jsonLoad(Path.Combine(p.tempDir, CDCRUSH.CDCRUSH_SETTINGS));
			}catch(haxe.lang.HaxeException e){
				t.fail(msg:e.Message); return;
			}

			jobData = p;

			LOG.log("== Detailed CD INFOS ==");
			LOG.log(p.cd.getDetailedInfo());

			for(int i=0;i<p.cd.tracks.length;i++)
			{
				// Push TASK RESTORE tasks right after this one
				// Note: Task will take care of encoded cue case
				addNextAsync(new TaskRestoreTrack(p.cd.tracks[i] as cd.CDTrack)); 
			}//--

			t.complete();

		}, "-Preparing to Restore","Reading stored CD info and preparing track restore tasks"));



		// - Join all Tracks together
		// -------------------------
		if(p.mode<2)
		add(new CTask((t) => 
		{
			// -- Join tracks
			if(p.mode==1 || !p.cd.MULTIFILE) {
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
			cd.CDInfos CD = p.cd;

			int progressStep = (int)Math.Round(100.0f/CD.tracks.length);
			// --
			for(int i=0;i<CD.tracks.length;i++)
			{
				cd.CDTrack track = CD.tracks[i] as cd.CDTrack;
				string ext = Path.GetExtension(track.workingFile);
	
				if(p.mode==2) // Restore to encoded audio
				{				
					if(CD.tracks.length==1) {
						track.trackFile = $"{CD.CD_TITLE}{ext}";
					}else {
						track.trackFile = $"{CD.CD_TITLE} (track {track.trackNo}){ext}";
					}
					if(!CD.MULTIFILE) track.rewriteIndexes_forMultiFile(); // :: CONVERTS SINGLE TO MULTI
				}
				else
				{
					if(p.mode==1 && CD.MULTIFILE) // :: CONVERT MULTI TO SINGLE
					{
						track.rewriteIndexes_forSingleFile();
					}

					if(CD.MULTIFILE && p.mode!=1) 
					{
						track.trackFile = $"{CD.CD_TITLE} (track {track.trackNo}){ext}";
					}

					if(!CD.MULTIFILE || p.mode==1) {
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

	void check_parameters()
	{
		// Check for input files
		// --------------------
		if(!CDCRUSH.check_file_(p.inputFile, CDCRUSH.CDCRUSH_EXTENSIONS)) {
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

	}// ------------------

	// -
	public override void start()
	{
		check_parameters();
		LOG.line();
		LOG.log("=== RESTORING A CD with the following parameters :");
		LOG.log("- Input : {0}", p.inputFile);
		LOG.log("- Output Dir : {0}", p.outputDir);
		LOG.log("- Temp Dir : {0}", p.tempDir);
		LOG.log("- Create subfolder : {0}", p.flag_folder);
		LOG.log("- Restore Method : {0}", p.mode);
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
