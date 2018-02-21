using cdcrush.lib;
using cdcrush.lib.app;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		
		// --
		if(track.isData)
		{
			var ecm = new EcmTools();
			ecm.onComplete = (s) => {
				if(s) {
					deleteOldFile();
					complete();
				}else{
					fail(msg:ecm.ERROR);
				}
			};
			

			// Before compressing DATA tracks, get the MD5
			using(var md5 = System.Security.Cryptography.MD5.Create())
			{
				using(var str = File.OpenRead(trackFile))
				{
					track.md5 = BitConverter.ToString(md5.ComputeHash(str)).Replace("-","").ToLower();
				}
			}

			// New filename that is going to be generated:
			track.storedFileName = track.getTrackName() + ".bin.ecm";
			track.workingFile = Path.Combine(jobData.tempDir, track.storedFileName);
			ecm.ecm(trackFile,track.workingFile);	// old .bin file from wherever it was to temp/bin.ecm
		}
		else
		{
			var ffmp = new FFmpeg();
			ffmp.onComplete = (s) => {
				if(s) {
					deleteOldFile();
					complete();
				}else {
					fail(msg:ffmp.ERROR);
				}
			};
			// New filename that is going to be generated:
			track.storedFileName = track.getTrackName() + ".ogg";
			track.workingFile = Path.Combine(jobData.tempDir, track.storedFileName);
			ffmp.audioPCMToOgg(trackFile, jobData.audioQuality, track.workingFile);
		}
		
	}// -----------------------------------------

	// --
	// Delete old files
	// ONLY IF it's in the TEMP folder!
	void deleteOldFile()
	{
		if(jobData.workFromTemp) {
			File.Delete(trackFile); // Delete it if it was cut from a single bin
		}

	}// -----------------------------------------

}// -- end class
	
}// -- end namespace
