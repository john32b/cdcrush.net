using cdcrush.lib;
using cdcrush.lib.task;
using cdcrush.prog;
using Microsoft.WindowsAPICodePack.Taskbar;
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


/**
 * Main form of cdcrush program
 */
public partial class FormMain : Form 
{
	// GLOBAL ACCESS
	// ----------------
	
	// Write a status message from everywhere
	internal static Action<string, int> sendMessage;
	// Send progress report from everywhere
	internal static Action<int> sendProgress;
	// Send whole form LOCK status from components
	internal static Action<bool> sendLock;
	// Request to open CD INFO popup
	internal static Action<cd.CDInfos> showCdInfo;

	// VARS ::
	// ---------------

	// Keep current open tab index
	int CURRENT_TAB;

	// First tab to go when loading the program
	const int STARTING_TAB = (int)TAB_ID.RESTORE;

	// If true, will clear the status at the next tab change. accessible from everywhere
	public static bool FLAG_REQUEST_STATUS_CLEAR = false;

	// Keep the window title
	string windowTitle;

	// =========================================================
	// --
	public FormMain() 
	{
		InitializeComponent();
		sendMessage = form_setText;
		sendProgress = form_setProgress;
		sendLock = form_lockAll;
		showCdInfo = form_showCdInfo;
		CDCRUSH.jobStatusHandler = genericJobProgressReport;
	}// -----------------------------------------

	// --
	private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
	{
		// --To cancel. e.cancel = true

		if(CDCRUSH.LOCKED)
		{
			if(!FormTools.PopupYesNo("An operation is in progress, really quit?","Confirm"))
			{
				e.Cancel = true;
			}
		}
			
		Properties.Settings.Default.Save();
		CDCRUSH.kill();
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
	
		// - Init Form Things 
		FormTools.fileLoadDialogPrepare("ffmpeg", "FFmpeg.exe|ffmpeg.exe");
		form_setProgress(0);
		form_setText("Ready.", 1);

		// - Set Infos tab
		windowTitle = CDCRUSH.PROGRAM_NAME + " v" + CDCRUSH.PROGRAM_VERSION;
		this.Text = windowTitle;

		// - Links
		info_ver.Text = CDCRUSH.PROGRAM_VERSION;
		link_web.LinkClicked += new LinkLabelLinkClickedEventHandler((a, b) => {
			link_web.LinkVisited = true;
			System.Diagnostics.Process.Start(CDCRUSH.LINK_SOURCE);
		});

		link_donate.LinkClicked += new LinkLabelLinkClickedEventHandler( (a,b) => {
			link_donate.LinkVisited = true;
			System.Diagnostics.Process.Start(CDCRUSH.LINK_DONATE);
		});

		// -- Init Engine
		if(!CDCRUSH.init()) {
			// For some reason the engine couldn't initialize
			form_setText(CDCRUSH.ERROR, 3);
			form_lockAll(true);
			return;
		}

		// -- FFmpeg
		form_updateFFmpegStatus();

		// -- Settings tab
		loadAndSetupSettings();

		// -- Set initial tab
		tabControl1.SelectedIndex = STARTING_TAB;

		// --
		#if (!DEBUG)
			group_debug.Visible = false;
		#endif

	}// -----------------------------------------


	// --
	// Load saved settings and apply them to the engine
	// Reflect the changes to the form as well
	void loadAndSetupSettings()
	{
		LOG.log("[FormMain] Loading Settings : ");

		// -- TEMP_PATH
		if(!string.IsNullOrWhiteSpace(Properties.Settings.Default.tempFolder))
		{
			LOG.log("\t tempfolder : " + Properties.Settings.Default.tempFolder);
			if(!CDCRUSH.setTempFolder(Properties.Settings.Default.tempFolder))
			{
				// Stored temp folder is no longer valid for some reason :
				Properties.Settings.Default.tempFolder = null;
			}
		}

		// Temp Folder Form Elements :
		info_tempFolder.Text = CDCRUSH.TEMP_FOLDER;

		// - FFMPEG_PATH
		if(!string.IsNullOrWhiteSpace(Properties.Settings.Default.ffmpegPath))
		{
			LOG.log("\t ffmpeg : " + Properties.Settings.Default.ffmpegPath);
			form_setFFmpegPath(Properties.Settings.Default.ffmpegPath);
		}

		// - MAX_TASKS
		if(Properties.Settings.Default.maxTasks>0)
		{
			LOG.log("\t maxtasks : " + Properties.Settings.Default.maxTasks);
			CDCRUSH.MAX_TASKS = Properties.Settings.Default.maxTasks;
		}	

		num_threads.Value = CDCRUSH.MAX_TASKS;

	}// -----------------------------------------

	// =============================================
	// FORM INTERACTION
	// =============================================


	// --
	// Show a popup with some CD infos like checksums
	public void form_showCdInfo(cd.CDInfos cd)
	{
		if(FormChecksums.ISOPEN) return;
		var f = new FormChecksums(cd);
		f.Show();
	}// -----------------------------------------


	// -- AutoCalled whenever a file has been dropped
	//
	public void handle_dropped_file(string file)
	{
		FLAG_REQUEST_STATUS_CLEAR = true; // just in case

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
	/// <param name="type">Color = 0:no change, 1:normal, 2:green, 3:red</param>
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
				TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);
				return;
			}
			else
				if (per == 0)
				{
					progressBar1.Style = ProgressBarStyle.Blocks;
					progressBar1.Value = per;
					this.Text = windowTitle;
					TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
					TaskbarManager.Instance.SetProgressValue(0,100);
				}else
				{
					if (per > 100) per = 100;
					progressBar1.Value = per;
					this.Text =  $"[{per}%] " + windowTitle;
					TaskbarManager.Instance.SetProgressValue(per,100);
				}
		});

	}// -----------------------------------------


	// ============================================================
	// == EVENTS
	// ============================================================

	private string lastTaskName="";
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
			case CJobStatus.start:
				lastTaskName = null;
				break;

			case CJobStatus.taskStart:

				if(j.TASK_LAST.FLAG_PROGRESS_DISABLE) {
					lastTaskName = null;
					return;
				}

				if(!string.IsNullOrEmpty(j.TASK_LAST.desc)) {
					lastTaskName = j.TASK_LAST.desc;
				}else{
					lastTaskName = j.TASK_LAST.name;
				}

				form_setText(lastTaskName, 0);
				break;

			case CJobStatus.progress:
				form_setProgress((int)Math.Ceiling(j.PROGRESS_TOTAL));
				form_setText($"[{j.PROGRESS_TOTAL.ToString("0.#")}%] {lastTaskName}", 0);
				break;

			case CJobStatus.complete:
				form_setText(j.name + " Complete ", 2);
				LOG.line(40);
				LOG.log(""); // newline
				form_setProgress(100);
				FormTools.invoke(this,()=>{
					TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
					FormTools.FlashWindow(this.Handle);
				});
				break;

			case CJobStatus.fail:
				form_setText(j.name + " Failed ", 3);
				break;
		}
	}// -----------------------------------------

	// --
	// Changed Tab Index
	private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
	{
		CURRENT_TAB = tabControl1.SelectedIndex;

		// Reset status message if leftover message from an operation
		if(FLAG_REQUEST_STATUS_CLEAR)
		{
			form_setProgress(0);
			TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
			form_setText("Ready", 1);
			FLAG_REQUEST_STATUS_CLEAR = false;
		}

	}// -----------------------------------------


	// ------------------------------------------
	// SETTINGS PANEL
	// ------------------------------------------

	// --
	private void btn_temp_def_Click(object sender, EventArgs e)
	{
		if(CDCRUSH.setTempFolder())
		{
			info_tempFolder.Text = CDCRUSH.TEMP_FOLDER;
			Properties.Settings.Default.tempFolder = null;
		}
	}// -----------------------------------------
	// --
	private void btn_selectTemp_Click(object sender, EventArgs e)
	{
		SaveFileDialog d = new SaveFileDialog();
		d.FileName = "TEMP_FOLDER_THIS_ONE";
		d.CheckPathExists = true;
		d.Title = "Select Temp folder";

		if (d.ShowDialog() == DialogResult.OK)
		{
			string path;
			try{
				path = System.IO.Path.GetDirectoryName(d.FileName);
			}catch(ArgumentException)
			{
				return;
			}

			if(CDCRUSH.setTempFolder(path))
			{
				info_tempFolder.Text = CDCRUSH.TEMP_FOLDER;
				Properties.Settings.Default.tempFolder = path; // A valid folder
			}
		}
	}// -----------------------------------------

	// --
	private void num_threads_ValueChanged(object sender, EventArgs e)
	{
		CDCRUSH.MAX_TASKS = (int) num_threads.Value;
		Properties.Settings.Default.maxTasks =  (int) num_threads.Value;
	}// -----------------------------------------

	// --
	private void btn_ffmpeg_Click(object sender, EventArgs e)
	{
		var files = FormTools.fileLoadDialog("ffmpeg");
		if(files!=null)
		{
			form_setFFmpegPath(System.IO.Path.GetDirectoryName(files[0]));
		}
	}// -----------------------------------------

	// --
	// Path must be the folder containing `ffmpeg.exe`
	void form_setFFmpegPath(string path=null)
	{
		CDCRUSH.setFFMPEGPath(path);
		info_ffmpeg.Text = CDCRUSH.FFMPEG_PATH;
		Properties.Settings.Default.ffmpegPath = CDCRUSH.FFMPEG_PATH;
		form_updateFFmpegStatus();
	}// -----------------------------------------

	// --
	// Read Engine FFmpeg status and reflect on the form
	void form_updateFFmpegStatus()
	{
		if(CDCRUSH.FFMPEG_OK)
		{
			info_ffmpeg_status.Text = "FFmpeg is ready.";
			info_ffmpeg_status.ForeColor = Color.Green;
		}else
		{
			info_ffmpeg_status.Text = "FFmpeg is missing.";
			info_ffmpeg_status.ForeColor = Color.Red;
		}
	}// -----------------------------------------

	// --
	private void chk_keepTemp_CheckedChanged(object sender, EventArgs e)
	{
		#if (!DEBUG)
			// I don't know how the user got here, but it's a mistake.
			return;
		#endif

		CDCRUSH.FLAG_KEEP_TEMP = chk_keepTemp.Checked;
	}// -----------------------------------------

	// --
	private void btn_toolsTest_Click(object sender, EventArgs e)
	{
		var f = new FormComponentsTest();
			f.ShowDialog();
	}// -----------------------------------------

	private void label12_Click(object sender, EventArgs e)
	{
		if(FormLog.ISOPEN) return;
		var f = new FormLog();
			f.Show();
	}// -----------------------------------------

	}// -- end class
}// -- end namespace
