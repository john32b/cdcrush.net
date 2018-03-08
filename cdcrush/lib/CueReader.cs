using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Web.Script.Serialization;

namespace cdcrush.lib
{

/**
 * CueReader is the main class to read and operate CUE files
 */
public class CueReader
{
	// :: STATICS

	// Provide compatibility with older versions ( when reading a JSON file )
	public const int VERSION = 3;

	// Supported file formats. Future support for ccd
	public const string SUPPORTED_FORMATS = "cue"; // Separate with |  <-important!

	// --
	static int getSectorsByDataType(string t)
	{
		switch (t)
		{
			case "AUDIO": 		return 2352;	// PCM Audio
			case "CDG" : 		return 2352;	// Karaoke cd+g
			case "MODE1/2048":	return 2048;	// CDROM Mode1 Data (cooked)
			case "MODE1/2352":	return 2352;	// CDROM Mode1 Data (raw)
			case "MODE2/2336":	return 2336;	// CDROM XA Mode2 Data
			case "MODE2/2352":	return 2352;	// CDROM XA Mode2 Data
			case "CDI/2336":	return 2336;	// CDI Mode2 Data
			case "CDI/2352":	return 2352;	// CDI Mode2 Data
			default: return 0; // Why throw an exception? throw new Exception("Unsuported type : " + _type);
		}

	}//------------------------------------


	// :: Saved to JSON ::

	public string CD_TITLE	{ get; set; }
	public string CD_TYPE { get; private set; }
	public string CD_AUDIO_QUALITY {get; set; }		// Describe the audio quality in a string. SET EXTERNALLY.
	public int CD_TOTAL_SIZE { get; private set; }	// Size of all tracks of the CD in bytes
	public int SECTOR_SIZE { get; private set; }
	public bool MULTIFILE {get; private set; } // The CD was read from a CUE with multiple files, (every track own file)
	
	// :: VARS

	// Hold all the tracks
	public List<CueTrack> tracks;

	// -- Loaded CUE file information ::
	public string loadedFile_path {get; private set; }	// full path of the file loaded
	public string loadedFile_dir {get; private set; }	// directory the file belongs to
	public string loadedFile_ext {get; private set; }	// extension without the dot (.), always lowercase
	string[] loadedFile;	// the actual file contents

	// -- Holds the latest error occured so the user can read it
	public string ERROR { get; private set; }

	// -- Parser Helpers ::
	private CueTrack openTrack;	// a track that is being parsed
	private string openFile;	// stores image file filenames

	// -----------------------------------------

	// --
	public CueReader()
	{
	}// -----------------------------------------

	// --
	public override string ToString()
	{
		return string.Format("Title:{0} | Size:{1} | Tracks:{2} | MultiFile : {3}", CD_TITLE, CD_TOTAL_SIZE, tracks.Count, MULTIFILE);
	}// -----------------------------------------

	// --
	// Display some info about the CD and tracks
	public void debugInfo()
	{
		LOG.log(this);
		LOG.line();
		foreach(var tt in tracks) { 
			LOG.log(tt);
		}
		LOG.line();
	}// -----------------------------------------

	/**
	 * Figure out the CD TYPE from the track infos
	 */
	private void getCDTypeFromTracks()
	{
		CD_TYPE = null;
		foreach (var t in tracks) {
			if (t.isData) { CD_TYPE = t.trackType; break; } // break: no need to check other tracks
		}

		if (CD_TYPE == null) {
			CD_TYPE = "AUDIO";
		}

		// NOTE : It was checked whether tracks are of a valid type
		//		  earlier on the cue parser
		SECTOR_SIZE = getSectorsByDataType(CD_TYPE);

	}// -----------------------------------------


	/// <summary>
	/// Get the MD5 of the first data track of the CD
	/// </summary>
	/// <returns></returns>
	public string getFirstDataTrackMD5()
	{
		// LINQ
		if(tracks==null) return null;
		return tracks.Where(t=>t.isData).FirstOrDefault()?.md5;
	}// -----------------------------------------
	

	/// <summary>
	/// Load a descriptor file
	/// The CD title is read from the .CUE filename
	/// Also checks if files declared inside the .CUE exist or not
	/// </summary>
	/// <param name="file">Filename to load [.cue]</param>
	/// <returns>Success ( read the ERROR Property )</returns>
	public bool load(string file)
	{		
		if (!File.Exists(file))
		{
			ERROR = "File:" + file + "does not exist"; return false;
		}

		// Vars init
		loadedFile_path = file;
		loadedFile_dir = Path.GetDirectoryName(loadedFile_path);
		loadedFile_ext = Path.GetExtension(loadedFile_path).ToLower().Trim('.');

		// This is redudant..
		if (!SUPPORTED_FORMATS.Contains(loadedFile_ext))
		{
			ERROR = "File Type is not supported.- Supported formats : " + SUPPORTED_FORMATS.ToString(); return false;
		}

		try {
			loadedFile = File.ReadAllLines(file);
		}
		catch (IOException) {
			ERROR = "There was an I/O problem loading the file"; return false;
		}

		// Init vars
		CD_TOTAL_SIZE = 0;
		CD_TITLE = "untitled CD"; SECTOR_SIZE = 0; CD_TYPE = null;
		tracks = new List<CueTrack>();
		openTrack = null; openFile = null;

		// Try to capture the CD TITLE from the CUE filename
		Match m1 = Regex.Match(loadedFile_path, @"([^\/\\]*)\.(?:"+SUPPORTED_FORMATS+@")$", RegexOptions.IgnoreCase);
		if (m1.Success) CD_TITLE = m1.Groups[1].Value;
			
		// Start Parsing the file based on it's type
		Func<string,int> parser;
		switch (loadedFile_ext)
		{
			case "cue": parser = cue_parser; break;
			case "ccd": parser = ccd_parser; break;
			default: parser = cue_parser; break;
		}

		// Parser ::

		for (int c = 0; c < loadedFile.Length; c++)
		{
			loadedFile[c] = loadedFile[c].Trim(); // Trim whitespaces
			if (loadedFile[c].Length == 0) continue; // Skip blank lines
			if (loadedFile[c] == "\n") continue;

			if (parser(loadedFile[c]) == 0)
			{
				ERROR = String.Format("Parse Error at line[{0}] : {1}", c+1, ERROR);
				return false;
			}
		}

		// -- POST PARSE CHECK ::

		if (tracks.Count == 0)
		{
			ERROR = "No Tracks in the CUE file"; return false;
		}

		getCDTypeFromTracks();

		// :: Some Debug Info
		LOG.log("[CUEREADER] : Loading : `{0}`", loadedFile_path);
		LOG.log("[CUEREADER] : Title : `{0}`, Type: `{1}`, NumberOfTracks:{2}", CD_TITLE, CD_TYPE, tracks.Count );
		LOG.indent(1);
		// --

		if(loadedFile_ext=="ccd")
		{
			// TODO: CCD
		}

		// :: Go through each and every track, regardless if multitrack or not,
		//	Check for every single one of the files if exist or not
		//	Also count the number of file images to figure out `multifilecd`

		int cc = 0; // Number of tracks with DiskFiles found
		foreach(var tr in tracks)
		{
			if(tr.indexes.Count==0) {
				ERROR = "Track [" + tr.trackNo + "] has no indexes defined"; return false;
			}

			if (tr.trackFile == null) continue;

			cc++;

			tr.workingFile = Path.Combine(loadedFile_dir, tr.trackFile);

			// Check the diskfiles
			if (!File.Exists(tr.workingFile)) {
				ERROR = "Image \"" + tr.trackFile + "\" does not exist"; return false;
			}

			// Get Sizes
			var finfo = new FileInfo(Path.Combine(loadedFile_dir, tr.trackFile));
			tr.byteSize = (int)finfo.Length; // it can't be more than 800MB, so it's safe
			tr.sectorSize = (int)Math.Ceiling((double)(tr.byteSize / SECTOR_SIZE));

			// --
			if(tr.sectorSize<=0) {
				// Rare but worth checking 
				ERROR = "DiskFile " + tr.trackFile + " is currupt"; return false;
			}

			CD_TOTAL_SIZE += tr.byteSize;

		}// -- end each track


		// :: POST PARSE CALCULATIONS AND CHECKS ::


		// - Is it MultiTrack with MultiFiles?
		if (cc == tracks.Count && cc > 1) { 
			LOG.log("+ MULTI-FILE Image CD");
			MULTIFILE = true;
			// Need to set sectorStart at each tracks;
			calculateTracksSectorStart();

		}else if (cc==1) {
			LOG.log("+ SINGLE-FILE Image CD");
			MULTIFILE = false;
			// In case an single was found but not at the first track:
			if(tracks[0].trackFile == null) {
				ERROR = "First track must declare an image file"; return false;
			}

			var imageSectorSize = tracks[0].sectorSize;
			//Calculate tracks, starting from the end, backwards to 0
			var c = tracks.Count - 1;
			//Calculate last track manually, out of the loop
			tracks[c].calculateStart();
			tracks[c].sectorSize = imageSectorSize - tracks[c].sectorStart;
			while(--c >= 0) {
				tracks[c].calculateStart();
				tracks[c].sectorSize = tracks[c + 1].sectorStart - tracks[c].sectorStart;
			}

			calculateTracksByteSize();

		}else if(cc==0) {
			ERROR = "There are no image files declared in the sheet."; return false;
		}else {
			// Rare, and I don't know if anything does this
			ERROR = "Multiple Image sheets are restricted to one Track per Image only"; return false;
		}

		LOG.log("+ Total CD SIZE : {0}", CD_TOTAL_SIZE); 
		foreach(var tt in tracks) LOG.log(tt);
		LOG.line(); LOG.indent(0);
		return true;
	}// -----------------------------------------

	// -- Fills `SectorStart`
	// Used when loading old version JSON files, or when reading multi file CUE 
	void calculateTracksSectorStart()
	{
		if (tracks.Count == 0) return;

		// - SectorStart
		int last = 0;
		for(int i=0;i<tracks.Count;i++) 
		{
			tracks[i].sectorStart = last;
			last += tracks[i].sectorSize;
		}
	}// -----------------------------------------
	// --Fills `byteSize`
	// Used when loading old version JSON files, or when reading single image CUE
	void calculateTracksByteSize()
	{
		if (tracks.Count == 0) return;
		for(int i=0;i<tracks.Count;i++) 
		{
			tracks[i].byteSize = tracks[i].sectorSize * SECTOR_SIZE;
		}
	}// -----------------------------------------
	/*
	 * Reads a CUE file, line by line, used in load();
	 * CUE SHEET INFO : http://wiki.hydrogenaud.io/index.php?title=Cue_sheet
	 * This is not a complete Cue Parser, just a basic one 
	 * that can read game disks. A track of data and tracks of CDDA audio.
	 */
	int cue_parser(string line)
	{
		// Get FILE image name
		if (line.ToUpper().StartsWith("FILE"))
		{
			openTrack = null; // Close any open Track

			// Try to get the filename within the quotes after FILE
			var ff = Regex.Split(line, "[\"\']");

			if (ff.Length < 3)
			{
				// Because Array should be like [] {"FILE","GAME NAME.BIN", ... ,"BINARY"}
				ERROR = "Could not get filename";
				return 0;
			}

			// [SAFEGUARD]
			if(!new string[] {"BINARY","WAVE","MP3"}.Contains( ff.Last().ToUpper().Trim()))
			{
				ERROR = "Unsupported File Type, or identifier missing. Supported = {BINARY,WAVE,MP3}";
				return 0;
			}

			// First and last elements are not needed
			openFile = string.Join("'", ff, 1, ff.Length - 2);

			return 1; // No reason to check for other cases
		}// -- FILE


		// Get Track NO and track TYPE
		if (line.ToUpper().StartsWith("TRACK"))
		{
			openTrack = null; // Close any open Track

			Match m = Regex.Match(line, @"^\s*TRACK\s+(\d+)\s+(\S+)", RegexOptions.IgnoreCase);
			if (m.Success)
			{
				int trackNo = int.Parse(m.Groups[1].Value);
				string trackType = m.Groups[2].Value.ToUpper();

				// [SAFEGUARD] Check to see if Track Type is valid
				if (getSectorsByDataType(trackType) == 0)
				{
					ERROR = "Unsupported TRACK type:" + trackType; return 0;
				}

				// [SAFEGUARD] Check to see if the trackNO is already defined in the tracks
				foreach (var t in tracks) {
					if (t.trackNo == trackNo) {
						ERROR = "TRACK Number already defined"; return 0;
					}
				}

				CueTrack tr = new CueTrack(trackNo, trackType);
				openTrack = tr;
				tr.trackFile = openFile; openFile = null;
				tracks.Add(tr);		
			}
			else {
				ERROR = "Cannot parse TRACK line"; return 0;
			}

			return 1;
		}// -- TRACK

		// --
		if (line.ToUpper().StartsWith("INDEX"))
		{
			if (openTrack == null) { ERROR = "INDEX not inside a TRACK"; return 0; }

			Match m = Regex.Match(line, @"^\s*INDEX\s+(\d+)\s+(\d{1,2}):(\d{1,2}):(\d{1,2})", RegexOptions.IgnoreCase);
			if (m.Success)
			{
				var indexNo = int.Parse(m.Groups[1].Value);
				if (openTrack.indexExists(indexNo)){
					ERROR = String.Format("Duplicate Index on track {0}", openTrack.trackNo); return 0;
				}
				openTrack.addIndex(indexNo,
						int.Parse(m.Groups[2].Value),
						int.Parse(m.Groups[3].Value),
						int.Parse(m.Groups[4].Value));
			}
			else {
				ERROR = "Cannot parse INDEX line"; return 0;
			}
			return 1;
		}// -- INDEX

		// --
		if (line.ToUpper().StartsWith("PREGAP")) 
		{
			if (openTrack == null) { ERROR = "INDEX not inside a TRACK"; return 0; }

			Match m = Regex.Match(line, @"^\s*PREGAP\s+(\d{1,2}):(\d{1,2}):(\d{1,2})", RegexOptions.IgnoreCase);
			if (m.Success)
			{
				openTrack.setGap(int.Parse(m.Groups[1].Value),
					int.Parse(m.Groups[2].Value),
					int.Parse(m.Groups[3].Value));
			}
			else {				
				ERROR = "Cannot parse PREGAP line"; return 0;
			}
			return 1;
		}// - PREGAP

		// --
		// Title can be either at ROOT is used as the CD title, or inside a Track
		if (line.ToUpper().StartsWith("TITLE"))
		{
			// Currently only used as CD title, and only works with titles inside ""
			if (openTrack == null) {
				Match m = Regex.Match(line,".+\"(.+)\"$");
				if (m.Success) {
					CD_TITLE = m.Value;
				}
				else {
					ERROR = "Can't parse TITLE"; return 0;
				}

			}
		}// -- TITLE

		// Skip Comments
		// if (line.ToUpper().StartsWith("REM")) return 1; -- Everything else will return 1

		return 1;
	}// -----------------------------------------

	// --
	int ccd_parser(string line)
	{
		// TODO: Build this class
		return 1;
	}// -----------------------------------------

// --
	public void convertMultiToSingle()
	{
		// TODO:
	}// -----------------------------------------


	/// <summary>
	/// Load a previously saved JSON settings file
	/// </summary>
	/// <param name="file">The JSON file to load</param>
	/// <returns>Success 1 OK,0 ERROR</returns>
	public bool loadJson(string file)
	{
		if (!File.Exists(file)) {
			ERROR = "File:" + file + "does not exist";
			return false;
		}

		string loadedJSONText;

		try{
			loadedJSONText = File.ReadAllText(file);
		}
		catch(IOException e)
		{
			ERROR = e.ToString(); return false;
		}
		catch(UnauthorizedAccessException)
		{
			ERROR = "Unauthorized Access"; return false;
		}

		// -- JSON Get
		var js = new JavaScriptSerializer();
		// Note: Should never throw error because it was created internally
		Dictionary<string,object> data = js.Deserialize<Dictionary<string,object>>(loadedJSONText);
		
		// -- Version check, defaults to 1 if field missing
		int versionLoaded = 1;
		if (data.ContainsKey("version")) versionLoaded = int.Parse(data["version"].ToString());
	
		// -- Backwards Compatibility ::
		switch(versionLoaded)
		{
			case 1: 
				// Convert V1 to V2
				int ssize = (int)data["sectorSize"];
				foreach(Dictionary<string,object> t in (ArrayList)data["tracks"]) {
					t["diskFileSize"] = (int)t["sectorSize"] * ssize;
					t["diskFile"] = null;
					t["isData"] = (bool) !((t["type"] as string) == "AUDIO");
					
				}
				goto case 2;

			case 2:
				// Convert V2 to V3
				string CDT=null;	// If any is data, then set to the data type.
				int diskFiles=0;	// Count tracks with diskfiles set
				bool anyAudio=false;// Is there any audio track?
				foreach(Dictionary<string,object> t in (ArrayList)data["tracks"]) {
						t["trackType"] = t["type"];
						t["indexes"] = t["indexAr"];// Array is the same
						t["storedFileName"] = t["filename"];
						t["byteSize"] = t["diskFileSize"];
						t["md5"] = ""; // New field
						if(!anyAudio && t["trackType"].ToString()=="AUDIO") anyAudio = true;
						// Capture the first data format of any track:
						if((bool)t["isData"] && CDT==null) CDT = t["trackType"].ToString();
						if(t["diskFile"]!=null){ diskFiles++; }
					}

				// New/Renamed properties :
				data["audio"] = anyAudio ? "??? kbps" : "";
				data["cdType"] = CDT ?? "AUDIO";
				data["totalSize"] = data["imageSize"];
				data["multiFile"] = (diskFiles>1) && (diskFiles == (data["tracks"] as ArrayList).Count);
				break;

			default: break;
		}

		// -- At this point the data is properly in v3 format.


		// -- Read tracks ::
		tracks = new List<CueTrack>();
		foreach(Dictionary<string,object> t in (ArrayList)data["tracks"]) {
			var tr = new CueTrack((int)t["trackNo"], t["trackType"].ToString());
			tr.sectorSize = (int)t["sectorSize"];
			tr.sectorStart = (int)t["sectorStart"];
			tr.byteSize = (int)t["byteSize"];
			tr.pregapMinutes = (int)t["pregapMinutes"];
			tr.pregapSeconds = (int)t["pregapSeconds"];
			tr.pregapMillisecs = (int)t["pregapMillisecs"];
			tr.storedFileName = (string)t["storedFileName"];
			tr.md5 = (string)t["md5"];

			foreach(Dictionary<string,object> ind in (ArrayList)t["indexes"]) {
				tr.addIndex((int)ind["no"], (int)ind["minutes"], (int)ind["seconds"], (int)ind["millisecs"]);
			}

			tracks.Add(tr);
		}

		// -- cd info data ::
		CD_TITLE = data["cdTitle"].ToString();
		CD_TYPE = data["cdType"].ToString();
		CD_TOTAL_SIZE = (int) data["totalSize"];
		CD_AUDIO_QUALITY = data["audio"].ToString();
		MULTIFILE = (bool)data["multiFile"];
		SECTOR_SIZE = (int) data["sectorSize"];

		// Some Checks ::
		if (tracks[0].byteSize == 0) calculateTracksByteSize(); // Single File Multi Tracks
		if (tracks.Count > 1 && tracks[1].sectorStart == 0) calculateTracksSectorStart(); // Multi File Multi Track

		return true;
	}// -----------------------------------------


	/// <summary>
	/// Save the information read from the CUE file to a json file,
	/// it provides more data than the CUE itself, as it has some extra properties
	/// </summary>
	/// <param name="file">Full path of the file to be written</param>
	/// <returns>Success 1 OK,0 ERROR</returns>
	public bool saveJson(string filePath)
	{
		// Some Safeguards::
		if(tracks.Count==0){
			ERROR = "No tracks to save"; return false;
		}
	
		#if DEBUG
		foreach(var tr in tracks) {
			if(tr.storedFileName == null) {
				ERROR = String.Format("Track no({0}) doesn't have `storedFileName` set. ERROR", tr.trackNo); return false;
			}
		}
		#endif

		// Build the data that is going to be written
		var data = new
		{
			version = VERSION,
			cdTitle = CD_TITLE,
			cdType = CD_TYPE,
			audio = CD_AUDIO_QUALITY,
			sectorSize= SECTOR_SIZE,
			totalSize = CD_TOTAL_SIZE,
			multiFile = MULTIFILE,
			tracks
		};

		// Serialize
		var js = new JavaScriptSerializer();

		try{
			File.WriteAllText(filePath, js.Serialize(data));
		}
		catch(IOException e)
		{
			ERROR = e.ToString(); return false;
		}
		catch(UnauthorizedAccessException)
		{
			ERROR = "Unauthorized Access"; return false;
		}

		return true;
	}// -----------------------------------------

	/// <summary>
	/// Save the CD info in this class to a CUE file
	/// </summary>
	/// <returns>Success 1 OK,0 ERROR</returns>
	public bool saveCUE(string outputFile, string comment=null)
	{
		// Quick way to figuring out if there is any data
		if (tracks == null || tracks.Count == 0) {
			ERROR = "There is no data to save"; return false;
		}

		string data = "";

		CueTrack tr; // temp

		for(int i=0;i<tracks.Count;i++)
		{
			tr = tracks[i];

			if(tr.trackFile != null) 
			{
				string fileType = "BINARY";
				switch(Path.GetExtension(tr.trackFile).ToLower()){
					case ".ogg" : fileType = "OGG"; break;
					case ".flac" : fileType = "FLAC"; break;
					case ".mp3" : fileType = "MP3"; break;
					default : break;
				}

				data += String.Format("FILE \"{0}\" {1}\n", tr.trackFile, fileType);
			}

			data += String.Format("\tTRACK {0:00} {1}\n", tr.trackNo , tr.trackType);

			if (tr.hasPregap()){
				data += String.Format("\t\tPREGAP {0}\n", tr.getPregapString());
			}

			for (int t = 0; t < tr.indexes.Count; t++) {
				var ind = tr.indexes[t];
				data += string.Format("\t\tINDEX {0:00} {1:00}:{2:00}:{3:00}\n", 
										ind.no, ind.minutes, ind.seconds, ind.millisecs);
			}

		}// --

		// - Save engine info
		data += string.Format("REM ------------------------------\n");
		data += string.Format("REM CDCRUSH Version : {0}\n", prog.CDCRUSH.PROGRAM_VERSION);
		data += string.Format("REM Audio Quality : {0}\n", CD_AUDIO_QUALITY);

		if (comment != null)
		{
			data += "REM " + comment;
		}

		try
		{
			File.WriteAllText(outputFile, data);
		}
		catch (IOException e)
		{
			ERROR = e.ToString(); return false;
		}
		catch (UnauthorizedAccessException)
		{
			ERROR = "Unauthorized Access"; return false;
		}

		return true;
	}// -----------------------------------------

}// -- end CueReader





/**
 * Cue Track describes a single track inside a Cue File
 */
public class CueTrack
{
	[ScriptIgnore]
	public bool isData;

	public string md5;			// Keep the md5 value if it's a DATA track
	public int trackNo;
	public string trackType;	// Type of the track, e.g. "mode2/2352"
	public List<CueIndex> indexes;
	public int sectorSize;	// Converted value, length of this sector in ints
	public int sectorStart;	// Converted value, start from the CD start
	public int byteSize;	// Total size of the RAW image portion or file
	public int pregapMinutes;
	public int pregapSeconds;
	public int pregapMillisecs;
	public string storedFileName = null;	// Keep the CRUSHED track filename. e.g. track02.ogg

	//:: These are only used when READING a CUE FILE ::
	[ScriptIgnore]
	// What FILE this track is associated to. Just the filename. (READ FROM CUE) (WRITTEN TO CUE)
	// MUST BE GENERATED BY USER when written to CUE
	public string trackFile = null;

	[ScriptIgnore]
	// Full path of the working file. TEMP VAR.
	// Used in CRUSH/RESTORE operations, store pathnames etc.
	public string workingFile = null;	
	// -----------------------------------------

	// --
	public CueTrack(){} // Needed for the JSON deserializer
	public CueTrack(int _no, string _type = null) 
	{
		indexes = new List<CueIndex>();
		trackNo = _no;
		trackType = _type; // Note, CUE reader sets this to upper
		isData = (_type != "AUDIO");
	}// -----------------------------------------

	// --
	public void addIndex(int _no, int _minutes, int _seconds, int _millisecs)
	{
		indexes.Add(new CueIndex { no = _no, minutes = _minutes, seconds = _seconds, millisecs = _millisecs });
	}// -----------------------------------------

	// Generate the index based on the sector size, used in CCD and converting multi to single
	public void addIndexBySector(int _index, int len)
	{
		int mm = (int)Math.Floor(len / 4500f);
		int ss = (int)Math.Floor((len % 4500f) / 75);
		int ms = (int)((len % 4500f) % 75);
		addIndex(_index, mm, ss, ms);
	}// -----------------------------------------

	// Used when converting multifile to singlefile
	// Writes new times based on `sectorStart`
	public void setNewTimesBasedOnSector()
	{
		var old = indexes.ToList();
		indexes.Clear();

		// First Index is mandatory
		addIndexBySector(0, sectorStart);
		
		// Check to see if there are more indexes	
		for (int i = 1; i < old.Count; i++)
		{
			addIndexBySector(i, sectorStart + old[i].toSector());
			// e.g. 00:02:00 is 150 sectors, so it's going to be 
			// (sectorstart+150)-> translated to time again
		}
	}// -----------------------------------------

	// Used when converting singleFile to multiFile
	// Resets the first index to 0:0:0 and calculates the next indexes
	public void setNewTimesReset()
	{
		if(indexes.Count==0) return; // Should NEVER happen

		var ind = indexes.ToList();
			indexes.Clear();

		// Keep the master time
		var d1 = new DateTime(2000,1,1,0,ind[0].minutes,ind[0].seconds,ind[0].millisecs);
		
		// Zero out first index
		addIndex(0,0,0,0);
			
		for(int c=1;c<ind.Count;c++)
		{
			var d2 = new DateTime(2000,1,1,0,ind[c].minutes,ind[c].seconds,ind[c].millisecs);
			var diff = d2 - d1;
			addIndex(c,diff.Minutes,diff.Seconds,diff.Milliseconds);
		}

	}// -----------------------------------------

	// --
	public bool indexExists(int _index)
	{
		return indexes.Any(v => v.no == _index);
	}// -----------------------------------------

	// --
	public void setGap(int mm, int ss, int ms)
	{
		pregapMinutes = mm;
		pregapSeconds = ss;
		pregapMillisecs = ms; 
	}// -----------------------------------------

	// This is for singleFile sheets, calculates the sector which the track starts
	// Based on the index time
	public void calculateStart()
	{
		sectorStart = indexes[0].toSector();
	}// -----------------------------------------

	public string getTrackName()
	{
		return "track" + trackNo.ToString("D2");
	}// -----------------------------------------

	public string getFilenameRaw()
	{
		return getTrackName() + (isData ? ".bin" : ".pcm");
	}// -----------------------------------------

	public string getPregapString() 
	{
		return String.Format("{0:00}:{1:00}:{2:00}", pregapMinutes, pregapSeconds, pregapMillisecs);
	}// -----------------------------------------

	public bool hasPregap() 
	{
		return (pregapMillisecs > 0 || pregapSeconds > 0 || pregapMinutes > 0);
	}// -----------------------------------------
	// --
	public override string ToString()
	{
		return String.Format(@"Track No:{0:00} | TYPE:{1} | SecStart:{2} | SecLen:{3} | Size:{4} | MD5:{5}", trackNo, trackType, sectorStart, sectorSize,byteSize,md5);
	}// -----------------------------------------

}// -- end CueTrack


	// -- Helper
	public struct CueIndex {
		public int no, minutes, seconds, millisecs;
		public int toSector() {
			int sector = minutes * 4500;
				sector += seconds * 75;
				sector += millisecs;
			return sector;
		}// --
	}// -- 


}// -- end namespace
