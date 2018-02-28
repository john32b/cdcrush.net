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
	// Percentage of number of tasks completed
	public int TASKS_COMPLETION_PERCENT {get; private set;} // # USER READ

	// The last task running, pointer
	public CTask TASK_LAST { get; private set; }	// # USER READ

	// Currently active slots for ASYNC tasks.
	bool[] slots;

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
	//  taskStart	:	A new task just started (Read job.TASK_LAST)
	//  taskEnd		:	A task just ended		(Read job.TASK_LAST)
	//	fail		:	Job has failed
	// B: The CJob itself
	public Action<CJobStatus, CJob> onJobStatus = (a, b) => { };


	// #USERSET  
	// Called whenever the status changes
	// A, status message :
	//		run		:	The task has just started
	//		progress:	The task progress has changed
	//		complete:	The task has been completed
	//		fail	:	The task has failed, see the ERROR field
	// B, the Task itself
	public Action<CTaskStatus, CTask> onTaskStatus = (a, b) => { };	// Nothing by default 

	// If the job has failed, this holds the ERROR code, copied from task ERROR code
	// [0] is Error Code
	// [1] is Error Message
	public string[] ERROR {get; private set;}

	// --
	// Create a new Job handler for Custom Tasks
	public CJob(string _name = null, object _taskData = null)
	{
		taskQueue = new List<CTask>();
		currentTasks = new List<CTask>();
		taskData = _taskData;
		TASKS_TOTAL = 0;
		name = _name ?? "Unnamed Job, " + DateTime.Now.ToString();
		timer = new Stopwatch();
		status = CJobStatus.waiting;
		ERROR = new string[2];
	}// -----------------------------------------

	// Kill all active Tasks
	~CJob()
	{

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
		// Fill in the slot array 
		slots = Enumerable.Repeat<bool>(false, MAX_CONCURRENT).ToArray();

		// SHOULD ONLY BE CALLED ONCE IN THE LIFETIME
		// TODO: SAFEGUARD FOR THAT

		TASKS_COMPLETION_PERCENT = 0;
		TASKS_COMPLETE = 0;

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
		
			// Find the next available slot[]
			var fr = Array.FindIndex(slots, s => s == false);
			slots[fr] = true;
			t.SLOT = fr; // SLOTS :: Set t.TASK to a unique number between tasks from 0->MAX_CONCURRENT
		}	// end lock

		LOG.log("[CJOB] : Task Start: {0} | Remaining: {1} ", t.name, taskQueue.Count);
		await Task.Factory.StartNew(() => t.start());
	}// -----------------------------------------


	// End a task properly
	// Task has either Completed or Failed
	private void killTask(CTask t)
	{
		slots[t.SLOT] = false;
		currentTasks.Remove(t);
		t.kill();
	}// -----------------------------------------


	//-- Internal task status handler
	private void _onTaskStatus(CTaskStatus s,CTask t)
	{
		// Pass this through
		onTaskStatus(s, t);

		switch(s)
		{
			case CTaskStatus.complete:
				LOG.log("[CJOB] : Task Completed: {0} ", t);
				taskData = t.dataSend;
				TASKS_COMPLETE++;
				TASKS_COMPLETION_PERCENT = (int)Math.Ceiling( (100.0f/TASKS_TOTAL) * TASKS_COMPLETE);
				TASK_LAST = t;
				onJobStatus(CJobStatus.taskEnd, this);
				killTask(t);
				feedQueue();
				break;

			case CTaskStatus.fail:
				// Fail the whole job
				// FUTURE, make non-important tasks where the job won't fail?
				LOG.log("[CJOB] : Error : Task Failed: {0}", t);
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
	/// Custom code, called on FAIL and COMPLETE
	/// </summary>
	virtual protected void kill()
	{
		LOG.log("[CJOB] : Killing Job :: ", this.name);
		foreach(var t1 in currentTasks) t1.kill();
		currentTasks.Clear();
		foreach(var t2 in taskQueue) t2.kill(); // They are not running but just in case
		taskQueue.Clear();
	}// -----------------------------------------


}// -- end class

}// -- end namespace
