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
	// Point to the JOB's restore parameters
	CrushParams p;
	CueTrack track;

	string trackFile;  // Temp name, Autocalculated

	// --
	public TaskCompressTrack(CueTrack tr):base(null,"Compressing")
	{
		name = "Compress";
		desc = string.Format("Compressing track {0}", tr.trackNo);
		track = tr;
	}// -----------------------------------------

	// --
	public override void start()
	{
		base.start();

		p = (CrushParams) jobData;

		// Working file is already set and points to either TEMP or INPUT folder
		trackFile = track.workingFile;

		// Before compressing the tracks, get and store the MD5 of the track
		using(var md5 = System.Security.Cryptography.MD5.Create())
		{
			using(var str = File.OpenRead(trackFile))
			{
				track.md5 = BitConverter.ToString(md5.ComputeHash(str)).Replace("-","").ToLower();
			}
		}

		// --
		if(track.isData)
		{
			var ecm = new EcmTools(CDCRUSH.TOOLS_PATH);
			ecm.onComplete = (s) => {
				if(s) {
					deleteOldFile();
					complete();
				}else{
					fail(msg:ecm.ERROR);
				}
			};

			// In case the task ends abruptly
			killExtra = () => ecm.kill();

			// New filename that is going to be generated:
			track.storedFileName = track.getTrackName() + ".bin.ecm";
			track.workingFile = Path.Combine(jobData.tempDir, track.storedFileName);
			ecm.ecm(trackFile,track.workingFile);	// old .bin file from wherever it was to temp/bin.ecm
		}
		else // AUDIO TRACK :
		{
			var ffmp = new FFmpeg(CDCRUSH.FFMPEG_PATH);
			ffmp.onComplete = (s) => {
				if(s) {
					deleteOldFile();
					complete();
				}else {
					fail(msg:ffmp.ERROR);
				}
			};

			// In case the task ends abruptly
			killExtra = () => ffmp.kill();

			// Cast for easy coding
			Tuple<int,int> audioQ = jobData.audioQuality;

			switch(audioQ.Item1)
			{
				case 0: // FLAC
					track.storedFileName = track.getTrackName() + ".flac";
					track.workingFile = Path.Combine(jobData.tempDir, track.storedFileName);
					ffmp.audioPCMToFlac(trackFile,track.workingFile);
					break;

				case 1: // VORBIS
					track.storedFileName = track.getTrackName() + ".ogg";
					track.workingFile = Path.Combine(jobData.tempDir, track.storedFileName);
					ffmp.audioPCMToOggVorbis(trackFile, audioQ.Item2, track.workingFile);
					break;

				case 2: // OPUS
					track.storedFileName = track.getTrackName() + ".ogg";
					track.workingFile = Path.Combine(jobData.tempDir, track.storedFileName);
					ffmp.audioPCMToOggOpus(trackFile, CDCRUSH.OPUS_QUALITY[audioQ.Item2], track.workingFile);
					break;

			}//- end switch

		}//- end if (track.isData)
		
	}// -----------------------------------------

	// --
	// Delete old files ONLY IF they reside in the TEMP folder!
	void deleteOldFile()
	{
		if(CDCRUSH.FLAG_KEEP_TEMP) return;
		if(jobData.workFromTemp) {
			File.Delete(trackFile); // Delete it if it was cut from a single bin
		}
	}// -----------------------------------------

}// -- end class	
}// -- end namespace
