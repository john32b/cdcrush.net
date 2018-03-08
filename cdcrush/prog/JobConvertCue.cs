using cdcrush.lib;
using cdcrush.lib.app;
using cdcrush.lib.task;

using System;
using System.IO;


namespace cdcrush.prog
{
	

/// <summary>
/// A collection of tasks, that will CRUSH a cd,
/// Tasks will run in order, and some will run in parallel
/// </summary>
class JobConvertCue:CJob
{
	
	const string FOLDER_SUFFIX = " (c)";

	// --
	public JobConvertCue(CrushParams p):base("Convert CD")
	{
		// Check for input files
		// :: --------------------
			if(!CDCRUSH.check_file_(p.inputFile,".cue")) {
				fail(msg:CDCRUSH.ERROR);
				return;
			}

			if(string.IsNullOrEmpty(p.outputDir)) {
				p.outputDir = Path.GetDirectoryName(p.inputFile);
			}

			// : NEW :
			// : ALWAYS Create a subfolder to avoid overwriting the source files
			try {
				p.outputDir = Path.Combine(p.outputDir, p.cdTitle + FOLDER_SUFFIX);
			}
			catch(ArgumentException) {
				fail("Output Dir Error " + p.outputDir);
				return;
			}

			if(!FileTools.createDirectory(p.outputDir)) {
				fail(msg: "Can't create Output Dir " + p.outputDir);
				return;
			}

			p.tempDir = Path.Combine(CDCRUSH.TEMP_FOLDER,Guid.NewGuid().ToString().Substring(0, 12));
			if(!FileTools.createDirectory(p.tempDir)) {
				fail(msg: "Can't create TEMP dir");
				return;
			}

		// IMPORTANT!! sharedData gets set by value, NOT A POINTER, do not make changes to p after this
		jobData = p;

		// --
		// - Read the CUE file ::
		add(new CTask((t) =>
		{
			var cd = new CueReader();
			jobData.cd = cd;

			if(!cd.load(p.inputFile)) {
				t.fail(msg:cd.ERROR);
				return;
			}

			// Post CD CUE load ::

			// In case user named the CD, otherwise it's going to be the same
			if(!string.IsNullOrWhiteSpace(p.cdTitle))
			{
				cd.CD_TITLE = FileTools.sanitizeFilename(p.cdTitle);
			}

			// Real quality to string name
			cd.CD_AUDIO_QUALITY = CDCRUSH.getAudioQualityString(p.audioQuality);

			// This flag notes that all files will go to the TEMP folder
			jobData.workFromTemp = !cd.MULTIFILE;

			t.complete();

		},"Reading",true));

		
		// - Cut tracks
		// ---------------------------
		add(new TaskCutTrackFiles());

		// - Compress tracks
		// ---------------------
		add(new CTask((t) =>
		{
			// Only encode the audio tracks
			foreach(CueTrack tr in (jobData.cd as CueReader).tracks)  {	
				if(!tr.isData) addNextAsync(new TaskCompressTrack(tr));
			}
			t.complete();
		},"Preparing"));

		// - Create new CUE file
		// --------------------
		add(new CTask((t) =>
		{
			CueReader cd = jobData.cd;

			// DEV: So far :
			// track.trackFile is UNSET. cd.saveCue needs it to be set.
			// track.workingFile points to a valid file, some might be in TEMP folder and some in input folder (data tracks)

			// -- Move files to output folder
			foreach(var track in cd.tracks) 
			{

				if(!cd.MULTIFILE){
					// Fix the index times to start with 00:00:00
					track.setNewTimesReset();
				}

				string ext = Path.GetExtension(track.workingFile);
				
				track.trackFile = $"{cd.CD_TITLE} (track {track.trackNo}){ext}";

				// Data track was not cut or encoded.
				// It's in the input folder, don't move it
				if(track.isData && cd.MULTIFILE)
				{
					FileTools.tryCopy(track.workingFile,Path.Combine(p.outputDir, track.trackFile));
				}
				else
				{
					FileTools.tryMove(track.workingFile, Path.Combine(p.outputDir, track.trackFile));
				}
			}

			//-- Create the new CUE file
			if(!cd.saveCUE(Path.Combine(p.outputDir,cd.CD_TITLE + ".cue"))) {
				t.fail(cd.ERROR); return;
			}

			t.complete();

		}, "Finalizing"));

		// -- COMPLETE --

	}// -----------------------------------------

	/// <summary>
	/// Called on FAIL / COMPLETE / PROGRAM EXIT
	/// Clean up temporary files
	/// </summary>
	protected override void kill()
	{
		base.kill();

		if(CDCRUSH.FLAG_KEEP_TEMP) return;

		// - Cleanup
		CrushParams p = jobData;
		if (p.tempDir != p.outputDir)  // NOTE: This is always a subdir of the master temp dir
		{ 
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
