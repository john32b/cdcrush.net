using System;

namespace cdcrush.lib.app
{

	/// <summary>
	/// Generic Archiver class
	/// </summary>
	abstract class AbArchiver:ICliReport
	{
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
		public virtual bool compress(string[] listOfFiles,string destinationFile = null)
		{
			return true;
		}// -----------------------------------------
		// Return Preliminary Success, onComplete will report final success
		public virtual bool extractAll(string inputFile, string destinationFolder = null)
		{
			return true;
		}// -----------------------------------------
		// Return Preliminary Success, onComplete will report final success
		public virtual bool extractFiles(string inputFile, string[] listOfFiles, string destinationFolder=null)
		{
			return true;
		}// -----------------------------------------

		// Forcefully end the operation
		public void kill() => proc?.kill();

	}// -- end class

}// --
