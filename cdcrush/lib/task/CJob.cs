using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace cdcrush.lib.task
{


// Some used on callback messages, some for internal step keep
public enum CJobStatus
{
	waiting,	// Job hasn't started yet
	complete,	// Job is complete
	start,		// Job has just started
	fail,		// Job has failed
	progress,	// Job progress has been updated
	taskStart,	// A New task has started
	taskEnd		// A task has ended
};


/// <summary>
/// 
/// ## Custom Job
/// 
/// Manager for a series of custom tasks, handles the execution of tasks and 
/// reports progress back to the user
/// 
/// <example>
///		var job = new CJob("Restore");
///		
///			job.addAsync(new CTask((t) => {
///				// Do something;
///				t.complete();
///			}));
///			
/// </example>
/// 
/// </summary>
public class CJob
{
	// General Use Job name
	public string name {get; set;}
	// General Use String ID
	public string sid {get; set;}

	// #USERSET
	// How many parallel tasks to run at a time
	// Set this right after creating the object
	public int MAX_CONCURRENT = 2;

	// #USER SET
	// Custom user set data. Can be shared between tasks
	// Useful for storing custom job parameters and such.
	public dynamic jobData {get; set;}

	// Internal pointer to keep track of the data in and out of tasks
	object taskData;

	// Total number of tasks in this job
	public int TASKS_TOTAL { get; private set; }	// # USER READ
	// Number of completed tasks
	public int TASKS_COMPLETE { get; private set; } // # USER READ
	// Number of tasks currently running
	public int TASKS_RUNNING {get; private set;} // # USER READ

	// The last task, pointer, usen on callbacks
	public CTask TASK_LAST { get; private set; }	// # USER READ

	// Currently active slots for ASYNC tasks.
	bool[] slots_active;

	// Holds all the tasks that are waiting to be executed
	List<CTask> taskQueue;

	// Pointers to the current working tasks
	public List<CTask> currentTasks { get; private set;}

	// Keep track of the job completion time
	public Stopwatch timer {get; private set;}

	// Current Job Status
	public CJobStatus status {get; private set;}

	// #USERSET #OPTIONAL
	// Same call as onJobStatus(complete)
	// Called whenever the Job Completes or Fails
	// True : success, False : Error (read the ERROR field)
	// NOTE: Be careful if you set BOTH this and onJobStatus they will both get called
	public Action<bool> onComplete = null;

	// #USERSET
	// Sends JOB Related status updates
	// A: Job Status
	//	waiting		:	Job hasn't started yet
	//	complete	:	Job is complete
	//	start		:	Job has just started
	//  progress	:	Job progress % updates  (Read job.PROGRESS)
	//  taskStart	:	A new task just started (Read job.TASK_LAST)
	//  taskEnd		:	A task just ended		(Read job.TASK_LAST)
	//	fail		:	Job has failed
	// B: The CJob itself
	public Action<CJobStatus, CJob> onJobStatus = (a, b) => { };


	// #USERSET  
	// Called whenever the status changes
	// A, status message :
	//		start	:	The task has just started
	//		progress:	The task progress has changed
	//		complete:	The task has been completed
	//		fail	:	The task has failed, see the ERROR field
	// B, the Task itself
	public Action<CTaskStatus, CTask> onTaskStatus = (a, b) => { };	// Nothing by default 

	// If the job has failed, this holds the ERROR code, copied from task ERROR code
	// [0] is Error Code
	// [1] is Error Message
	public string[] ERROR {get; private set;}

	// Keep track of whether the job is done and properly shutdown
	private bool IS_KILLED = false;


	// : NEW :
	
	// How much a task will contribute to the job progress %
	// Can be #USERSET, if you want to hack it. Else it is autocalculated
	float TASK_PROGRESS_RATIO;

	// Store the progress of the currently ongoing tasks, (using slot index)
	// -1 to indicate no progress, 0-100 for standard progress
	int[] slots_progress;

	// Progress % of past completed tasks. ! NOT TOTAL PROGRESS !
	float TASKS_COMPLETED_PROGRESS;

	// This is the REAL progress %
	// Percentage of tasks completed
	public float PROGRESS_TOTAL {get; private set;}

	// ====================================================


	// --
	// Create a new Job handler for Custom Tasks
	public CJob(string _name = null, object _taskData = null)
	{
		taskQueue = new List<CTask>();
		currentTasks = new List<CTask>();
		taskData = _taskData;
		name = _name ?? "Unnamed Job, " + DateTime.Now.ToString();
		timer = new Stopwatch();
		status = CJobStatus.waiting;
		ERROR = new string[2];
		TASKS_RUNNING = 0; TASKS_COMPLETE = 0; TASKS_TOTAL = 0;
		TASKS_COMPLETED_PROGRESS = 0; PROGRESS_TOTAL = 0;
		TASK_PROGRESS_RATIO = 0;
	}// -----------------------------------------

	// --
	// Destructor
	~CJob()
	{
		kill();
	}// -----------------------------------------

	// --
	// Incase a job adds tasks while running. Set this to get proper progress output
	public void hack_setExpectedProgTracks(int num)
	{
		TASK_PROGRESS_RATIO = 1.0f/num;
	}// -----------------------------------------

	// Adds a task in the queue that will be executed ASYNC
	public CJob addAsync(CTask t)
	{
		t.async = true; add(t);
		return this;
	}// -----------------------------------------
	// Add a task
	public CJob add(CTask t)
	{
		taskQueue.Add(t);
		TASKS_TOTAL++;
		return this;
	}// -----------------------------------------
	// Add a task to the top of the queue
	public CJob addNext(CTask t)
	{
		taskQueue.Insert(0, t);
		TASKS_TOTAL++;
		return this;
	}// -----------------------------------------
	// Add a task to the top of the queue
	public CJob addNextAsync(CTask t)
	{
		t.async = true; addNext(t);
		return this;
	}// -----------------------------------------

	// Starts the JOB
	// THIS IS ASYNC and will return execution to the caller right away'
	// Use the onStatus and onComplete callbacks to get updates
	public void start()
	{
		// --
		if(status != CJobStatus.waiting) {
			throw new Exception("A CJob object can only run once");
		}

		// If not set by user, autoset it
		if(TASK_PROGRESS_RATIO==0)
		{
			// num of tasks that report progress
			int tp=taskQueue.Where(t=>t.FLAG_PROGRESS_DISABLE==false).Count();
			TASK_PROGRESS_RATIO = 1.0f/tp;
		}

		// Fill in the slot array 
		slots_active = Enumerable.Repeat(false, MAX_CONCURRENT).ToArray();
		slots_progress = Enumerable.Repeat(-1,MAX_CONCURRENT).ToArray();
		
		timer.Start();
		status = CJobStatus.start;
		onJobStatus(CJobStatus.start, this);
		feedQueue();
	}// -----------------------------------------


	// Scans the task queue and executes them in order
	// also executes multiple at once if they are async.
	private void feedQueue()
	{
		lock(taskQueue) { // Since feedQueue can be executed ASYNC by any task that is ending //

		if(taskQueue.Count>0)
		{
			var t = taskQueue.First(); // Peek the first element

			if(currentTasks.Count<MAX_CONCURRENT)
			{
				// if previous was SYNC, and there are 0 running tasks, so ok to run
				// if previous was ASYNC, then this can run in parallel
				if(t.async){
					startNextTask().ConfigureAwait(false);
					feedQueue();
				}else{
					if(currentTasks.Count==0){
						startNextTask().ConfigureAwait(false);
					}// else there is a task still running and it will call this again when it ends
				}
			}// else the buffer is full

		}else{

			// Make sure there are no async tasks waiting to be completed
			if(TASKS_COMPLETE == TASKS_TOTAL)
			{
				// Job Complete
				timer.Stop();
				kill();
				status = CJobStatus.complete;
				onJobStatus(CJobStatus.complete, this);
				onComplete?.Invoke(true);
			}
		}
		
		}// end lock
	}// -----------------------------------------


	// Force - Starts the next task on the queue
	// PRE : taskQueue.Count > 0 ; Checked earlier
	async private Task startNextTask()
	{
		CTask t;

		lock (taskQueue)
		{
			t = taskQueue.First();
			t.parent = this;
			t.dataGet = taskData;
			t.onStatus = _onTaskStatus;
			taskQueue.RemoveAt(0);
			currentTasks.Add(t);
		
			// Find the next available slot[] index
			var fr = Array.FindIndex(slots_active, s => s == false);
			slots_active[fr] = true;
			t.SLOT = fr; // SLOTS :: Set t.TASK to a unique number between tasks from 0->MAX_CONCURRENT
		}	// end lock

		TASKS_RUNNING ++;
		LOG.log("[CJOB]: Task Start | {0} | Remaining:{1} | Running:{2}", t, taskQueue.Count, currentTasks.Count);
		await Task.Factory.StartNew(() => t.start());
	}// -----------------------------------------


	// End a task properly
	// Task has either Completed or Failed
	private void killTask(CTask t)
	{
		TASKS_RUNNING --;
		slots_active[t.SLOT] = false;
		slots_progress[t.SLOT] = -1;
		if(!t.FLAG_PROGRESS_DISABLE) TASKS_COMPLETED_PROGRESS += TASK_PROGRESS_RATIO * 100;
		currentTasks.Remove(t);
		t.kill();
	}// -----------------------------------------

	// --
	// Calculate the Total Progress
	private void calculateProgress()
	{
		PROGRESS_TOTAL = TASKS_COMPLETED_PROGRESS;
		for(int i=0;i<MAX_CONCURRENT;i++) {
			if(slots_active[i]) PROGRESS_TOTAL += slots_progress[i] * TASK_PROGRESS_RATIO;
		}
	}// -----------------------------------------

	//-- Internal task status handler
	private void _onTaskStatus(CTaskStatus s,CTask t)
	{
		// Pass this through
		onTaskStatus(s, t);

		switch(s)
		{
			case CTaskStatus.complete:
				LOG.log("[CJOB]: Task Completed: {0} ", t);
				taskData = t.dataSend;
				TASKS_COMPLETE++;
				TASK_LAST = t;
				onJobStatus(CJobStatus.taskEnd, this);
				killTask(t);
				feedQueue();
				break;

			// TODO: I could report the progress on a timer
			//		 This is not ideal if there are many tasks running at once (CPU wise)
			// NOTE: Will not get called from FLAG_NO_PROGRESS tasks
			case CTaskStatus.progress:
				slots_progress[t.SLOT] = t.PROGRESS;
				// LOG.log("Task:{0}, slot:{2} progress:{1}",t.name,t.PROGRESS,t.SLOT);
				calculateProgress();
				onJobStatus(CJobStatus.progress,this);
				break;

			case CTaskStatus.fail:
				// Fail the whole job
				LOG.log("[CJOB]: Error : Task Failed: {0}", t);
				killTask(t);
				fail(t.ERROR[0], t.ERROR[1]);
				break;

			case CTaskStatus.start:
				TASK_LAST = t;
				onJobStatus(CJobStatus.taskStart, this);
				break;
		}

	}// -----------------------------------------

	/// <summary>
	/// Force fail the JOB and cancel all remaining tasks, or this is called when a Task Fails
	/// </summary>
	/// <param name="code"></param>
	/// <param name="msg"></param>
	protected void fail(string code="", string msg="")
	{
		ERROR[0] = code;
		ERROR[1] = msg;

		kill();

		status = CJobStatus.fail;
		onJobStatus(CJobStatus.fail, this);
		onComplete?.Invoke(false);
	}// -----------------------------------------


	/// <summary>
	/// Cleanup code, called on FAIL and COMPLETE
	/// </summary>
	virtual protected void kill()
	{
		if(IS_KILLED) return; IS_KILLED = true;

		LOG.log("[CJOB]: Killing Job :: ", name);

		// Clear any running task
		foreach(var t1 in currentTasks) t1.kill();
		currentTasks.Clear();

		// Clear any waiting task (just in case)
		foreach(var t2 in taskQueue) t2.kill();
		taskQueue.Clear();
	}// -----------------------------------------


}// -- end class

}// -- end namespace
