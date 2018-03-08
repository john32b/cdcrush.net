using System;
using System.IO;

namespace cdcrush.lib
{
/**
* General Purpose File/Dir related functions
*/
class FileTools
{

	/// <summary>
	/// 
	/// </summary>
	/// <param name="path"></param>
	/// <returns></returns>
	public static bool hasWriteAccess(string path)
	{
		try
		{
			// Attempt to get a list of security permissions from the folder. 
			// This will raise an exception if the path is read only or do not have access to view the permissions. 
			System.Security.AccessControl.DirectorySecurity ds = Directory.GetAccessControl(path);
			return true;
		}
		catch (UnauthorizedAccessException) { return false; }
		catch (NotSupportedException) { return false; }
		catch (IOException) { return false; }
		catch (ArgumentException) { return false; }
	}// -----------------------------------------


	/// <summary>
	/// 
	/// </summary>
	/// <param name="path"></param>
	/// <returns></returns>
	public static bool createDirectory(string path)
	{
		try{
			Directory.CreateDirectory(path);
			return true;
		}
		catch (IOException) { return false; }
		catch (UnauthorizedAccessException) { return false; }
		catch (NotSupportedException) { return false; }
		catch (ArgumentException) { return false; } 
	}// -----------------------------------------

	/// <summary>
	/// 
	/// </summary>
	/// <param name="path"></param>
	/// <param name="newPath"></param>
	/// <returns></returns>
	public static bool tryMove(string path,string newPath=null)
	{
		if (newPath == null) newPath = path + ".old";

		try{
			File.Move(path, newPath);
			return true;
		}
		catch (IOException) { return false; }
		catch (ArgumentException) { return false; }
		catch (UnauthorizedAccessException) { return false; }
		catch (NotSupportedException) { return false; }
	}// -----------------------------------------

	/// <summary>
	/// 
	/// </summary>
	/// <param name="source"></param>
	/// <param name="dest"></param>
	/// <returns></returns>
	public static bool tryCopy(string source,string dest)
	{
		if(source==dest) return true;
		try{
			File.Copy(source,dest);
			return true;
		}
		catch (IOException) { return false; }
		catch (ArgumentException) { return false; }
		catch (UnauthorizedAccessException) { return false; }
		catch (NotSupportedException) { return false; }
	}// -----------------------------------------


	/// <summary>
	/// 
	/// 
	/// </summary>
	/// <param name="path"></param>
	/// <returns></returns>
	public static bool tryDelete(string path)
	{
		try{
			File.Delete(path);
			return true;
		}
		catch (IOException) { return false; }
		catch (ArgumentException) { return false; }
		catch (UnauthorizedAccessException) { return false; }
		catch (NotSupportedException) { return false; }
	}// -----------------------------------------

	/// <summary>
	/// Sanitize a filename,remove illegal characters
	/// </summary>
	/// <param name="input"></param>
	/// <returns></returns>
	public static string sanitizeFilename(string input)
	{
		return String.Concat(input.Split(Path.GetInvalidFileNameChars()));
	}// -----------------------------------------

}// --
}// --
