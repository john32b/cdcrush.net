using System;

namespace cdcrush.lib.task
{

// Some used on callback messages, some for internal step keep
public enum CTaskStatus
{
	waiting, complete, start, fail, progress
};


/// <summary>
/// A simple custom Task that runs and reports progress.
/// Works somewhat like a Task wrapper.
/// Usually lives inside a "CJob" object, which manages a series of CTasks execution
/// </summary>
public class CTask
{
	// Global UID generator for the tasks
	static int UID = 0;
	// ---------------------------

	// Task Unique ID
	public int uid {get; private set;}

	// Task Name
	public string name {get; set;}

	// Task Description
	public string desc {get; set;}

	// Current progress from 0 to 100
	int progress_percent = 0;

	// Set this to autopush updates
	public int PROGRESS {
		get { return progress_percent; }
		set{
			progress_percent = value;
			if(progress_percent>100) progress_percent = 100;
			if(!FLAG_PROGRESS_DISABLE) onStatus(CTaskStatus.progress, this);
		}
	}//--

	// If true will not report progress or count towards job progress
	// Useful for small tasks that start and end instantaneously
	public bool FLAG_PROGRESS_DISABLE = false;

	// Custom flag, handled by user
	public bool FLAG_PROGRESS_UKNOWN = false;

	// Current lifecycle step
	public CTaskStatus status;

	// Used if a JOB runs multiple tasks at once, holds an imaginary "slot" inside the maximum concurrent limit
	// - useful to keep track of progress of individual tasks, etc.
	// - CJob sets this
	public int SLOT {get; internal set;}

	// #USERSET Called when this task is completed
	public Action onComplete = null;

	// #USERSET  Called whenever the status changes
	// Available statuses ==
	//		start	:	The task has just started
	//		progress:	The task progress has changed
	//		complete:	The task has been completed
	//		fail	:	The task has failed, read the ERROR field
	public Action<CTaskStatus, CTask> onStatus = (a, b) => { };	// Nothing by default 

	// # USER SET
	// if you want to declare a kill function on quick tasks, here is where you do it
	public Action killExtra = null;

	// Pointer to the Job holding this task
	internal CJob parent;

	// CJob sets this
	// SYNC tasks run by themselves while no other task is running on the Job
	// ASYNC tasks can run in parallel with other ASYNC tasks on the Job
	internal bool async = false;

	// -- Data
	// Pointer to JOB's shared data object
	public dynamic jobData { 
		get { return parent.jobData; }
	}

	// Data read from the previous task
	internal object dataGet;

	// Data to send to the next task
	internal object dataSend = null;

	// Data that is unique to the task, perhaps you might need this sometimes
	public object custom = null;

	// If set, then this code will run at task start
	private Action<CTask> quick_run;

	// If the task has failed, this holds the ERROR code
	// [0] is Error Code. Custom, can be short IDs like "file", "net"
	// [1] is Error Message
	public string[] ERROR {get; private set;}

	/// <summary>
	/// Create a Task for use in a CJob manager
	/// </summary>
	/// <param name="Qrun">If you want to create a quick task without extending it</param>
	/// <param name="Name">Optional task name, used for logging</param>
	public CTask(Action<CTask> Qrun=null, string Name=null)
	{
		uid = ++UID;
		name = Name ?? string.Format("task_{0}", uid);
		desc = "";
		quick_run = Qrun;
		status = CTaskStatus.waiting;
		
		if(name.StartsWith("-"))	// NO PROGRESS
		{
			FLAG_PROGRESS_DISABLE = true;
			name = name.Substring(1);
		}

	}// -----------------------------------------

	// # USER EXTEND
	// Or you can just use a quick_run
	public virtual void start()
	{
		status = CTaskStatus.start;
		onStatus(CTaskStatus.start, this);
		PROGRESS = 0; // Make sure you call this after pushing the "start" event
		quick_run?.Invoke(this);
	}// -----------------------------------------

	// Call this from the extended class or a quickrun to indicate task completion
	public void complete()
	{
		PROGRESS = 100;
		status = CTaskStatus.complete;
		onStatus(CTaskStatus.complete, this);
		onComplete?.Invoke();
	}// -----------------------------------------

	// The task has failed, can be called from a quick run also
	// ErrorCode, ErrorMessage
	public void fail(string code="", string msg="")
	{
		ERROR = new string[2] {code, msg};
		onStatus(CTaskStatus.fail, this);
	}// -----------------------------------------

	// Called from a job, use this to free resources etc.
	// Called on Completion / Fail / Forced Exit
	public virtual void kill()
	{
		// # OVERRIDE THIS , don't forget to call base.kill()
		killExtra?.Invoke();
	}// -----------------------------------------

	/// <summary>
	/// Quickly log something
	/// </summary>
	/// <param name="str"></param>
	public void log(string str)
	{
		LOG.log("[CTASK] ({0}): {1}", name, str);
	}// -----------------------------------------

	// --
	public override string ToString()
	{
		return string.Format("uid:{2}, name:{0}, desc:{1}", name, desc, uid);
	}// -----------------------------------------
	
	/// <summary>
	/// Quickly handle a CLI app completion
	/// </summary>
	/// <param name="cli"></param>
	public void handleCliReport(app.ICliReport cli)
	{
		cli.onComplete = (s) =>
		{
			if(s){
				complete();
			}else{
				fail(cli.ERROR);
			}
		};
	}// -----------------------------------------
}// --

}// -- 
