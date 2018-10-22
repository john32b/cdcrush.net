using cdcrush.lib;
using cdcrush.lib.app;
using System;
using System.IO;


namespace cdcrush.prog
{


/// <summary>
/// - Compresses a Track (data or audio)
///
/// CHANGES:
///  - track.workingFile : points to the new encoded file path
///  - track.storedFileName : is set to just a filename. e.g (track02.ogg) How it's saved in the archive?
///  - The old `track.workingFile` is deleted
/// 
/// </summary>
class TaskCompressTrack : lib.task.CTask
{
	// Copy of JOB restore parameters
	CrushParams p;
	// Pointer to working track
	cd.CDTrack track;

	string INPUT;	// File that is going to be encoded
	string OUTPUT;	// File that is going to be created from INPUT

	// --
	public TaskCompressTrack(cd.CDTrack tr):base(null,"Encoding")
	{
		name = "Compress";
		desc = string.Format("Encoding track {0}", tr.trackNo);
		track = tr;
	}// -----------------------------------------

	// --
	public override void start()
	{
		base.start();

		p = (CrushParams) jobData;

		// Working file is already set and points to either TEMP or INPUT folder
		INPUT = track.workingFile;
		// NOTE: OUTPUT path is generated later with the setupfiles() function
		
		// Before compressing the tracks, get and store the MD5 of the track
		using(var md5 = System.Security.Cryptography.MD5.Create())
		{
			using(var str = File.OpenRead(INPUT))
			{
				track.md5 = BitConverter.ToString(md5.ComputeHash(str)).Replace("-","").ToLower();
			}
		}

		// --
		if(track.isData)
		{
			var ecm = new EcmTools(CDCRUSH.TOOLS_PATH);
			setupHandlers(ecm);

			// New filename that is going to be generated:
			setupFiles(".bin.ecm");
			ecm.ecm(INPUT,OUTPUT);	// old .bin file from wherever it was to temp/bin.ecm
		}
		else // AUDIO TRACK :
		{
			// Get Audio Data. (codecID, codecQuality)
			Tuple<string,int> audioQ = jobData.audioQuality;

			// New filename that is going to be generated:
			setupFiles(AudioMaster.getCodecExt(audioQ.Item1));

			// I need ffmpeg for both occations 
			var ffmp = new FFmpeg(CDCRUSH.FFMPEG_PATH);
			setupHandlers(ffmp);

			if(audioQ.Item1=="TAK")
			{	
				var tak = new Tak(CDCRUSH.TOOLS_PATH);
			
				// This will make FFMPEG read the PCM file, convert it to WAV on the fly
				// and feed it to TAK, which will convert and save it.
				ffmp.convertPCMStreamToWavStream( (ffmpegIn,ffmpegOut) => {
					var sourceFile = File.OpenRead(INPUT);
					tak.encodeFromStream(OUTPUT , (takIn) => { 
						ffmpegOut.CopyTo(takIn);
						takIn.Close();
					});
					sourceFile.CopyTo(ffmpegIn);	// Feed PCM to FFMPEG
					ffmpegIn.Close();
				});

			}else{
				// It must be FFMPEG
				ffmp.encodePCM(audioQ.Item1, audioQ.Item2, INPUT, OUTPUT);
			}		
			
		}//- end if (track.isData)
		
	}// -----------------------------------------


	// Quickly add handlers to a process
	// --
	void setupHandlers(IProcessStatus o)
	{
		o.onProgress = (p) => PROGRESS = p; // Uses setter
		o.onComplete = (s) => {
			if(s) {
				deleteOldFile();
				complete();
			}else {
				fail(msg:o.ERROR);
			}
		};

		killExtra = () => o.kill(); // In case the task ends abruptly
	}// --


	// Qucikly set :
	// + storedFileName
	// + workingFile
	// Ext must have the period (.) e.g. ".flac"
	void setupFiles(string ext)
	{
		track.storedFileName = track.getTrackName() + ext;

		if(p.flag_convert_only) {
			// Convert files to output folder directly
			track.workingFile = Path.Combine(jobData.outputDir, track.storedFileName);
		}else{
			// Convert files to temp folder, since they are going to be archived later
			track.workingFile = Path.Combine(jobData.tempDir, track.storedFileName);
		}

		OUTPUT = track.workingFile;
	}// -----------------------------------------

	// --
	// Delete old files ONLY IF they reside in the TEMP folder!
	void deleteOldFile()
	{
		if(CDCRUSH.FLAG_KEEP_TEMP) return;

		// Make sure the file is in the TEMP folder ::
		if(jobData.flag_sourceTracksOnTemp)
		{
			File.Delete(INPUT); 
		}
	}// -----------------------------------------

}// -- end class	
}// -- end namespace
