using cdcrush.lib;
using cdcrush.lib.task;

using System;
using System.IO;


namespace cdcrush.prog
{
	

/// <summary>
/// A collection of tasks, that will CONVERT a cd,
/// Tasks will run in order, and some will run in parallel
/// </summary>
class JobConvertCue:CJob
{

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
			p.outputDir = CDCRUSH.checkCreateUniqueOutput(p.outputDir, p.cdTitle + CDCRUSH.RESTORED_FOLDER_SUFFIX);
			if(p.outputDir==null) {
				fail("Output Dir Error " + p.outputDir);
				return;
			}

			// -
			p.tempDir = CDCRUSH.getSubTempDir();
			if(!FileTools.createDirectory(p.tempDir)) {
				fail(msg: "Can't create TEMP dir");
				return;
			}

		// Useful to know.
		p.flag_convert_only = true;

		// IMPORTANT!! sharedData gets set by value, NOT A POINTER, do not make changes to p after this
		jobData = p;

		hack_setExpectedProgTracks(p.expectedTracks + 2);

		// --
		// - Read the CUE file ::
		add(new CTask((t) =>
		{
			var CD = new cd.CDInfos();
			jobData.cd = CD;

			try{
				CD.cueLoad(p.inputFile);
			}catch(haxe.lang.HaxeException e){
				t.fail(msg:e.Message); return;
			}

			// --
			if(CD.tracks.length==1)
			{
				t.fail(msg:"No point in converting. No audio tracks on the cd.");
				return;
			}

			// Meaning the tracks are going to be extracted in the temp folder
			jobData.flag_sourceTracksOnTemp = (!CD.MULTIFILE && CD.tracks.length > 1);

			// In case user named the CD, otherwise it's going to be the same
			if(!string.IsNullOrWhiteSpace(p.cdTitle))
			{
				CD.CD_TITLE = FileTools.sanitizeFilename(p.cdTitle);
			}

			// Real quality to string name
			CD.CD_AUDIO_QUALITY = CDCRUSH.getAudioQualityString(p.audioQuality);

			t.complete();

		},"-Reading", "Reading CUE data and preparing"));

		
		// - Cut tracks
		// ---------------------------
		add(new TaskCutTrackFiles());

		// - Encode tracks
		// ---------------------
		add(new CTask((t) =>
		{
			// Only encode the audio tracks
			cd.CDInfos CD = jobData.cd;
			for(int i=0;i<CD.tracks.length;i++)
			{
				cd.CDTrack tr = CD.tracks[i] as cd.CDTrack;
				if(!tr.isData) addNextAsync(new TaskCompressTrack(tr));
			}
			t.complete();
		},"-Preparing", "Preparing to compress tracks"));

		// - Create new CUE file
		// --------------------
		add(new CTask((t) =>
		{
			cd.CDInfos CD = jobData.cd;

			// DEV: So far :
			// track.trackFile is UNSET. cd.saveCue needs it to be set.
			// track.workingFile points to a valid file, some might be in TEMP folder and some in input folder (data tracks)

			int stepProgress = (int)Math.Round(100.0f/CD.tracks.length);

			// -- Move files to output folder
			for(int i=0;i<CD.tracks.length;i++)
			{
				cd.CDTrack track = CD.tracks[i] as cd.CDTrack;

				if(!CD.MULTIFILE) {
					// Fix the index times to start with 00:00:00
					track.rewriteIndexes_forMultiFile();
				}

				string ext = Path.GetExtension(track.workingFile);
				
				track.trackFile = $"{CD.CD_TITLE} (track {track.trackNo}){ext}";

				// Data track was not cut or encoded.
				// It's in the input folder, don't move it
				if(track.isData && CD.MULTIFILE)
				{
					FileTools.tryCopy(track.workingFile, Path.Combine(p.outputDir, track.trackFile));
				}
				else
				{
					// TaskCompress already put the audio files on the output folder
					// But it's no big deal calling it again
					// This is for the data tracks that are on the temp folder
					FileTools.tryMove(track.workingFile, Path.Combine(p.outputDir, track.trackFile));
				}

				t.PROGRESS += stepProgress;
			}

			//-- Create the new CUE file
			try{
				CD.cueSave(
					Path.Combine(p.outputDir,CD.CD_TITLE + ".cue") ,
					new haxe.root.Array<object>( new [] {
						"CDCRUSH (dotNet) version : " + CDCRUSH.PROGRAM_VERSION,
						CDCRUSH.LINK_SOURCE
				}));

			}catch(haxe.lang.HaxeException e){
				t.fail(msg:e.Message); return;
			}

			t.complete();

		}, "Finalizing","Calculating track data and creating .CUE"));

		// -- COMPLETE --

	}// -----------------------------------------

	// -
	public override void start()
	{
		CrushParams p = jobData;
		LOG.line();
		LOG.log("=== CONVERTING A CD with the following parameters :");
		LOG.log("- Input : {0}", p.inputFile);
		LOG.log("- Output Dir : {0}", p.outputDir);
		LOG.log("- Temp Dir : {0}", p.tempDir);
		LOG.log("- CD Title  : {0}", p.cdTitle);
		LOG.log("- Audio Quality : {0}",CDCRUSH.getAudioQualityString(p.audioQuality));
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

		// - Cleanup
		CrushParams p = jobData;
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
