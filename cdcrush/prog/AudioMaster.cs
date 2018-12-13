using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cdcrush.lib.app;

namespace cdcrush.prog
{

/// <summary>
/// An audio wrapper for all audio encoders used Ffmpeg, Tak
/// Provides generalized functionality for encoding/decoding audio
/// - Also provides universal Audio Codec IDs for use in forms
/// </summary>
class AudioMaster
{
	// List of all codecIDs. Every codecID is understood by this class
	public static readonly string[] codecs = new [] { "VORBIS" , "OPUS", "MP3" , "FLAC", "TAK"};
	
	static readonly string[] lossyAudioExt = new [] { ".ogg", ".mp3" };

	/// <summary>
	/// Returns Array with String Info of Each Index, Default Index
	/// </summary>
	/// <param name="codecID">CodecID as it is on the `codecs` static array</param>
	/// <returns>Array with Quality Information, Default Quality Index</returns>
	public static Tuple<string[],int> getQualityInfos(string codecID)
	{
		// There are just two codecs, so I am doing this check by hand
		if(codecID=="TAK")
		{
			return Tuple.Create(new string[0],0);
		}

		// It must be a codecID from FFMPEG I am not going to check
		var cod = FFmpeg.getCodecByID(codecID);
		return Tuple.Create(cod.getQualityInfos(),cod.qualityDefault);
	}// --

	/// <summary>
	/// Return full Codec name, based on an ID
	/// </summary>
	/// <param name="codecID"></param>
	/// <returns></returns>
	public static string getCodecIDName(string codecID)
	{
		if(codecID=="TAK") return "Tak Lossless";
		// Should be FFmpeg
		return FFmpeg.getCodecByID(codecID).name;
	}// --


	/// <summary>
	/// Get an informative string with Codec Name + 
	/// </summary>
	/// <param name="q"></param>
	/// <returns></returns>
	public static string getCodecSettingsInfo(Tuple<string,int> A)
	{
		var name = getCodecIDName(A.Item1);
		var qinfos = getQualityInfos(A.Item1).Item1;
		if(qinfos.Length>0) {
			return name + " " + qinfos[A.Item2];
		}
		return name;
	}// --

	public static string getCodecExt(string codecID)
	{
		if(codecID=="TAK") return ".tak";
		else return FFmpeg.getCodecByID(codecID).ext;
	}// --


	/// <summary>
	/// Is a file lossy or not
	/// </summary>
	/// <param name="ext"></param>
	/// <returns></returns>
	public static bool isLossyByExt(string ext)
	{
		return Array.Exists(lossyAudioExt,(s) => { 
			return s == ext.ToLower();
		});
	}



}// - end class

}// - end namespace
