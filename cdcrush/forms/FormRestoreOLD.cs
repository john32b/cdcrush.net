using cdcrush.lib;
using cdcrush.prog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cdcrush.forms
{
public partial class FormRestoreOLD : Form
{

	// Hold the last successful QUICK LOAD arc file
	string quickLoadedArc;
	//===================================================


	// --
	public FormRestoreOLD()
	{
		InitializeComponent();
	}// -----------------------------------------



	// --
	private void FormRestore_Load(object sender, EventArgs e)
	{
		// Tooltips :: -----
			// Create the ToolTip and associate with the Form container.
			ToolTip toolTip1 = new ToolTip();
			// Set up the delays for the ToolTip.
			toolTip1.AutoPopDelay = 5000;
			toolTip1.InitialDelay = 600;
			toolTip1.ReshowDelay = 200;
			// - Set the tooltips :
			toolTip1.SetToolTip(toggle_single, "Restore the CD to a single CUE/BIN archive");
			toolTip1.SetToolTip(toggle_subf, "Create a subfolders for the restored CDs");

		// Drag Drop -> TODO:
			FormTools.dragDropFormEnable(this, (string[] files) => {
				if (CDCRUSH.LOCKED) return;
				Trace.WriteLine("DROPPED FILE(s) :");
				Trace.Indent();
				foreach (string file in files) Trace.WriteLine(file);
				Trace.Unindent();
				// --
				form_quickLoadFile(files[0]);
			});

		// -- INIT  :: -----
		// --
		CDCRUSH.init();
		// --
		FormTools.fileLoadDialogPrepare();

		// --
		quickLoadedArc = null;
		// The Form starts with everything unlocked, so just lock the "RESTORE" button
		form_section_lock("action",true);
		// Empty the infos
		form_setCdInfo(null);

		// --
		form_setText("Ready.",1);
		form_setProgress(0);

		
	}// -----------------------------------------


	// -- 
	private void btn_RESTORE_Click(object sender, EventArgs e)
	{
		// Checks for LOCKED, valid files:
		CDCRUSH.restoreARC(quickLoadedArc, input_out.Text, null, toggle_subf.Checked, toggle_single.Checked);
		form_section_lock("all", true);

		// attach progress bar
		// prepare to accept messages etc
	}// -----------------------------------------


	/// <summary>
	/// LOCK parts of the form
	/// </summary>
	/// <param name="_section">input,action,all</param>
	/// <param name="_lock"></param>
	void form_section_lock(string _section,bool _lock)
	{		
		switch(_section)
		{
			case "input":
				input_in.Enabled = !_lock;
				btn_input_in.Enabled = !_lock;
				btn_input_out.Enabled = !_lock;
				input_out.Enabled = !_lock;
				toggle_single.Enabled = !_lock;
				toggle_subf.Enabled = !_lock;
				break;
			case "action":
				btn_RESTORE.Enabled = !_lock;
				break;
			case "all":
				form_section_lock("input", _lock);
				form_section_lock("action", _lock);
				break;
		}
	}// -----------------------------------------


	/// <summary>
	/// Set form infos from a CueReader object
	/// </summary>
	/// <param name="cdInfo">title,size,type,tracks,cover</param>
	void form_setCdInfo(dynamic cdInfo = null)
	{
		if(cdInfo==null)
		{
			// set all to none
			info_cdtitle.Text = "";
			info_size0.Text = "";
			info_size1.Text = "";
			info_tracks.Text = "";
			info_type.Text = "";
			return;
		}

		info_cdtitle.Text = cdInfo.title;
		info_size0.Text =  String.Format("{0}MB", FormTools.bytesToMB(cdInfo.size0));
		info_size1.Text =  String.Format("{0}MB", FormTools.bytesToMB(cdInfo.size1));
		info_type.Text = cdInfo.type;
		info_tracks.Text = String.Format("{0}", cdInfo.tracks);
	}// -----------------------------------------

	/// <summary>
	/// 
	/// </summary>
	/// <param name="file">If NULL will set default ICON</param>
	public void form_setCoverImage(string file)
	{
		if (file == null) goto defIcon;

		try{
			Image image;
			using (var bmpTemp = new Bitmap(file)) {
				image = new Bitmap(bmpTemp);
			}
			pictureBox1.Image = image;
			pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
			return;
		}
		catch(System.IO.IOException) {goto defIcon;}
		catch(NotSupportedException) {goto defIcon;}
		catch(ArgumentException) {goto defIcon;}

		defIcon:
			if (pictureBox1.Image != null) pictureBox1.Image.Dispose();
			pictureBox1.Image = cdcrush.Properties.Resources.cd128;
	}// -----------------------------------------


	// --
	public void form_setProgress(int per)
	{
		if (per < 0)
		{
			progressBar1.Style = ProgressBarStyle.Marquee;
			progressBar1.MarqueeAnimationSpeed = 10;
			return;
		}else
		if(per==0)
		{
			FormTools.invoke(progressBar1,()=>{progressBar1.Style = ProgressBarStyle.Blocks;});
		}

		if (per > 100) per = 100;

		FormTools.invoke(progressBar1,()=>{progressBar1.Value = per;});
	}
	//===================================================================


	Color[] FORM_STATUS_COLORS = new[] {SystemColors.GrayText,Color.Green,Color.Red};

	/// <summary>
	/// 
	/// </summary>
	/// <param name="msg"></param>
	/// <param name="type">1:normal, 2:green, 3:red</param>
	public void form_setText(string msg="", int type=0)
	{
		info_status.Text = msg;
		if(type>0)
		{
			info_status.ForeColor = FORM_STATUS_COLORS[type-1];
		}
	}// -----------------------------------------

	/// <summary>
	/// QUICK LOAD AN ARC FILE AND DISPLAY INFO
	/// </summary>
	void form_quickLoadFile(string file)
	{
		Action<Object> onLoad = (o) =>
		{
			FormTools.invoke(this,()=>{

				form_setProgress(0);
				form_section_lock("all", false);

					if(o==null)
					{
						form_setText(CDCRUSH.ERROR, 3);
						LOG.log("ERROR - " + CDCRUSH.ERROR);
						return;
					}

				// This file will be restored when the button is clicked
				quickLoadedArc = file;

				input_in.Text = file;
				form_setCdInfo(o);
				form_setCoverImage((o as dynamic).cover); // Note: Cover file may not exist
				form_setText("Ready.", 2);
				btn_RESTORE.Focus();

			});
		}; // --
		
		if(CDCRUSH.loadQuickInfo(file,onLoad))
		{
			// Waiting to load quick info : Lock Form
			form_section_lock("all", true);
			form_setText("Reading Information ..", 1);
			form_setProgress(-1);
		}else
		{
			// Something bad happened
			LOG.log(CDCRUSH.ERROR);
			form_setText(CDCRUSH.ERROR, 3);
		}

	}// -----------------------------------------


	/// <summary>
	/// SELECT INPUT FILE ::
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btn_input_in_Click(object sender, EventArgs e)
	{
		var files = FormTools.fileLoadDialog();
		if (files == null) return;
		form_quickLoadFile(files[0]);
	}// -----------------------------------------



	// DEBUG ::
	private void button1_Click(object sender, EventArgs e)
	{
		//var s = FormTools.fileLoadDialog();
		//form_setCoverImage(s);

		var f = new FormComponentsTest();
			f.Show();
	
	}

	/// <summary>
	/// SAVE DIALOG
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btn_input_out_Click(object sender, EventArgs e)
	{
		SaveFileDialog d = new SaveFileDialog();
		d.FileName = "HERE.cue";
		d.CheckPathExists = true;
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

	}







}// -- end class --
}// -- end namespace --
