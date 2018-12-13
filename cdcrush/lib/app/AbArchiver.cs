using System;

namespace cdcrush.lib.app
{

	/// <summary>
	/// Generic Archiver class
	/// </summary>
	public abstract class AbArchiver:IProcessStatus
	{
		// user set, whether to create solid archives when using compress
		// NON Solid archives can have single files extracted way faster
		public bool SOLID;

		// This will be auto-set whenever compress() is complete and returns TRUE
		// This value will be read to whatever the archiver reports to the stdout
		public long COMPRESSED_SIZE;

		// Hold the current operation ID ("compress","restore")
		protected string operation;

		protected CliApp proc;
		protected int _progress; // 0 - 100
		public int progress {
			get {
				return _progress;
			}
			set{
				_progress = value;
				onProgress?.Invoke(_progress);
			}
		}

		public string ERROR {get; protected set;}
		public Action<int> onProgress { get; set; }
		public Action<bool> onComplete { get; set;} // OnComplete(Success), read ERROR for errors

		// Return Preliminary Success, onComplete will report final success
		// Note: compressionLevel or CompressinString, one or the other
		public virtual bool compress(string[] listOfFiles,string destinationFile, int compressionLevel, string compressionString)
		{
			return true;
		}// -----------------------------------------

		/// <summary>
		/// Extract `Input file` to `Destination Folder`
		/// If `filesToExtract` is set, then will try to extract these files only from withing the archive
		/// </summary>
		/// <param name="inputFile"></param>
		/// <param name="destinationFolder"></param>
		/// <param name="filesToExtract"></param>
		/// <returns></returns>
		public virtual bool extract(string inputFile, string destinationFolder, string[] filesToExtract = null)
		{
			return true;
		}

		// Append files to an archive, Usually using the fastest compression mode for fast recovery
		public virtual bool append(string archive,string[] files)
		{
			return true;
		}

		// Forcefully end the operation
		public void kill() => proc?.kill();

	}// -- end class

}// --
