using System;
using System.IO;
using System.Text.RegularExpressions;

namespace cdcrush.lib.app
{
	/// <summary>
	/// Wrapper for ECM TOOLS
	/// Requires "ecm.exe" and "unecm.exe", delcare where they are at the constuctor
	/// 
	///		"onProgress" => Reports percentage 0 to 100
	/// 
	/// 
	/// </summary>
	class EcmTools:IProcessStatus
	{
		const string EXECUTABLE_ECM = "ecm.exe";
		const string EXECUTABLE_UNECM = "unecm.exe";

		string exe_ecm, exe_unecm; // Final paths

		CliApp app;

		// #CALLBACK on progress updates (0 - 100)
		public Action<int> onProgress {get; set;}
		// OnComplete(Success), read ERROR for errors
		public Action<bool> onComplete { get; set;}
		// --
		public string ERROR {get; private set;}

		// Keep the actual progress
		public int progress {get; private set;}

		// Helper for the regex
		string regexString = "";

		/// <summary>
		/// Create the EcmTools object
		/// </summary>
		/// <param name="exePath">Where ecm.exe and unecm.exe are</param>
		public EcmTools(string exePath = "")
		{
			string binPath = exePath;
			exe_ecm = Path.Combine(binPath, EXECUTABLE_ECM);
			exe_unecm = Path.Combine(binPath, EXECUTABLE_UNECM);

			app = new CliApp(""); // Set executable later

			app.onComplete = (code) =>
			{
				if (code == 0)
				{
					onComplete?.Invoke(true);
				}
				else
				{
					ERROR = "EcmTools error.";
					onComplete?.Invoke(false);
				}
			};

			app.onStdErr = (s) =>
			{
				// Read the progress percent and push it
				var m = Regex.Match(s,@"\s*"+ regexString + @" \((\d{1,3})%");
				if(m.Success) {
					progress = int.Parse(m.Groups[1].Value);
					// Debug.WriteLine("CAPTURED PERCENT  - " +  m.Groups[1].Value);
					onProgress?.Invoke(progress);
				}
			};

		}// -----------------------------------------


		// --
		public void kill() => app.kill();

		/// <summary>
		/// Covert a .bin file to .ecm
		/// </summary>
		/// <param name="input">The file to convert</param>
		/// <param name="output">If ommited ,will place the result in the same folder as the input</param>
		public void ecm(string input, string output = null)
		{
			progress = 0; regexString = "Encoding";
			app.executable = exe_ecm;
			if (output != null) output = '"' + output + '"';
			app.start(string.Format("\"{0}\" {1}", input, output));
		}// -----------------------------------------
		
		/// <summary>
		/// Convert a .ecm file to .bin
		/// </summary>
		/// <param name="input">The file to convert</param>
		/// <param name="output">If ommited ,will place the result in the same folder as the input</param>
		public void unecm(string input,string output = null)
		{
			progress = 0; regexString = "Decoding";
			app.executable = exe_unecm;
			if (output != null) output = '"' + output + '"';
			app.start(string.Format("\"{0}\" {1}", input, output));
		}// -----------------------------------------

	}// -- end class
}// --
