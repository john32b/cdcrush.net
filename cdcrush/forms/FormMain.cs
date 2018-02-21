using cdcrush.lib;
using cdcrush.lib.task;
using cdcrush.prog;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace cdcrush.forms
{

	// The four main tabs of the main form
	enum TAB_ID:int {
		RESTORE=0,
		COMPRESS,
		OPTIONS,
		INFO
	};


public partial class FormMain : Form {

	// GLOBAL ACCESS
	// ----------------
	
	// Write a status message from everywhere
	internal static Action<string, int> sendMessage;
	// Send progress report from everywhere
	internal static Action<int> sendProgress;
	// Send whole form LOCK status from components
	internal static Action<bool> sendLock;

	// VARS ::
	// ---------------

	// Keep current open tab index
	int CURRENT_TAB;

	// First tab to go when loading the program
	const int STARTING_TAB = (int)TAB_ID.COMPRESS;

	// =========================================================
	// --
	public FormMain() 
	{
		InitializeComponent();
		sendMessage = form_setText;
		sendProgress = form_setProgress;
		sendLock = form_lockAll;
		CDCRUSH.jobStatusHandler = genericJobProgressReport;
	}// -----------------------------------------

	// :: Initialize things:
	private void FormMain_Load(object sender, EventArgs e)
	{
		// -- Enable drag and drop
		FormTools.dragDropFormEnable(this, (string[] files) => {
			if (CDCRUSH.LOCKED) return;
			if (files == null) return;
			handle_dropped_file(files[0]);
		});
	
		// -- Engine
		if(!CDCRUSH.init()) {
			// For some reason the engine couldn't initialize
			form_setText(CDCRUSH.ERROR, 3);
			form_lockAll(true);
			return;
		}

		// - Init Form Things 
		form_setProgress(0);
		form_setText("Ready.", 1);

		info_ver.Text = CDCRUSH.PROGRAM_VERSION;
		link_web.LinkClicked += new LinkLabelLinkClickedEventHandler((a, b) => {
				link_web.LinkVisited = true;
				System.Diagnostics.Process.Start(CDCRUSH.WEB_SITE);
		});

		// Set initial tab
		tabControl1.SelectedIndex = STARTING_TAB;
	}// -----------------------------------------



	// =============================================
	// FORM INTERACTION
	// =============================================

	// -- AutoCalled whenever a file has been dropped
	//
	public void handle_dropped_file(string file)
	{
		if(CURRENT_TAB==(int)TAB_ID.COMPRESS)
		{
			panelCompress1.handle_dropped_file(file);
		}
		else if(CURRENT_TAB==(int)TAB_ID.RESTORE)
		{
			panelRestore1.handle_dropped_file(file);
		}
	}// -----------------------------------------

	/// <summary>
	/// Locks/Unlocks the form
	/// Can be called from threads
	/// </summary>
	/// <param name="_lock"></param>
	public void form_lockAll(bool _lock)
	{
		FormTools.invoke(this, () =>
		{
			tabControl1.Enabled = !_lock;
		});
	}// -----------------------------------------


	/// <summary>
	/// Can be called from threads
	/// Updates status message
	/// </summary>
	/// <param name="msg">Message</param>
	/// <param name="type">0:no change, 1:normal, 2:green, 3:red</param>
	public void form_setText(string msg="", int type=0)
	{
		FormTools.invoke(this, () =>
		{
			info_status.Text = msg;

			if (type > 0)
			{
				info_status.ForeColor = FORM_STATUS_COLORS[type - 1];
			}
		});
	}// -----------------------------------------
	Color[] FORM_STATUS_COLORS = new[] {SystemColors.GrayText,Color.Green,Color.Red};


	
	/// <summary>
	/// Updates progress bar
	/// Can be called from threads
	/// </summary>
	/// <param name="per">Progress Percent</param>
	public void form_setProgress(int per)
	{
		FormTools.invoke(this, () =>
		{
			if (per < 0)
			{
				progressBar1.Style = ProgressBarStyle.Marquee;
				progressBar1.MarqueeAnimationSpeed = 15;
				return;
			}
			else
				if (per == 0)
				{
					progressBar1.Style = ProgressBarStyle.Blocks;
				}

				else if (per > 100) per = 100;

			progressBar1.Value = per;

		});
	}// -----------------------------------------


	// ============================================================
	// == EVENTS
	// ============================================================

	
	/// <summary>
	/// Generic progress report of any CJob type object
	/// Changes progress bar and sets simple status messages
	/// </summary>
	/// <param name="s">JobStatus</param>
	/// <param name="j">Job</param>
	private void genericJobProgressReport(CJobStatus s,CJob j)
	{
		switch(s)
		{
			case CJobStatus.taskStart:

				if(j.TASK_LAST.PROGRESS_UNKNOWN) {
					form_setProgress(-1);
					// LOG.log("PROGRESS BAR WORKING-----------");
				}

				if (string.IsNullOrEmpty(j.TASK_LAST.desc))
					form_setText(j.TASK_LAST.name, 0);
				else
					form_setText(j.TASK_LAST.desc, 0);
				break;

			case CJobStatus.taskEnd:

				if(j.TASK_LAST.PROGRESS_UNKNOWN) {
					form_setProgress(0); // Restore the progress bar to normal 
				}else{
					form_setProgress(j.TASKS_COMPLETION_PERCENT);
				}

				break;

			case CJobStatus.complete:
				form_setText(j.name + " complete ", 2);
				break;

			case CJobStatus.fail:
				form_setText(j.name + " failed ", 3);
				break;
		}
	}// -----------------------------------------

	// --
	// Changed Tab Index
	private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
	{
		CURRENT_TAB = tabControl1.SelectedIndex;
	}// -----------------------------------------

	// -
	// Set/Unset Default temp folder
	private void check_tempDef_CheckedChanged(object sender, EventArgs e)
	{
		// TODO

		if(check_tempDef.Checked) {
			
		} else {
			
		}
	}// -----------------------------------------

	// --
	private void btn_selectTemp_Click(object sender, EventArgs e)
	{
		// TODO
	}// -----------------------------------------

}// end class
}// --
