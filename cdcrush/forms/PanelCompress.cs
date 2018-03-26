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
	string loadedCuePath;
	// This holds the info of the CD that was just compressed. Set AFTER crushing
	CueReader postCdInfo;
	// Hold the number of tracks of the quickloadedCD
	int numberOfTracks;

	// -----------------------------------------
	public PanelCompress()
	{
		InitializeComponent();
	}// -----------------------------------------

	// --
	private void PanelCompress_Load(object sender, EventArgs e)
	{
		// -- Some Tooltips
         ToolTip tt = new ToolTip();		 
		 tt.SetToolTip(chk_encodedCue, "Encodes audio tracks and creates a .CUE file that handles data and encoded audio tracks. Doesn't create a final archive. This format can be used in some emulators.");
		 tt.SetToolTip(pictureBox1,"You can optionally set an image cover for this CD and it will be stored in the archive.");
         
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
		form_set_crushed_size();
		loadedCuePath = null;
		postCdInfo = null;
		
		// --
		form_set_proper_action_name();
		combo_data_c.SelectedIndex = 3;
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
				chk_encodedCue.Enabled = !_lock;
				input_in.Enabled = !_lock;
				btn_input_in.Enabled = !_lock;
				btn_input_out.Enabled = !_lock;
				input_out.Enabled = !_lock;
				//--
				info_cdtitle.Enabled = !_lock;
				combo_audio_q.Enabled = !_lock;
				combo_audio_c.Enabled = !_lock;
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
	/// Call this to change the main button's name depending on the `convert` checkbox
	/// </summary>
	void form_set_proper_action_name()
	{
		btn_CRUSH.Text = chk_encodedCue.Checked?"CONVERT":"CRUSH";
	}// -----------------------------------------


	/// <summary>
	/// Set some CD infos from a CUE file
	/// </summary>
	/// <param name="cdInfo"></param>
	void form_set_info_pre(dynamic cdInfo = null)
	{
		btn_chksm.Enabled = false;

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
	/// <param name="size"></param>
	void form_set_crushed_size(int size = 0)
	{
		if(size==0) {
			info_size0.Text = "";
		}else{
			info_size0.Text =  String.Format("{0}MB", FormTools.bytesToMB(size));
		}
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
		form_set_cover(null);	// Empty it out
		form_set_info_pre(null);
		form_set_crushed_size(); // Empty it out
		loadedCuePath = null;
		postCdInfo = null;
		numberOfTracks = 0;

		// Clear the status infos at next tab change
		FormMain.FLAG_REQUEST_STATUS_CLEAR = true;

		if (o == null) // Error
		{
			FormMain.sendMessage(CDCRUSH.ERROR, 3);
		}
		else
		{
			loadedCuePath = file;
			input_in.Text = file;
			form_lockSection("action", false);
			form_set_info_pre(o);
			numberOfTracks = o.tracks;
			FormMain.sendMessage("Loaded CUE OK.", 2);
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
			FormMain.sendMessage("Unsupported file extension. Drop a .CUE file",3);
		}
	}// -----------------------------------------

	// --
	// Event Clicked Compressed Button
	private void btn_CRUSH_Click(object sender, EventArgs e)
	{
		// Get a valid audio parameters tuple
		Tuple<int,int> audioQ = Tuple.Create(combo_audio_c.SelectedIndex,combo_audio_q.SelectedIndex);

		// Since I can fire 2 jobs from here, have a common callback
		Action<bool,int,CueReader> jobCallback = (complete, newSize, cd) => {
			FormTools.invoke(this, () =>{

				form_lockSection("all", false);
				form_set_crushed_size(newSize);
				postCdInfo = cd;	// Either null or full info

				if(complete) {
					FormMain.sendMessage("Complete", 2);
					btn_chksm.Enabled = true;
					
				}else 
				{
					FormMain.sendProgress(0);
					FormMain.sendMessage(CDCRUSH.ERROR,3);
				}
			});
		};

		// Reset the message color, incase it was red
		FormMain.sendMessage("", 1);

		// Engine request job result
		bool res = false;

		// Fix progress reporting. HACKY WAY :-/
		CDCRUSH.HACK_CD_TRACKS = numberOfTracks;

		// Either compress to an archive, or just convert
		// Note : Progress updates are automatically being handled by the main FORM
		if(chk_encodedCue.Checked) {
			res = CDCRUSH.startJob_ConvertCue(loadedCuePath, input_out.Text, audioQ, 
				info_cdtitle.Text, jobCallback);
		}else {
			res = CDCRUSH.startJob_CrushCD(loadedCuePath, input_out.Text, audioQ, 
				preparedCover, info_cdtitle.Text, combo_data_c.SelectedIndex+1, jobCallback);
		}
		
		// -- Is everything ok?
		if(res) {
			form_lockSection("all", true);
		}else{
			FormMain.sendMessage(CDCRUSH.ERROR, 3);
		}

	}// -----------------------------------------

	// --
	// Event: Input Folder Select
	// + Will present the folder of the currently loaded CUE file first
	private void btn_input_in_Click(object sender, EventArgs e)
	{
		string lastDir = null;
		if (loadedCuePath != null) lastDir = System.IO.Path.GetDirectoryName(loadedCuePath);
		var files = FormTools.fileLoadDialog("cue",lastDir);
		if (files == null) return;
		form_quickLoadFile(files[0]);
	}// -----------------------------------------

	// --
	// Event: Output Folder Select
	// + Will present the folder of the currently loaded CUE file first
	private void btn_input_out_Click(object sender, EventArgs e)
	{
		string lastDir = null;
		if (loadedCuePath != null) lastDir = System.IO.Path.GetDirectoryName(loadedCuePath);
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
	// Cover Image Clicked
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

	// --
	// Codec was changed, fill out the quality combobox
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
				for(int i=0;i<FFmpeg.VORBIS_QUALITY.Length;i++) {
					combo_audio_q.Items.Add(FFmpeg.VORBIS_QUALITY[i].ToString() + "k Vbr");
				}
				combo_audio_q.Enabled = true;
				break;
			case 2: // OPUS
				for(int i=0;i<FFmpeg.OPUS_QUALITY.Length;i++) {
					combo_audio_q.Items.Add(FFmpeg.OPUS_QUALITY[i].ToString() + "k Vbr");
				}				
				combo_audio_q.Enabled = true;
				break;
			case 3: // MP3
				for(int i=0;i<FFmpeg.MP3_QUALITY.Length;i++) {
					combo_audio_q.Items.Add(FFmpeg.MP3_QUALITY[i].ToString() + "k Vbr");
				}
				combo_audio_q.Enabled = true;
				break;
		}

		combo_audio_q.SelectedIndex = 0;
	}// -----------------------------------------

	// --
	// Encode Instead Checkbox
	private void chk_encodedCue_CheckedChanged(object sender, EventArgs e)
	{
		form_set_proper_action_name();
	}// -----------------------------------------

	// --
	private void btn_chksm_Click(object sender, EventArgs e)
	{
		FormMain.showCdInfo(postCdInfo);
	}// -----------------------------------------

	}// --
}// --
