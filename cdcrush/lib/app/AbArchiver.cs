using System;

namespace cdcrush.lib.app
{
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
				if (onProgress != null) onProgress(_progress);
			}
		}

		public string ERROR {get; protected set;}
		public Action<int> onProgress { get; set; }
		public Action<bool> onComplete { get; set;} // OnComplete(Success), read ERROR for errors

		// Return Preliminary Success
		public virtual bool compress(string[] listOfFiles,string destinationFile = null)
		{
			return true;
		}// -----------------------------------------
		// Return Preliminary Success
		public virtual bool extractAll(string inputFile, string destinationFolder = null)
		{
			return true;
		}// -----------------------------------------
		// Return Preliminary Success
		public virtual bool extractFiles(string inputFile, string[] listOfFiles, string destinationFolder=null)
		{
			return true;
		}// -----------------------------------------


	}// -- end class

}// --
