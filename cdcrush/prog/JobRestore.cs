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

	// : Internal Use :

	// Temp dir for the current batch, it's autoset by this object,
	// is a subfolder of the master TEMP folder
	internal string tempDir;
	internal CueReader cd;

}// --


/// <summary>
/// A collection of tasks, that will CRUSH a cd,
/// Tasks will run in order, and some will run in parallel
/// </summary>
class JobRestore: CJob
{
	// --
	public JobRestore(RestoreParams p) : base("Restore CD")
	{

		// Check for input files
		// :: --------------------

		if(!CDCRUSH.check_file_(p.inputFile, CDCRUSH.CDCRUSH_EXTENSION)) {
			fail(msg: CDCRUSH.ERROR);
			return;
		}

		if(string.IsNullOrEmpty(p.outputDir)) {
			p.outputDir = Path.GetDirectoryName(p.inputFile);
		}

		if(p.flag_folder) {
			try {
				p.outputDir = Path.Combine(p.outputDir, Path.GetFileNameWithoutExtension(p.inputFile));
			}
			catch(ArgumentException) {
				fail("Output Dir Error " + p.outputDir);
				return;
			}
		}

		if(!FileTools.createDirectory(p.outputDir)) {
			fail(msg: "Can't create Output Dir " + p.outputDir);
			return;
		}

		p.tempDir = Path.Combine(CDCRUSH.TEMP_FOLDER, Guid.NewGuid().ToString().Substring(0, 12));
		if(!FileTools.createDirectory(p.tempDir)) {
			fail(msg: "Can't create TEMP dir");
			return;
		}

		// IMPORTANT!! sharedData gets set by value, NOT A POINTER, do not make changes to p after this
		jobData = p;

		// - Extract the Archive
		// -----------------------
		add(new CTask((t) => {
			var arc = new FreeArc();
			t.handleCliReport(arc);
			arc.extractAll(p.inputFile, p.tempDir);
		}, "Extracting", true));

		//  - Read JSON data
		//  - Restore tracks
		//  - JOIN if it has to
		// -----------------------
		add(new CTask((t) => {
			var cd = new CueReader();
			jobData.cd = cd;

			// This runs in sync:
			if(!cd.loadJson(Path.Combine(p.tempDir, CDCRUSH.CDCRUSH_SETTINGS))) {
				t.fail(msg: cd.ERROR);
				return;
			}//--

			// #DEBUG
			// cd.debugInfo();

			// - Push TASK RESTORE tasks right after this one
			foreach(CueTrack tr in cd.tracks) {
				addNextAsync(new TaskRestoreTrack(tr));
			}//--

			t.complete();

		}, "Preparing to Restore"));



		// - Join Tracks
		// -----------------------
		add(new CTask((t) => {
			CueReader cd = jobData.cd;
			// -- Join tracks
			if(p.flag_forceSingle || !cd.MULTIFILE) {
				// The task will read data from the shared job data var
				// Will join all tracks in place into track01.bin
				// Note: Sets track.workingFile to null to moved track
				addNext(new TaskJoinTrackFiles());
			}//--

			t.complete();

		}, "Preparing to Join"));




		// - Prepare tracks `trackfile` which is the track written to the CUE
		// - Convert tracks
		// - Move files to final destination
		// - Create CUE files
		// - Delete Temp Files
		// -----------------------
		add(new CTask((t) => {
			CueReader cd = jobData.cd;

			// --
			foreach(var track in cd.tracks) {
				if(p.flag_forceSingle && cd.MULTIFILE) // :: CONVERT MULTI TO SINGLE
				{
					track.setNewTimesBasedOnSector();
				}

				if(cd.MULTIFILE && !p.flag_forceSingle) {
					track.trackFile = cd.CD_TITLE + " " + track.getFilenameRaw();
				}

				if(!cd.MULTIFILE || p.flag_forceSingle) {
					if(track.trackNo == 1)
						track.trackFile = cd.CD_TITLE + ".bin";
					else
						track.trackFile = null;

				}

				// --
				// Move ALL files to final output folder
				// NULL workingFile means that is has been deleted
				if(track.workingFile != null) {
					FileTools.tryMove(track.workingFile, Path.Combine(p.outputDir, track.trackFile));
				}

			}// -- end track processing

			// --
			// Create CUE File
			if(!cd.saveCUE(Path.Combine(p.outputDir, cd.CD_TITLE + ".cue"))) {
				t.fail(cd.ERROR); return;
			}

			t.complete();

		}, "Moving, Finalizing"));

		// - Complete -

	}// -----------------------------------------


	/// <summary>
	/// Called on FAIL and COMPLETE
	/// </summary>
	protected override void kill()
	{
		base.kill();

		RestoreParams p = jobData;
		if (p.tempDir != p.outputDir) {
			// NOTE: This is always a subdir of the master
			try
			{
				Directory.Delete(p.tempDir, true);
			}
			catch(IOException)
			{
				// do nothing
			}
		}// --			

	}// -----------------------------------------

}// --
}// --
