using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using cdcrush.lib;
using cdcrush.prog;

namespace cdcrush.forms
{

/**
 * - Control that offers parameters for CD RESTORING
 * - Displays information for the ARC to be restored from the ENGINE
 * - 
 * 
 */
public partial class PanelRestore : UserControl
{
	// This is the last quick loaded ARC file dropped/opened
	// It's a valid file everytime, since it is checked
	public string preparedArcPath;



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
		preparedArcPath = null;
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
				toggle_single.Enabled = !_lock;
				toggle_subf.Enabled = !_lock;

				form_lockSection("action", _lock);
				FormMain.sendLock(_lock);
				break;
		}
	}// -----------------------------------------



	/// <summary>
	/// Set form infos from a CueReader object
	/// </summary>
	/// <param name="cdInfo">title,size0,size1,tracks,audio</param>
	void form_setCdInfo(dynamic cdInfo = null)
	{
		if(cdInfo==null)
		{
			// set all to none
			info_cdtitle.Text = "";
			info_size0.Text = "";
			info_size1.Text = "";
			info_tracks.Text = "";
			info_audio.Text = "";
			info_md5.Text = "";
			return;
		}

		info_cdtitle.Text = cdInfo.title;
		info_size0.Text =  String.Format("{0}MB", FormTools.bytesToMB(cdInfo.size0));
		info_size1.Text =  String.Format("{0}MB", FormTools.bytesToMB(cdInfo.size1));
		info_audio.Text = cdInfo.audio;
		info_tracks.Text = String.Format("{0}", cdInfo.tracks);
		info_md5.Text = cdInfo.md5;
	}// -----------------------------------------


	/// <summary>
	/// 
	/// </summary>
	/// <param name="file">If NULL will set default ICON</param>
	public void form_setCoverImage(string file)
	{
		if(!FormTools.imageSetFile(pictureBox1, file))
		{
			pictureBox1.Image = cdcrush.Properties.Resources.cd128;
		}
	}// -----------------------------------------



	/// <summary>
	/// Quick load an .arc file and display info
	/// </summary>
	void form_quickLoadFile(string file)
	{
		Action<Object> onLoad = (o) =>
		{
			FormTools.invoke(this,()=>{

				FormMain.sendProgress(0);
				form_lockSection("all", false);

					if(o==null)
					{
						//form_setText(CDCRUSH.ERROR, 3);
						LOG.log("ERROR - " + CDCRUSH.ERROR);
						return;
					}

				// This file will be restored when the button is clicked
				preparedArcPath = file;

				input_in.Text = file;
				form_setCdInfo(o);
				form_setCoverImage((o as dynamic).cover); // Note: Cover file may not exist
				FormMain.sendMessage("Ready.", 2);
				btn_RESTORE.Focus();
			});
		}; // --
		
		if(CDCRUSH.loadQuickInfo(file,onLoad))
		{
			// Waiting to load quick info : Lock Form
			form_lockSection("all", true);
			FormMain.sendMessage("Reading Information ..", 1);
			FormMain.sendProgress(-1);
		}else
		{
			// Something bad happened
			LOG.log(CDCRUSH.ERROR);
			FormMain.sendMessage(CDCRUSH.ERROR, 3);
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
		else
			FormMain.sendMessage("Unsupported file type. Drop an `.arc` file", 3);
	}// -----------------------------------------


	// --
	private void btn_RESTORE_Click(object sender, EventArgs e)
	{
		if (CDCRUSH.LOCKED) return; // for any reason
		form_lockSection("all", true);

		// Starts the job
		// Note, Progress updates are automatically being handled by the main FORM
		// I will only handle fail statuses for now
		CDCRUSH.restoreARC(preparedArcPath, input_out.Text, toggle_subf.Checked, toggle_single.Checked,
			(complete)=>{
				FormTools.invoke(this, () =>
				{
					form_lockSection("all", false);
					form_lockSection("action", true); // Just restored this image, why restore it again?
				});

				if(complete)
				{
					FormMain.sendMessage("Complete", 2);
				}else
				{
					FormMain.sendMessage(CDCRUSH.ERROR,3);
				}
			});
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
		if (preparedArcPath != null) lastDir = System.IO.Path.GetDirectoryName(preparedArcPath);
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


}// --
}// --
