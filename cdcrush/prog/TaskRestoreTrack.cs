using cdcrush.lib;
using cdcrush.lib.app;
using System.IO;

namespace cdcrush.prog
{


/// <summary>
/// - Restores compressed to full size
/// - Sets track.workingFile to new file
/// - Delets old file
/// </summary>
class TaskRestoreTrack : lib.task.CTask
{

	// Point to the JOB's restore parameters
	RestoreParams p;
	CueTrack track;

	bool isFlac = false;

	string crushedTrackPath;  // Autocalculated

	// --
	public TaskRestoreTrack(CueTrack tr)
	{
		name = "Restore";
		desc = string.Format("Restoring track {0}", tr.trackNo);
		track = tr;
	}// -----------------------------------------

	// --
	public override void start()
	{
		base.start();

		p = (RestoreParams)jobData;

		// --
		crushedTrackPath = Path.Combine(p.tempDir, track.storedFileName);
		// Set the final track pathname now, I need this for later.
		track.workingFile = Path.Combine(p.tempDir, track.getFilenameRaw());

		// --
		if(track.isData)
		{
			var ecm = new EcmTools(CDCRUSH.TOOLS_PATH);
			ecm.onComplete = (s) => {
			ecm.onProgress = handleProgress;
				if(s){
					deleteOldFile();
					if(!checkTrackMD5()) {
						fail(msg:"MD5 checksum is wrong!");
						return;
					}
					complete();
				}else{
					fail(msg:ecm.ERROR);
				}
			};
			ecm.unecm(crushedTrackPath);
			killExtra = () => ecm.kill();
		}
		else
		{
			// No need to convert back
			if(p.flag_encCue) {
				// Fix current file
				track.workingFile = crushedTrackPath;
				complete();
				return;
			}

			// --
			isFlac = (Path.GetExtension(track.storedFileName) == ".flac");

			var ffmp = new FFmpeg(CDCRUSH.FFMPEG_PATH);
			ffmp.onProgress = handleProgress;
			ffmp.onComplete = (s) => {
				if(s){
					deleteOldFile(); // Don't need it
					if(!isFlac) {
						// OGG and MP3 don't restore to the exact byte length
						correctPCMSize();
					}else
					{
						// FLAC restores to exact bytes
						if(!checkTrackMD5()){
							fail(msg:"MD5 checksum is wrong!");
							return;
						}
					}
					complete();
				}else{
					fail(msg:ffmp.ERROR);
				}
			};

			ffmp.audioToPCM(crushedTrackPath);
			killExtra = () => ffmp.kill();
		}

		log("Restoring track -" + track.storedFileName);

	}// -----------------------------------------

	// --
	void handleProgress(int p)
	{
		PROGRESS = p;
	}// -----------------------------------------

	// --
	void deleteOldFile()
	{
		if(CDCRUSH.FLAG_KEEP_TEMP) return;
		File.Delete(crushedTrackPath);
	}// -----------------------------------------

	
	// -
	// Fix the filesize of the restored track
	// This is only when restoring from .OGG files, .FLAC seems to be fine by default.
	void correctPCMSize()
	{
		using(FileStream fileStream = new FileStream(track.workingFile,FileMode.Open, FileAccess.Write))
		{
			fileStream.SetLength(track.byteSize);
		}
	}// -----------------------------------------

	// Check the track's generated MD5 against the original MD5
	// It can take some time if it's a big file.
	bool checkTrackMD5()
	{
		// Only check if requested? TODO.
		return true; 

		if(track.md5==null){
			LOG.log("Cannot check MD5 against original, this is an old version archive where md5 was not calculated.");
			return true;
		}

		string newMD5 = "";

		// Before compressing the tracks, get and store the MD5 of the track
		using(var md5 = System.Security.Cryptography.MD5.Create())
		{
			using(var str = File.OpenRead(track.workingFile))
			{
				newMD5 = System.BitConverter.ToString(md5.ComputeHash(str)).Replace("-","").ToLower();
			}
		}

		return (track.md5==newMD5);
	}// -----------------------------------------

}// -- end class
}// -- end namespace
