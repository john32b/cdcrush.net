using System;
using System.Windows.Forms;
using cdcrush.lib;
using cdcrush.lib.app;
using cdcrush.prog;

namespace cdcrush.forms
{

public partial class PanelCompress : UserControl
{
	// Path of the dropped/selected cover image
	string preparedCover;
	string preparedCue;

	// -----------------------------------------
	public PanelCompress()
	{
		InitializeComponent();
	}// -----------------------------------------

	// --
	private void PanelCompress_Load(object sender, EventArgs e)
	{
		// -- Initialize Audio Settings:
		foreach(string scodec in CDCRUSH.AUDIO_CODECS)
		{
			combo_audio_c.Items.Add(scodec);
		}

		combo_audio_c.SelectedIndex = 0;
		combo_audio_c_SelectedIndexChanged(null,null); // force first call? why

		FormTools.fileLoadDialogPrepare("cue", "CUE files (*.cue)|*.cue");
		FormTools.fileLoadDialogPrepare("cover", "Image files (*.jpg)|*.jpg");

		// --
		form_lockSection("action", true);
		form_set_cover(null); // will also set preparedCover
		form_set_info_pre(null);
		form_set_info_post(null);
		preparedCue = null;
		
	}// -----------------------------------------

	// ==========================================
	// FORM ACTIONS
	// ==========================================

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
				btn_CRUSH.Enabled = !_lock;
				break;
			case "all":
				input_in.Enabled = !_lock;
				btn_input_in.Enabled = !_lock;
				btn_input_out.Enabled = !_lock;
				input_out.Enabled = !_lock;
				//--
				info_cdtitle.Enabled = !_lock;
				combo_audio_q.Enabled = !_lock;
				pictureBox1.Enabled = !_lock;
				form_lockSection("action", _lock);
				FormMain.sendLock(_lock);
				break;
		}
	}// -----------------------------------------

	/// <summary>
	/// Checks input file and if valid it sets it
	/// </summary>
	/// <param name="file"></param>
	void form_set_cover(string file)
	{
		if(FormTools.imageSetFile(pictureBox1, file))
		{
			// OK
			preparedCover = file;
		}
		else
		{
			// FAIL
			pictureBox1.Image = Properties.Resources.dropimage;
			preparedCover = null;
		}
	}// -----------------------------------------

	/// <summary>
	/// Set some CD infos from a CUE file
	/// </summary>
	/// <param name="cdInfo"></param>
	void form_set_info_pre(dynamic cdInfo = null)
	{
		if(cdInfo==null)
		{
			// set all to none
			info_cdtitle.Text = "";
			info_size1.Text = "";
			info_tracks.Text = "";
			return;
		}

		info_cdtitle.Text = cdInfo.title;
		info_size1.Text =  String.Format("{0}MB", FormTools.bytesToMB(cdInfo.size1));
		info_tracks.Text = String.Format("{0}", cdInfo.tracks);
	}// -----------------------------------------

	/// <summary>
	/// Set some CD infos from data gathered AFTER crushing
	/// </summary>
	/// <param name="cdInfo"></param>
	void form_set_info_post(dynamic cdInfo = null)
	{
		if(cdInfo==null)
		{
			// set all to none
			info_size0.Text = "";
			//info_md5.Text = "";
			return;
		}

		info_size0.Text =  String.Format("{0}MB", FormTools.bytesToMB(cdInfo.size0));
	//	info_md5.Text = cdInfo.md5;
	}// -----------------------------------------

	/// <summary>
	/// Quick load a `.cue` file, called when dropped and open file
	/// Check and prepare
	/// </summary>
	void form_quickLoadFile(string file)
	{
		dynamic o = CDCRUSH.loadQuickCUE(file);

		// Reset Infos and Action, unlock on valid file
		form_lockSection("action", true);
		form_set_cover(null); // will also set preparedCover
		form_set_info_pre(null);
		form_set_info_post(null);
		preparedCue = null;

		FormMain.FLAG_CLEAR_STATUS = true;

		if (o == null) // Error
		{
			FormMain.sendMessage(CDCRUSH.ERROR, 3);
		}
		else
		{
			preparedCue = file;
			input_in.Text = file;
			form_lockSection("action", false);
			form_set_info_pre(o);
		}

	}// -----------------------------------------

	// ==========================================
	// EVENTS
	// ==========================================
	
	/// <summary>
	/// A file has been dropped and the ENGINE is NOT LOCKED
	/// </summary>
	/// <param name="file"></param>
	public void handle_dropped_file(string file)
	{
		// handle the file
		var ext = System.IO.Path.GetExtension(file).ToLower();
		if (ext == ".cue")
			form_quickLoadFile(file);
		else if (ext == ".jpg")
			form_set_cover(file);
		else {
			FormMain.sendMessage("Unsupported file extension.",3);
			FormMain.FLAG_CLEAR_STATUS = true;
		}
	}// -----------------------------------------


	/// <summary>
	/// 
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btn_CRUSH_Click(object sender, EventArgs e)
	{

		// Get a valid audio parameters tuple
		Tuple<int,int> audioQ = Tuple.Create(combo_audio_c.SelectedIndex,combo_audio_q.SelectedIndex);

		// Start the job
		// Note, Progress updates are automatically being handled by the main FORM
		bool res = CDCRUSH.crushCD(preparedCue, input_out.Text, audioQ, preparedCover, info_cdtitle.Text,
			(complete, md5, newSize) => {

				FormTools.invoke(this, () =>{
					form_lockSection("all", false);
					form_lockSection("action", true);
					form_set_info_post(new {
						md5,
						size0 = newSize
					});

				});

				if(complete)
				{
					FormMain.sendMessage("Complete", 2);
				}else
				{
					FormMain.sendMessage(CDCRUSH.ERROR,3);
				}
			});

		if(res)
		{
			form_lockSection("all", true);
		}else{
			FormMain.sendMessage(CDCRUSH.ERROR, 3);
			FormMain.FLAG_CLEAR_STATUS = true;
		}
	}// -----------------------------------------

	// --
	private void btn_input_in_Click(object sender, EventArgs e)
	{
		string lastDir = null;
		if (preparedCue != null) lastDir = System.IO.Path.GetDirectoryName(preparedCue);
		var files = FormTools.fileLoadDialog("cue",lastDir);
		if (files == null) return;
		form_quickLoadFile(files[0]);
	}// -----------------------------------------

	// --
	private void btn_input_out_Click(object sender, EventArgs e)
	{
		string lastDir = null;
		if (preparedCue != null) lastDir = System.IO.Path.GetDirectoryName(preparedCue);
		SaveFileDialog d = new SaveFileDialog();
		d.FileName = "HERE.arc";
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
	private void pictureBox1_Click(object sender, EventArgs e)
	{	
		if (CDCRUSH.LOCKED) return;
		var files = FormTools.fileLoadDialog("cover");
		if (files == null)  {
			form_set_cover(null); // Remove image if cancelled
		}else{
			form_set_cover(files[0]);
		}
	}// -----------------------------------------

	/**
	 * Codec was changed, fill out the quality combobox
	 **/
	private void combo_audio_c_SelectedIndexChanged(object sender, EventArgs e)
	{
		combo_audio_q.Items.Clear();

		// Ordering is defined in CDCRUSH.AUDIO_CODECS
		switch(combo_audio_c.SelectedIndex)
		{
			case 0: // FLAC
				// Keep empty
				combo_audio_q.Items.Add("");
				combo_audio_q.Enabled = false;
				break;
			case 1: // VORBIS
				for(int i=0;i<CDCRUSH.VORBIS_QUALITY.Length;i++) {
					combo_audio_q.Items.Add(CDCRUSH.VORBIS_QUALITY[i].ToString() + "k Vbr");
				}
				combo_audio_q.Enabled = true;
				break;
			case 2: // OPUS
				for(int i=0;i<CDCRUSH.OPUS_QUALITY.Length;i++) {
					combo_audio_q.Items.Add(CDCRUSH.OPUS_QUALITY[i].ToString() + "k Vbr");
				}				
				combo_audio_q.Enabled = true;
				break;
		}

		combo_audio_q.SelectedIndex = 0;
	}// -----------------------------------------

}// --
}// --
