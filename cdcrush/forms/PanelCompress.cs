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
	cd.CDInfos postCdInfo;
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
		 //tt.SetToolTip(chk_encodedCue, "Encodes audio tracks and creates a .CUE file that handles data and encoded audio tracks. Doesn't create a final archive. This format can be used in some emulators.");
		 tt.SetToolTip(pictureBox1,"You can optionally set an image cover for this CD and it will be stored in the archive.");
         
		// -- Initialize combobox Audio Compression
		foreach(var codecID in AudioMaster.codecs)
		{
			combo_audio_c.Items.Add(AudioMaster.getCodecIDName(codecID));
		}

		combo_audio_c.SelectedIndex = 0;
		combo_audio_c_SelectedIndexChanged(null,null); // force first call? why

		// -- Initialize combobox Archive Compression
		foreach(var cs in ArchiveMaster.compressionStrings)
		{
			combo_data_c.Items.Add(cs);
		}

		combo_data_c.SelectedIndex = ArchiveMaster.DEFAULT_INDEX;

		// -- 
		combo_method.SelectedIndex = 0;

		FormTools.fileLoadDialogPrepare("cue", "CUE files (*.cue)|*.cue");
		FormTools.fileLoadDialogPrepare("cover", "Image files (*.jpg)|*.jpg");

		// --
		form_prepare_init();

	}// -----------------------------------------

	// ==========================================
	// FORM ACTIONS
	// ==========================================


	/// <summary>
	/// Clear CD Infos and Locks 
	/// </summary>
	private void form_prepare_init()
	{
		form_lockSection("action", true);
		form_set_cover(null);	// Empty it out
		form_set_cd_info(null);
		form_set_crushed_size(); // Empty it out
		loadedCuePath = null;
		postCdInfo = null;
		numberOfTracks = 0;
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
				btn_CRUSH.Enabled = !_lock;
				break;
			case "all":
				combo_method.Enabled = !_lock;
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
		if(FormTools.imageSetFile(pictureBox1, file)) {  
			preparedCover = file;
		}
		else {
			pictureBox1.Image = Properties.Resources.dropimage;
			preparedCover = null;
		}
	}// -----------------------------------------


	/// <summary>
	/// Set some CD infos from a CUE file
	/// </summary>
	/// <param name="cdInfo"> {title,size1,tracks}</param>
	void form_set_cd_info(dynamic cdInfo = null)
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

		numberOfTracks = cdInfo.tracks;
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
		form_prepare_init();

		// Clear the status infos at next tab change
		FormMain.FLAG_REQUEST_STATUS_CLEAR = true;

		dynamic o = CDCRUSH.loadQuickCUE(file);

		if (o == null) // Error
		{
			FormMain.sendMessage(CDCRUSH.ERROR, 3);
		}
		else
		{
			// CUE Loaded OK, fill in form infos and unlock buttons

			loadedCuePath = file;
			input_in.Text = file;
			form_lockSection("action", false);
			form_set_cd_info(o);
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
			FormMain.sendMessage("Unsupported file extension. Drop a .CUE file, or .JPG for a cover",3);
		}
	}// -----------------------------------------

	// --
	// Event Clicked Compressed Button
	private void btn_CRUSH_Click(object sender, EventArgs e)
	{									
		// Common callback for all job types
		Action<bool,CrushParams> jobCallback = (complete, jobdata) =>
		{
			FormTools.invoke(this, () => 
			{
				form_lockSection("all", false);
				form_set_crushed_size(jobdata.crushedSize);
				postCdInfo = jobdata.cd;	// Either null or full info

				if(complete) {
					FormMain.sendMessage("Complete", 2);
					btn_chksm.Enabled = true;
				}else {
					FormMain.sendProgress(0);
					FormMain.sendMessage(CDCRUSH.ERROR,3);
				}

				// Make progress bar and status message clear after
				FormMain.FLAG_REQUEST_STATUS_CLEAR = true;

			});
		};


		// Reset the message color, incase it was red
		FormMain.sendMessage("", 1);

		// Fix progress reporting. HACKY WAY :-/
		CDCRUSH.HACK_CD_TRACKS = numberOfTracks;

		// Either compress to an archive, or just convert
		// Note : Progress updates are automatically being handled by the main FORM

		// This TUPLE will hold (CODECID, QUALITY INDEX)
		Tuple<string,int> audioQ = Tuple.Create( AudioMaster.codecs[combo_audio_c.SelectedIndex],
												 combo_audio_q.SelectedIndex );

		bool res = CDCRUSH.startJob_Convert_Crush (
			combo_method.SelectedIndex,
			loadedCuePath,
			input_out.Text,
			audioQ,
			combo_data_c.SelectedIndex,
			info_cdtitle.Text,
			preparedCover,
			jobCallback
		);

		// -- Check preliminary Job Status
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
		SaveFileDialog d = new SaveFileDialog {
			FileName = "HERE.arc",
			CheckPathExists = true,
			InitialDirectory = lastDir,
			Title = "Select output folder"
		};

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

		// Will get available quality options for a CODEC ID
		// Will make checkbox disabled and point it to the default quality based on Codec

		var codecID = AudioMaster.codecs[combo_audio_c.SelectedIndex];
		var qInfo = AudioMaster.getQualityInfos(codecID);

		foreach(string info in qInfo.Item1)
		{
			combo_audio_q.Items.Add(info);
		}

		combo_audio_q.Enabled = qInfo.Item1.Length>0;
		if(combo_audio_q.Enabled) {
			combo_audio_q.SelectedIndex = qInfo.Item2;
		}											

	}// -----------------------------------------

	// --
	private void btn_chksm_Click(object sender, EventArgs e)
	{
		FormMain.showCdInfo(postCdInfo);
	}// -----------------------------------------

	// --
	private void combo_method_SelectedIndexChanged(object sender, EventArgs e)
	{
		switch(combo_method.SelectedIndex)
		{
			case 0: btn_CRUSH.Text = "CRUSH"; break;
			case 1: btn_CRUSH.Text = "CONVERT"; break;
			case 2: btn_CRUSH.Text = "CONVERT INTO ARCHIVE"; break;
		}

		combo_data_c.Enabled = combo_method.SelectedIndex !=1;

	}// -----------------------------------------

}// --
}// --
