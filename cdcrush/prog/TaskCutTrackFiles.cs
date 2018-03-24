using System;
using System.IO;
using cdcrush.lib;

namespace cdcrush.prog
{

/// <summary>
/// - Cut the CD BIN to multiple trackfiles
/// 
/// CHANGES : 
/// - track.workingFile : is set to the new cut track full path
/// 
/// </summary>
class TaskCutTrackFiles:cdcrush.lib.task.CTask
{

	// The main image file of the CD
	string input;

	// --
	public TaskCutTrackFiles():base()
	{
		name = "Cut";
		desc = "Cutting tracks into separate files";
	}// -----------------------------------------

	// --
	// Gets called regardless of number of tracks:
	public override void start()
	{
		base.start();

		// Get the global cd object
		CueReader cd = jobData.cd;
		input = cd.tracks[0].workingFile;

		// No need to cut an already cut CD
		// Multifiles `workingfile` is already set to proper
		if(cd.MULTIFILE)
		{
			complete();
			return;
		}

		// No need to copy the bytes to the temp folder, just work from the source
		if(cd.tracks.Count==1)
		{
			complete();
			return;
		}


		int progressStep = (int)Math.Round((double) (100 / cd.tracks.Count));

		// -- Cut the tracks ::

		FileStream inStr = new FileStream(input, FileMode.Open, FileAccess.Read);

		for (int i = 0; i < cd.tracks.Count; i++)
		{
			var track = cd.tracks[i];

			int byteStart = track.sectorStart * cd.SECTOR_SIZE;
			int byteLen = track.sectorSize * cd.SECTOR_SIZE;

			// New filename for the tracks
			track.workingFile = Path.Combine(jobData.tempDir,track.getFilenameRaw());
			FileStream outStr = new FileStream(track.workingFile, FileMode.CreateNew, FileAccess.Write);
			copyStream(inStr,outStr,byteStart,byteLen);
			outStr.Dispose();
			PROGRESS += progressStep;
		}// --

		inStr.Dispose();

		complete();
	}// -----------------------------------------


	// stackoverflow.com/questions/13021866/any-way-to-use-stream-copyto-to-copy-only-certain-number-of-bytes
	//
	public void copyStream(Stream input, Stream output, int start, int bytes)
	{
		byte[] buffer = new byte[32768];
		int read;
		input.Position = start;
		while (bytes > 0 && 
			   (read = input.Read(buffer, 0, Math.Min(buffer.Length, bytes))) > 0)
		{
			output.Write(buffer, 0, read);
			bytes -= read;
		}
	}// -----------------------------------------
}//--
}//--
