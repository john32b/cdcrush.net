using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace cdcrush.lib.app
{

/**
 * Generic CLI application spawner
 */
class CliApp
{
	// The main process object
	public Process proc {get; private set;}

	// Path to the executable, set directly to the process object
	public string executable { 
		get { return proc.StartInfo.FileName; }
		set { proc.StartInfo.FileName = value; } 
	}
	
	// #USERSET::
	public Action<int> onComplete;  // 0 = OK, 1 = Error, 2 = Could not run EXE
	public Action<string> onStdOut;
	public Action<string> onStdErr;

	public Action<string> onStdOutWord;	// Used if you enable HACK_STDOUT_RAW on the constructor
										// Will push out words read from StdOut.

	// --
	StringBuilder builderStdOut;
	StringBuilder builderStdErr;

	public string stdOutLog { get{ return builderStdOut.ToString(); } }
	public string stdErrLog { get{ return builderStdErr.ToString(); } }

	// Helpers
	private bool _hasStarted = false;

	// When some CLI apps don't flush their stdOut, capture it raw word by word.
	private bool HACK_STDOUT_RAW = false;

	// -----------------------------------------
	/// <summary>
	/// Starts a CLI app in a new thread
	/// </summary>
	/// <param name="exec">Path to the executable</param>
	/// <param name="enableStdOutHack">Enable Raw word capture from stdOut. Use `onStdOutWord` to capture</param>
	public CliApp(string exec,bool enableStdOutHack = false)
	{
		proc = new Process();
		executable = exec; // uses setter
		proc.StartInfo.UseShellExecute = false; // needed for executables!
		proc.StartInfo.CreateNoWindow = true;
		proc.StartInfo.RedirectStandardOutput = true;
		proc.StartInfo.RedirectStandardError = true;

		builderStdOut = new StringBuilder();
		builderStdErr = new StringBuilder();

		HACK_STDOUT_RAW = enableStdOutHack;

		if(!HACK_STDOUT_RAW)
		{
			proc.OutputDataReceived += (sender, e) =>
			{
				builderStdOut.Append(e.Data);
				if(!String.IsNullOrEmpty(e.Data) && onStdOut!=null)
					onStdOut(e.Data);
			};

			proc.ErrorDataReceived += (sender, e) =>
			{
				builderStdErr.Append(e.Data);
				if(!String.IsNullOrEmpty(e.Data) && onStdErr!=null)
					onStdErr(e.Data);
			};
		}
	}// -----------------------------------------

	// --
	~CliApp()
	{
		kill();
	}// -----------------------------------------	

	// --
	public void kill()
	{
		if(_hasStarted && !proc.HasExited) proc.Kill();
		//proc.Dispose();
	}// -----------------------------------------

	/// <summary>
	/// Start a thread in a new thread, it will keep going
	/// </summary>
	/// <param name="args">Arguments</param>
	public void start(string args = null,string workingDir = null)
	{
		proc.StartInfo.WorkingDirectory = workingDir; // Shoudl I make it System.Environment.CurrentDirectory ??
		proc.StartInfo.Arguments = args;
		
		LOG.log("[CLI.APP] : start() : {0} {1}", executable, proc.StartInfo.Arguments);

		// Note: I really need a Thread here, If I don't, it will lock the main thread and forms
		var th = new Thread(new ThreadStart(

			 () => {

				try{
					proc.Start();
				}catch(System.ComponentModel.Win32Exception){
					// Could not find the executable
					builderStdErr.Append($"Problem running {executable}");
					onComplete?.Invoke(2);
					return;	
				}

				_hasStarted = true;
				 
				if(HACK_STDOUT_RAW && onStdOutWord!=null)
				{
					int byte_r  = 0;
					StringBuilder word = new StringBuilder();

					while( (byte_r = proc.StandardOutput.BaseStream.ReadByte()) > -1 )
					{	
						if(byte_r==32 || byte_r==13) // SPACE or ENTER
						{
							if(word.Length>0)
							{
								onStdOutWord(word.ToString());
								word.Clear();
							}

						}else
						{
							if(byte_r>32) // No special characters in the stringbuilder
							{
								word.Append(Char.ConvertFromUtf32(byte_r));
							}
						}
					} // end while
				}//--

				if(!HACK_STDOUT_RAW) // Didn't use else because could be (hack=true, onStdOutWord=false)
				{
					proc.BeginErrorReadLine();
					proc.BeginOutputReadLine();
				}// -

				proc.WaitForExit();

				LOG.log("[CLI.APP] : Process \'{0}\' Exited with code {1}", executable, proc.ExitCode);
				onComplete?.Invoke(proc.ExitCode);

			 }));

		th.IsBackground = true;
		th.Name = $"cliApp({executable})";
		th.Start();
	}// -----------------------------------------


	/**
	 * Quickly create a CLI APP and return it
	 */
	static public CliApp quickStart(string filename, string args = null,Action<int> OnComplete = null)
	{
		var c = new CliApp(filename);
			c.onComplete = OnComplete;
			c.start(args);
		return c;
	}// -----------------------------------------

	
	/**
	 * Quickly create a CLI APP in SYNC and return its stdOut and stdErr
	 * Returns [ stdOut, stdErr, ExitCode ]
	 */
	static public string[] quickStartSync(string exePath,string args = null)
	{
		var app = new CliApp(exePath);
			app.proc.StartInfo.Arguments = args;
			LOG.log("[CLI.APP] : quickrun() : {0} {1}", exePath, args);
			app.proc.Start();
			app.proc.WaitForExit();
		var s = new string[] {
			app.proc.StandardOutput.ReadToEnd(),
			app.proc.StandardError.ReadToEnd(),
			app.proc.ExitCode.ToString()
		};
		// app.proc.Close(); // DO NOT CLOSE!! ~CliApp() will handle the process ending
		return s;
	}// -----------------------------------------

	/// <summary>
	/// Check to see if an executable exists / can be run
	/// </summary>
	/// <param name="exePath"></param>
	/// <returns></returns>
	static public bool exists(string exePath)
	{
		var app = new CliApp(exePath);
		
		try{
			app.proc.Start();
			app.proc.WaitForExit();
		}catch(System.ComponentModel.Win32Exception)
		{
			return false;
		}

		return true;
	}// -----------------------------------------


}// -- end class
}// -- end namespace
