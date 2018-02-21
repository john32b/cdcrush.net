using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
	class EcmTools:ICliReport
	{
		string exe_ecm, exe_unecm;
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
		/// <param name="toolsPath">Where ecm.exe and unecm.exe are</param>
		public EcmTools(string toolsPath = "")
		{
			string binPath = toolsPath;
			exe_ecm = Path.Combine(binPath, "ecm.exe");
			exe_unecm = Path.Combine(binPath, "unecm.exe");

			app = new CliApp(""); // Set executable later

			app.onComplete = (code) =>
			{
				if (code == 0)
				{
					if (onComplete != null) onComplete(true);
				}
				else
				{
					ERROR = "EcmTools error.";
					onComplete(false);
				}
			};

			app.onStdErr = (s) =>
			{
				// Read the progress percent and push it
				var m = Regex.Match(s,@"\s*"+ regexString + @" \((\d{1,3})%");
				if(m.Success) {
					progress = int.Parse(m.Groups[1].Value);
					// Debug.WriteLine("CAPTURED PERCENT  - " +  m.Groups[1].Value);
					if (onProgress != null) onProgress(progress);
				}
			};

		}// -----------------------------------------

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
