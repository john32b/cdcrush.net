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
	// The main process
	public Process proc {get; private set;}

	// Be able to change the exec pathf
	public string executable { 
		get { return proc.StartInfo.FileName; }
		set { proc.StartInfo.FileName = value; } 
	}
	
	// --
	// Called when process ends along with exit code, { Usually 0 = OK, 1 = Error.. 
	public Action<int> onComplete;  // #Userset
	public Action<string> onStdOut;	// #Userset
	public Action<string> onStdErr; // #Userset

	StringBuilder builderStdOut;
	StringBuilder builderStdErr;

	public string stdOutLog { get{ return builderStdOut.ToString(); } }
	public string stdErrLog { get{ return builderStdErr.ToString(); } }

	// Helpers
	private bool _hasStarted = false;

	// -----------------------------------------
	/// <summary>
	/// Starts a CLI app in a new thread
	/// </summary>
	/// <param name="exec">Path to the executable</param>
	/// <param name="sync">Run this in sync</param>
	public CliApp(string exec)
	{
		proc = new Process();
		//Debug.WriteLine("PROCESS {0} HAS ID {1}", exec, proc.Id);
		executable = exec; // setter
		proc.StartInfo.UseShellExecute = false; // needed for executables!
		proc.StartInfo.CreateNoWindow = true;
		proc.StartInfo.RedirectStandardOutput = true;
		proc.StartInfo.RedirectStandardError = true;

		//proc.EnableRaisingEvents = true; // { Needed for .Exited Event }
		//proc.Exited += proc_Exited;

		builderStdOut = new StringBuilder();
		builderStdErr = new StringBuilder();

		System.Text.StringBuilder g = new System.Text.StringBuilder();

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

	}// -----------------------------------------

	// --
	~CliApp()
	{
		if(_hasStarted && !proc.HasExited) proc.Kill();
		proc.Dispose(); 
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

		var th = new Thread(new ThreadStart(
			 () => {
				 proc.Start(); _hasStarted = true;
				 proc.BeginErrorReadLine();
				 proc.BeginOutputReadLine();
				 proc.WaitForExit();
				 LOG.log("[CLI.APP] : Process \'{0}\' Exited with code {1}", executable, proc.ExitCode);
				 if (onComplete != null) onComplete(proc.ExitCode);
			 }));
		th.IsBackground = true;
		th.Name = "cliApp(" + executable + ")";
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
	 * Quickly create a CLI APP in SYNC and return it's stdOut and stdErr
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
