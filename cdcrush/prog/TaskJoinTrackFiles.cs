using cdcrush.lib;
using System;
using System.IO;

namespace cdcrush.prog
{

/// <summary>
/// - Joins all the trackfiles together
/// - Changes track.workingFile from second's track up to null
/// </summary>
class TaskJoinTrackFiles:cdcrush.lib.task.CTask
{
	string[] inputs;
	string output;

	/// <summary>
	/// 
	/// PRE: FILES MUST EXIST!
	/// </summary>
	/// <param name="inputPaths"></param>
	/// <param name="outputFile">It can already exist and inputs will be appended, NULL for new file</param>
	public TaskJoinTrackFiles()
	{
		name = "Join";
		desc = "Joining tracks into a single .BIN";
	}// -----------------------------------------

	// --
	public override void start()
	{
		base.start();

		cd.CDInfos CD = jobData.cd;

		// --
		if(CD.tracks.length==1)
		{
			complete();
			return;
		}

		// --
		inputs = new string[CD.tracks.length-1];
		for(int i=1;i<CD.tracks.length;i++)
		{
			inputs[i-1] = (CD.tracks[i] as cd.CDTrack).workingFile;
			(CD.tracks[i] as cd.CDTrack).workingFile = null;	// Used later when moving, null files won't be moved
		}

		output = (CD.tracks[0] as cd.CDTrack).workingFile;

		int progressStep = (int)Math.Round((double) (100.0f / inputs.Length));

		// NOTE:
		// If I didn't use FileMode.Append , the file would be overwritten
		// It is compatible with new files also.
		FileStream streamWrite = new FileStream(output, FileMode.Append, FileAccess.Write);

		for(int i=0;i<inputs.Length;i++)
		{
			using(FileStream streamRead = File.OpenRead(inputs[i]))
			{
				streamRead.CopyTo(streamWrite);
			}

			if(!CDCRUSH.FLAG_KEEP_TEMP){
				FileTools.tryDelete(inputs[i]);
			}
			PROGRESS += progressStep;
		}

		streamWrite.Close();
		complete();

	}// -----------------------------------------
		
}// -- end class
}// -- end namespace
