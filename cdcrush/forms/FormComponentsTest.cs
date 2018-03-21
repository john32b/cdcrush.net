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
		private void txt_files_TextChanged(object sender, EventArgs e)
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
			app.compress(SELECTED_FILES,finalpath);
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
			app.extractAll(SELECTED_FILES[0],finalpath);
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
	}// --
}// --
