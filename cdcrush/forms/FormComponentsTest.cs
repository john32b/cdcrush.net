using cdcrush.lib;
using cdcrush.lib.app;
using System;
using System.Windows.Forms;

namespace cdcrush.forms
{

/**
 * Test form to test various functionalities
 * ----
 * IN DEVELOPMENT
 * ------------------------------------------*/

	public partial class FormComponentsTest : Form
	{
		public FormComponentsTest()
		{
			InitializeComponent();
		}// --

		private void FormComponentsTest_Load(object sender, EventArgs e)
		{
			LOG.log("Components Form Load");
			LOG.attachTextBox(textBox1);
			FormTools.fileLoadDialogPrepare("ecm", "ECM files (*.ecm)|*.ecm");
			FormTools.fileLoadDialogPrepare("arc", "ARC files (*.arc)|*.arc");
		
		}

		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			base.OnFormClosed(e);
			//LOG.detachTextBox();
		}

		// UNECM - UNECM
		// --
		private void button4_Click(object sender, EventArgs e)
		{
			var app = new EcmTools();
			app.onComplete = (s) =>
			{
				if(!s)
				{
					LOG.log(app.ERROR);
				}else{
					LOG.log("ECM COMPLETE");
				}

				FormTools.invoke(this, () => {
					this.Enabled = true;
				});
			};

			string file = FormTools.fileLoadDialog("ecm")[0];

			if(file!=null)
			{
				app.unecm(file+"1");
				this.Enabled = false;
			}

		}


		// UN ARC
		private void button6_Click(object sender, EventArgs e)
		{
			var app = new FreeArc();
			app.onComplete = (s) => {
				if(!s)
				{
					LOG.log(app.ERROR);
				}else{
					LOG.log("ARC COMPLETE");
				}
				FormTools.invoke(this, () => {
					this.Enabled = true;
				});
			};

			string file = FormTools.fileLoadDialog("arc")[0];
			if(file!=null)
			{
				app.extractAll(file);
				this.Enabled = false;
			}

		}

		string[] FILES_TO_JOIN;
		/// <summary>
		/// JOIN FILES
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button7_Click(object sender, EventArgs e)
		{
			if(FILES_TO_JOIN==null)
			{
				LOG.log("No files to join");
			}
		}// --

		/// <summary>
		/// SELECT FILES TO JOIN
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void join_files_Click(object sender, EventArgs e)
		{
			FILES_TO_JOIN = FormTools.fileLoadDialog("all","",true);
			join_files.Text = FILES_TO_JOIN.ToString();
			LOG.log(FILES_TO_JOIN);
		}//--


	}// --

}// --
