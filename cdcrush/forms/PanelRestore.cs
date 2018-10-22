using System;
using System.Windows.Forms;
using cdcrush.lib;
using cdcrush.prog;

namespace cdcrush.forms
{

/**
 * - Control that offers parameters for CD RESTORING
 * - Displays information for the ARC to be restored from the ENGINE
 */
public partial class PanelRestore : UserControl
{
	// This is the last quick loaded ARC file dropped/opened
	// It's a valid file everytime, since it is checked
	public string loadedArcPath;

	// CD Info of the CD prepared to be restored
	// Primarily used to getting the checksum data
	cd.CDInfos loadedCDInfo;

	// -----------------------------------------

	// --
	public PanelRestore()
	{
		InitializeComponent();
	}// --

	// --
	private void PanelRestore_Load(object sender, EventArgs e)
	{
		FormTools.fileLoadDialogPrepare("arc", "ARC files (*.arc)|*.arc");
		form_lockSection("action", true);
		form_setCdInfo(null);
		form_setCoverImage(null);
		loadedArcPath = null;
		loadedCDInfo = null;
	}// -----------------------------------------



	/// <summary>
	/// LOCK parts of the form
	/// </summary>
	/// <param name="_section">input,action,all</param>
	/// <param name="_lock"></param>
	public void form_lockSection(string _section,bool _lock)
	{		
		switch(_section)
		{
			case "action":
				btn_RESTORE.Enabled = !_lock;
				break;
			case "all":
				input_in.Enabled = !_lock;
				btn_input_in.Enabled = !_lock;
				btn_input_out.Enabled = !_lock;
				input_out.Enabled = !_lock;
				toggle_merged.Enabled = !_lock;
				toggle_subf.Enabled = !_lock;

				form_lockSection("action", _lock);
				FormMain.sendLock(_lock);
				break;
		}
	}// -----------------------------------------



	/// <summary>
	/// Set form infos from a CueReader object
	/// - Called when quickloading a CD
	/// - Called to clear fields
	/// </summary>
	/// <param name="cdInfo">Object CDCURSH,loadQuickInfo() sends</param>
	void form_setCdInfo(dynamic cdInfo = null)
	{
		if(cdInfo==null)
		{
			// set all to none
			info_cdtitle.Text = "";
			info_size0.Text = "";	// Small
			info_size1.Text = "";	// Full
			info_tracks.Text = "";
			info_audio.Text = "";
			btn_chksm.Enabled = false;
			loadedCDInfo = null;
			return;
		}

		loadedCDInfo = cdInfo.cd;
		btn_chksm.Enabled = true;

		int numberOfTracks = loadedCDInfo.tracks.length;

		info_cdtitle.Text = loadedCDInfo.CD_TITLE;
		info_size0.Text =  String.Format("{0}MB", FormTools.bytesToMB(cdInfo.sizeArc));
		info_size1.Text =  String.Format("{0}MB", FormTools.bytesToMB(loadedCDInfo.CD_TOTAL_SIZE));
		info_audio.Text = loadedCDInfo.CD_AUDIO_QUALITY;
		info_tracks.Text = numberOfTracks.ToString();

		// Init some controls dependant on track count
		toggle_merged.Enabled = (numberOfTracks>1);
		toggle_cueaudio.Enabled = (numberOfTracks>1);

		if(numberOfTracks==1)
		{
			toggle_merged.Checked = false;
			toggle_cueaudio.Checked = false;
		}

	}// -----------------------------------------


	/// <summary>
	/// Sets an image on the cover image placeholder,
	/// It will default on a predefined graphic if any error or null
	/// </summary>
	/// <param name="file">If NULL will set default ICON</param>
	public void form_setCoverImage(string file)
	{
		if(!FormTools.imageSetFile(pictureBox1, file))
		{
			pictureBox1.Image = Properties.Resources.cd128;
		}
	}// -----------------------------------------



	/// <summary>
	/// Quick load an .arc file and display info about the CD loaded
	/// </summary>
	void form_quickLoadFile(string file)
	{
		//	-
		Action<object> onLoad = (o) => 
		{
			FormTools.invoke(this, () => {

				FormMain.sendProgress(0);
				form_lockSection("all", false);

				if(o == null) 
				{
					LOG.log("QuickLoad ERROR - " + CDCRUSH.ERROR);
					FormMain.sendMessage(CDCRUSH.ERROR, 3);
					form_lockSection("action", true);
					return;
				}

				// This file will be restored when the button is clicked
				loadedArcPath = file;

				input_in.Text = file;
				form_setCdInfo(o);
				form_setCoverImage((o as dynamic).cover); // Note: Cover file may not exist
				FormMain.sendMessage("Loaded Info OK.", 2);
				btn_RESTORE.Focus();

			});

		}; // --

		// Clear the status infos at next tab change
		FormMain.FLAG_REQUEST_STATUS_CLEAR = true;

		if(CDCRUSH.loadQuickInfo(file, onLoad))
		{
			// Waiting to load quick info : Lock Form
			form_lockSection("all", true);
			FormMain.sendMessage("Reading Information ..", 1);
			FormMain.sendProgress(-1);
		}else{
			
			onLoad(null);	// hacky way to have one codebase for errors
		}

	}// -----------------------------------------

	// =============================================
	// = EVENTS
	// =============================================

	/// <summary>
	/// A file has been dropped and the ENGINE is NOT LOCKED
	/// </summary>
	/// <param name="file"></param>
	public void handle_dropped_file(string file)
	{
		// handle the file
		if (System.IO.Path.GetExtension(file).ToLower() == ".arc")
			form_quickLoadFile(file);
		else{
			FormMain.sendMessage("Unsupported file type. Drop an .ARC file", 3);
		}
	}// -----------------------------------------


	// --
	private void btn_RESTORE_Click(object sender, EventArgs e)
	{
		// --
		FormMain.sendMessage("", 1);

		// Send the nuber of tracks for proper progress reporting
		CDCRUSH.HACK_CD_TRACKS = loadedCDInfo.tracks.length;

		// Start the job
		// Note, Progress updates are automatically being handled by the main FORM
		bool res = CDCRUSH.startJob_RestoreCD(loadedArcPath, input_out.Text, 
			toggle_subf.Checked, toggle_merged.Checked, toggle_cueaudio.Checked,
			(complete)=>{
				FormTools.invoke(this, () =>
				{
					form_lockSection("all", false);
					
					if(complete)
					{
						// DEVNOTE: Should I lock the button in case user presses it again?
						//			I don't think so right?
						// form_lockSection("action", true);
						FormMain.sendMessage("Complete", 2);
					}else 
					{
						// job update-fail won't push progress, do it manually
						FormMain.sendProgress(0);
						FormMain.sendMessage(CDCRUSH.ERROR,3);
					}

					// Make progress bar and status message clear after
					FormMain.FLAG_REQUEST_STATUS_CLEAR = true;
				});
			});

		if(res){
			form_lockSection("all", true);
		}else{
			FormMain.sendMessage(CDCRUSH.ERROR, 3);
		}
	}// -----------------------------------------

	// --
	// Select an .ARC file, It's info will be read and
	// will be readied to be processed
	private void btn_input_in_Click(object sender, EventArgs e)
	{
		var files = FormTools.fileLoadDialog("arc");
		if (files == null) return;
		form_quickLoadFile(files[0]);
	}// -----------------------------------------

	// --
	// Select and set an output folder
	private void btn_input_out_Click(object sender, EventArgs e)
	{
		string lastDir = null;
		if (loadedArcPath != null) lastDir = System.IO.Path.GetDirectoryName(loadedArcPath);
		SaveFileDialog d = new SaveFileDialog();
		d.FileName = "HERE.cue";
		d.CheckPathExists = true;
		d.InitialDirectory = lastDir;
		d.Title = "Select output folder";

		if (d.ShowDialog() == DialogResult.OK)
		{
			string path;
			try{
				path = System.IO.Path.GetDirectoryName(d.FileName);
			}catch(ArgumentException)
			{
				path = null;
			}

			input_out.Text = path;
		}
	}// -----------------------------------------

	// --
	// Control the state of the `merged` toggle
	private void toggle_cueaudio_CheckedChanged(object sender, EventArgs e)
	{
		// Avoid non-user sets
		if(!toggle_cueaudio.Enabled) return; 

		if(toggle_cueaudio.Checked)
		{
			toggle_merged.Checked = false;
			toggle_merged.Enabled = false;
		}else{
			toggle_merged.Enabled = true;
		}
	}// -----------------------------------------

	// --
	private void btn_chksm_Click(object sender, EventArgs e)
	{
		FormMain.showCdInfo(loadedCDInfo);
	}// -----------------------------------------

	}// --
}// --
