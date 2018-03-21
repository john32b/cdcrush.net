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
		desc = "Joining tracks";
	}// -----------------------------------------

	// --
	public override void start()
	{
		base.start();

		CueReader cd = jobData.cd;

		// --
		if(cd.tracks.Count==1)
		{
			complete();
			return;
		}

		// --
		inputs = new string[cd.tracks.Count-1];
		for(int i=1;i<cd.tracks.Count;i++)
		{
			inputs[i-1] = cd.tracks[i].workingFile;
			cd.tracks[i].workingFile = null;	// Used later when moving, null files won't be moved
		}

		output = cd.tracks[0].workingFile;

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
		
}// --
}// --
