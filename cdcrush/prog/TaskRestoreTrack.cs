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
	cd.CDTrack track;

	// Lossy audio codecs don't revert back to exact byte
	bool requirePostSizeFix = false;

	string INPUT;	// The file that is going to be restored
	string OUTPUT;  // The file that is going to be created/restored to

	// --
	public TaskRestoreTrack(cd.CDTrack tr)
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

		INPUT = Path.Combine(p.tempDir, track.storedFileName);
		OUTPUT = Path.Combine(p.tempDir, track.getFilenameRaw());
		track.workingFile = OUTPUT; // Point to the new file

		// -
		var fileExt = Path.GetExtension(track.storedFileName);
		requirePostSizeFix = AudioMaster.isLossyByExt( fileExt );

		// --
		if(track.isData)
		{
			var ecm = new EcmTools(CDCRUSH.TOOLS_PATH);
			setupHandlers(ecm);
			ecm.unecm(INPUT);
		}
		else
		{
			// No need to convert back, end the task
			if(p.mode==2) {
				// Point to correct file
				track.workingFile = INPUT;
				complete();
				return;
			}

			var ffmp = new FFmpeg(CDCRUSH.FFMPEG_PATH);

			if(fileExt.ToLower()==".tak")
			{
				var tak = new Tak(CDCRUSH.TOOLS_PATH);
				setupHandlers(tak);

				tak.decodeToStream(INPUT,(_out) => {
					ffmp.convertWavStreamToPCM(OUTPUT,(_in)=>{
						_out.CopyTo(_in);
						_in.Close();
					});
				});

			}else
			{
				setupHandlers(ffmp);
				ffmp.audioToPCM(INPUT,track.workingFile);
			}
		}

		log("Restoring track -" + track.storedFileName);

	}// -----------------------------------------

	// --
	void setupHandlers(IProcessStatus o)
	{
		killExtra = () => o.kill();
		o.onProgress = (p) => PROGRESS = p; // Note: Uses setter
		o.onComplete = (s) => {
			if(s){
				deleteOldFile(); 
				if(requirePostSizeFix) 
				{
					correctPCMSize(); // Note: OGG and MP3 don't restore to the exact byte length
				}
				complete();
			}else{
				fail(msg:o.ERROR);
			}
		};
	}


	// - 
	// NOTE: Input files were created from the .ARC into TEMP folder, so I can delete them 
	//       as soon as I am done with them
	void deleteOldFile()
	{
		if(CDCRUSH.FLAG_KEEP_TEMP) return;
		File.Delete(INPUT);
	}// -----------------------------------------

	
	// -
	// Fix the filesize of the restored track
	// This is only when restoring from lossy encoders. Lossless restore to exact bytes
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
