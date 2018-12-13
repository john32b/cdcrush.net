using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace cdcrush.lib.app
{

/**
 * Generic CLI application spawner
 * Starts an app in a new thread asynchronously 
 * --
 * Example:
 *	 
 *	 proc = new CliApp('application.exe');
 *	 proc.onComplete = (code) => { 
 *	  // code 0 = OK, 1 = ERROR, 2 = CAN'T RUN EXE
 *	 };
 *	 proc.onStdOut = (s) => { LOG.log(s); }
 *	 proc.start("-i -a","c:\\temp");  // start(arguments, workingDir);
 *	 
 *	 //--
 *	 // If you want to use piping to/from the app
 *	 // before .start() set:
 *	 proc.flag_disable_user_stdout = true;	 // (this will disable user callbacks for stdout)
 *	 proc.start();
 *	 proc.
 *	
 *	 
 */
public class CliApp
{
	public static List<Thread> threads = null;

	// The main process object
	public Process proc {get; private set;}

	// Path to the executable, set directly to the process object
	public string executable { 
		get { return proc.StartInfo.FileName; }
		set { proc.StartInfo.FileName = value; } 
	}
	
	/// <summary>
	/// Called when the application exits. 
	///		 0 = OK, 1 = Error, 2 = Could not run EXE
	/// </summary>
	public Action<int> onComplete; 
	/// <summary>
	/// Pushes the STDOUT as is happens from the app
	/// </summary>
	public Action<string> onStdOut;
	/// <summary>
	/// Pushes the STDERR as it happens from the app
	/// </summary>
	public Action<string> onStdErr;

	/// <summary>
	/// Valid only if you set `flag_stdout_word_mode` to true
	/// Pushes words read from the STDOUT
	/// </summary>
	public Action<string> onStdOutWord;	

	// Internal stirng holders for stderr and stdout
	StringBuilder builderStdOut;
	StringBuilder builderStdErr;

	/// <summary>
	/// Logs the entire App STDOUT here
	/// DOES NOT WORK when `flag_stdout_word_mode` is set
	/// </summary>
	public string stdOutLog { get{ return builderStdOut.ToString(); } }

	/// <summary>
	/// Logs the entire App STDERR here
	/// </summary>
	public string stdErrLog { get{ return builderStdErr.ToString(); } }


	/// <summary>
	/// Access the main stdIn Stream. 
	/// </summary>
	public System.IO.Stream stdIn { get{return proc.StandardInput.BaseStream;}}
	/// <summary>
	/// Access the main stdOut Stream
	/// </summary>
	public System.IO.Stream stdOut { get{return proc.StandardOutput.BaseStream;}}


	/// <summary>
	/// When some CLI apps don't flush their stdOut, capture it raw word by word. 
	/// Access `onStdOutWord`
	/// </summary>
	public bool flag_stdout_word_mode = false;

	/// <summary>
	/// Set to true to disable ALL user stdout calls. Enable this if you want to pipe the stdout
	/// </summary>
	public bool flag_disable_user_stdout = false;

	// Helper
	bool _hasStarted = false;


	public Action onStdIOReady;

	// -----------------------------------------
	/// <summary>
	/// Starts a CLI app in a new thread
	/// </summary>
	/// <param name="exec">Path to the executable</param>
	public CliApp(string exec)
	{
		proc = new Process();
		executable = exec; // uses setter

		proc.StartInfo.UseShellExecute = false; // needed for executables!
		proc.StartInfo.CreateNoWindow = true;	

		proc.StartInfo.RedirectStandardInput = true;
		proc.StartInfo.RedirectStandardOutput = true;
		proc.StartInfo.RedirectStandardError = true;

		builderStdOut = new StringBuilder();
		builderStdErr = new StringBuilder();

	}// -----------------------------------------

	~CliApp()
	{
		kill();
	}// -----------------------------------------	

	public void kill()
	{
		if(_hasStarted && !proc.HasExited) proc.Kill();
	}// -----------------------------------------


	/// <summary>
	/// Start a thread in a new thread, it will keep going
	/// </summary>
	/// <param name="args">Arguments</param>
	public void start(string args = null,string workingDir = null)
	{
		proc.StartInfo.WorkingDirectory = workingDir;
		proc.StartInfo.Arguments = args;
		
		proc.ErrorDataReceived += (sender, e) =>
		{
			builderStdErr.Append(e.Data);
			if(!String.IsNullOrEmpty(e.Data) && onStdErr!=null)
				onStdErr(e.Data);
		};

		if(!flag_disable_user_stdout)
		{
			proc.OutputDataReceived += (sender, e) =>
			{
				builderStdOut.Append(e.Data);
				if(!String.IsNullOrEmpty(e.Data) && onStdOut!=null)
					onStdOut(e.Data);
			};
		}

		// -- 
		LOG.log("[CLI] > {0} {1}",executable,args);

		// Note: I really need a Thread here, If I don't, it will lock the main thread and forms
		Thread th = new Thread(new ThreadStart(
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

				proc.BeginErrorReadLine();

				if(flag_stdout_word_mode && onStdOutWord!=null)
				{
					captureWords();
				}else
				{
					if(!flag_disable_user_stdout)
					{
						proc.BeginOutputReadLine();
					}
				}

				onStdIOReady?.Invoke();

				proc.WaitForExit();

				onComplete?.Invoke(proc.ExitCode);

		}));

		// - Start the thread
		th.IsBackground = true;
		th.Name = $"cliApp({executable})";
		th.Start();
	}// -----------------------------------------

	/// <summary>
	/// Start capturing words from the stdout stream and push them on the callback
	/// </summary>
	private void captureWords()
	{
		int byte_r  = 0;
		StringBuilder word = new StringBuilder();

		while((byte_r = proc.StandardOutput.BaseStream.ReadByte()) > -1) 
		{
			builderStdOut.Append(Char.ConvertFromUtf32(byte_r));

			if(byte_r == 32 || byte_r == 13) // SPACE or ENTER
			{
				if(word.Length > 0) {
					onStdOutWord(word.ToString());
					word.Clear();
				}

			} else {
				if(byte_r > 32) // No special characters in the stringbuilder
				{
					word.Append(Char.ConvertFromUtf32(byte_r));
				}
			}
		}
	}// -----------------------------------------

	/// <summary>
	/// Quickly create a CLI APP and return it
	/// </summary>
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