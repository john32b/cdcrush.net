using System;
using System.IO;
using System.Windows.Forms;
using cdcrush.lib;
using cdcrush.lib.app;

namespace cdcrush.forms
{

	/**
	 * Test form to test various functionalities
	 * ----
	 * IN DEVELOPMENT
	 * ------------------------------------------*/

	public partial class FormComponentsTest : Form
	{
		// --
		public FormComponentsTest()
		{
			InitializeComponent();
		}// --
		
		
		private void FormComponentsTest_FormClosing(object sender, FormClosingEventArgs e)
		{
			LOG.log("[FormComponents] Destroying");
			LOG.detachTextBox();
		}// -----------------------------------------

		// --
		private void FormComponentsTest_Load(object sender, EventArgs e)
		{
			LOG.log("Components Form Load ------");
			LOG.attachTextBox(textbox_log);
			FormTools.fileLoadDialogPrepare("ecm", "ECM files (*.ecm)|*.ecm");
			FormTools.fileLoadDialogPrepare("arc", "ARC files (*.arc)|*.arc");
			FormTools.fileLoadDialogPrepare(); // all files
		}// -----------------------------------------


		// -
		// Select files to apply operations
		// -
		string[] SELECTED_FILES;
		private void txt_files_Click(object sender, EventArgs e)
		{
			SELECTED_FILES = FormTools.fileLoadDialog("all","",true);
			if(SELECTED_FILES != null)
			{
				txt_files.Text = string.Join(",", SELECTED_FILES);
				LOG.log(txt_files.Text);
			}else{
				txt_files.Text = "";
				SELECTED_FILES = null;
			}
		}// -----------------------------------------

		// --
		// Create ARC 
		private void btn_arc_Click(object sender, EventArgs e)
		{
			if(SELECTED_FILES==null) return;
			
			var app = new FreeArc(prog.CDCRUSH.TOOLS_PATH);
			app.onComplete = (s) => {
				LOG.log("--FreeArc Complete :: {0}", s);
			};

			app.onProgress = (p) => {
				LOG.log("--FreeArc Got Progress :: {0}", p);
			};

			string destFolder = Path.GetDirectoryName(SELECTED_FILES[0]);
			string destFilename = Path.GetFileNameWithoutExtension(SELECTED_FILES[0]);
			string finalpath = Path.Combine(destFolder,destFilename + "_test_.arc");

			if(!string.IsNullOrEmpty(txt_files_2.Text)){
				finalpath = txt_files_2.Text;
			}

			app.compress(SELECTED_FILES,finalpath,7);
		}// -----------------------------------------

		// --
		// UN ARC
		private void btn_unarc_Click(object sender, EventArgs e)
		{
			if(SELECTED_FILES==null) return;

			var app = new FreeArc(prog.CDCRUSH.TOOLS_PATH);
			app.onComplete = (s) => {
				LOG.log("--FreeArc Complete :: {0}", s);
			};
			app.onProgress = (p) => {
				LOG.log("--FreeArc Got Progress :: {0}", p);
			};
			string destFolder = Path.GetDirectoryName(SELECTED_FILES[0]);
			string destFilename = Path.GetFileNameWithoutExtension(SELECTED_FILES[0]);
			string finalpath = Path.Combine(destFolder,destFilename + "_test_");
			if(!string.IsNullOrEmpty(txt_files_2.Text)){
				finalpath = txt_files_2.Text;
			}
			app.extract(SELECTED_FILES[0],finalpath);
		}// -----------------------------------------


		// 
		private void btn_ecm_Click(object sender, EventArgs e)
		{
			if(SELECTED_FILES==null) return;
			var app = new EcmTools(prog.CDCRUSH.TOOLS_PATH);
			app.onComplete = (s) => {
				LOG.log("--ECM Complete :: {0}", s);
			};
			app.onProgress = (p) => {
				LOG.log("--ECM Progress :: {0}", p);
			};
			app.ecm(SELECTED_FILES[0]);
		}// -----------------------------------------

		private void btn_unecm_Click(object sender, EventArgs e)
		{
			if(SELECTED_FILES==null) return;
			var app = new EcmTools(prog.CDCRUSH.TOOLS_PATH);
			app.onComplete = (s) => {
				LOG.log("--ECM Complete :: {0}", s);
			};
			app.onProgress = (p) => {
				LOG.log("--ECM Progress :: {0}", p);
			};
			app.unecm(SELECTED_FILES[0]);
		}// -----------------------------------------

		private void btn_tak_Click(object sender, EventArgs e)
		{
			if(SELECTED_FILES==null) return;
			var app = new Tak(prog.CDCRUSH.TOOLS_PATH);
			app.onComplete = (s) => {
				LOG.log("-- WAV to TAK Complete :: {0}", s);
			};
			app.encode(SELECTED_FILES[0],txt_files_2.Text);
		}

		private void btn_untak_Click(object sender, EventArgs e)
		{
			if(SELECTED_FILES==null) return;
			var app = new Tak(prog.CDCRUSH.TOOLS_PATH);
			app.onComplete = (s) => {
				LOG.log("--TAK to WAV Complete :: {0}", s);
			};
			app.decode(SELECTED_FILES[0],txt_files_2.Text);
		}

		private void btn_tak_pcm_Click(object sender, EventArgs e)
		{
			if(SELECTED_FILES==null) return;
				
				var ffmp = new FFmpeg(prog.CDCRUSH.FFMPEG_PATH);
				var tak = new Tak(prog.CDCRUSH.TOOLS_PATH);

				tak.onComplete = (s) =>	{
					LOG.log($"TAK Operation Complete - {s}");
				};

				ffmp.onComplete = (s) => {
					LOG.log($"FFMPEG Operation Complete - {s}");
				};

				string INPUT = SELECTED_FILES[0];
				string OUTPUT = Path.ChangeExtension(INPUT,".tak");
			
				// This will make FFMPEG read the PCM file, convert it to WAV on the fly
				// and feed it to TAK, which will convert and save it.
				
				ffmp.convertPCMStreamToWavStream( (ffmpegIn,ffmpegOut) => {
					var sourceFile = File.OpenRead(INPUT);
					tak.encodeFromStream(OUTPUT, (takIn) => { 
						ffmpegOut.CopyTo(takIn);
						takIn.Close();
					});
					sourceFile.CopyTo(ffmpegIn);	// Feed PCM to FFMPEG
					ffmpegIn.Close();
				});
		}// --

		private void btn_untak_pcm_Click(object sender, EventArgs e)
		{
			if(SELECTED_FILES==null) return;

			string INPUT = SELECTED_FILES[0];
			string OUTPUT = Path.ChangeExtension(INPUT,".pcm");
			var ffmp = new FFmpeg(prog.CDCRUSH.FFMPEG_PATH);
			var tak = new Tak(prog.CDCRUSH.TOOLS_PATH);

			tak.onComplete = (s) =>	{
				LOG.log($"TAK Operation Complete - {s}");
			};

			ffmp.onComplete = (s) => {
				LOG.log($"FFMPEG Operation Complete - {s}");
			};

			tak.decodeToStream(INPUT,(_out) => {
				ffmp.convertWavStreamToPCM(OUTPUT,(_in)=>{
					_out.CopyTo(_in);
					_in.Close();
				});
			});

		}// --

		private void btn_7z_Click(object sender, EventArgs e)
		{
			if(SELECTED_FILES==null) return;
			
			var app = new SevenZip(prog.CDCRUSH.TOOLS_PATH);
			app.onComplete = (s) => {
				LOG.log("--7ZIP Complete :: {0}, SIZE:", s);
				LOG.log(app.COMPRESSED_SIZE);
			};

			app.onProgress = (p) => {
				LOG.log("--7ZIP Got Progress :: {0}", p);
			};

			string destFolder = Path.GetDirectoryName(SELECTED_FILES[0]);
			string destFilename = Path.GetFileNameWithoutExtension(SELECTED_FILES[0]);
			string finalpath = Path.Combine(destFolder,destFilename + "_test_.7z");

			if(!string.IsNullOrEmpty(txt_files_2.Text)){
				finalpath = txt_files_2.Text;
			}

			app.compress(SELECTED_FILES,finalpath,1);
		}



		private void btn_7unz_Click(object sender, EventArgs e)
		{
			if(SELECTED_FILES==null) return;

			var app = new SevenZip(prog.CDCRUSH.TOOLS_PATH);
			app.onComplete = (s) => {
				LOG.log("--7zip Complete :: {0}", s);
			};
			app.onProgress = (p) => {
				LOG.log("--7zip Got Progress :: {0}", p);
			};
			string destFolder = Path.GetDirectoryName(SELECTED_FILES[0]);
			string destFilename = Path.GetFileNameWithoutExtension(SELECTED_FILES[0]);
			string finalpath = Path.Combine(destFolder,destFilename + "_test_");
			
			if(!string.IsNullOrEmpty(txt_files_2.Text)){
				finalpath = txt_files_2.Text;
			}

			app.extract(SELECTED_FILES[0],finalpath);
		}



	}// --
}// --
