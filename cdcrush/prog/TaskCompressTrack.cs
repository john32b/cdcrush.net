using cdcrush.lib;
using cdcrush.lib.app;
using System;
using System.IO;


namespace cdcrush.prog
{


/// <summary>
/// - Compress Track (data or audio)
/// - Set track.storedFileName to proper name
/// </summary>
class TaskCompressTrack : lib.task.CTask
{

	// Point to the JOB's restore parameters
	CrushParams p;
	CueTrack track;

	string trackFile;  // Autocalculated

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

			int audioQ = jobData.audioQuality;
			if(audioQ==0) // FLAC
			{
				track.storedFileName = track.getTrackName() + ".flac";
				track.workingFile = Path.Combine(jobData.tempDir, track.storedFileName);
				ffmp.audioPCMToFlac(trackFile,track.workingFile);

			}else // OGG OPUS
			{
				track.storedFileName = track.getTrackName() + ".ogg";
				track.workingFile = Path.Combine(jobData.tempDir, track.storedFileName);
				ffmp.audioPCMToOgg(trackFile, CDCRUSH.OPUS_QUALITY[audioQ - 1], track.workingFile);
			}
		}
		
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
