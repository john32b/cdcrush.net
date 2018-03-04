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

		log("Restoring track -" + track.storedFileName);

		// --
		crushedTrackPath = Path.Combine(p.tempDir, track.storedFileName);
		// Set the final track pathname now, I need this for later.
		track.workingFile = Path.Combine(p.tempDir, track.getFilenameRaw());

		// --
		if(track.isData)
		{
			var ecm = new EcmTools(CDCRUSH.TOOLS_PATH);
			ecm.onComplete = (s) => {
				if(s){
					deleteOldFile();
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
			var ffmp = new FFmpeg(CDCRUSH.FFMPEG_PATH);
			ffmp.onComplete = (s) => {
				if(s){
					deleteOldFile();
					correctPCMSize();
					complete();
				}else{
					fail(msg:ffmp.ERROR);
				}
			};
			ffmp.audioToPCM(crushedTrackPath);
			killExtra = () => ffmp.kill();
		}
		
	}// -----------------------------------------

	// --
	void deleteOldFile()
	{
		File.Delete(crushedTrackPath);
	}// -----------------------------------------

	// Implies restored OK and file exists/
	void correctPCMSize()
	{
		//log("SIZE BEFORE " + new FileInfo(track.workingFile).Length); -> OK WORKS
		using(FileStream fileStream = new FileStream(track.workingFile,FileMode.Open, FileAccess.Write))
		{
			fileStream.SetLength(track.byteSize);
		}
		//log("SIZE AFTER " + new FileInfo(track.workingFile).Length); -> OK WORKS
	}// -----------------------------------------

}// -- end class
	
}// -- end namespace
