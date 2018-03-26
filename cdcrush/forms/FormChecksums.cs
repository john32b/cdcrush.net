using System;
using System.Windows.Forms;
using cdcrush.lib;

namespace cdcrush.forms
{

	// In development
	public partial class FormChecksums: Form
	{
		// --
		CueReader cd;

		// --
		public FormChecksums(CueReader _cd)
		{
			cd = _cd;
			InitializeComponent();
		}// -----------------------------------------
		
		// --
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if(cd==null) return; // Safeguard, It's not supposed to happen

			Text = "CD Info - " + cd.CD_TITLE;
			textbox.Text = prog.CDCRUSH.PROGRAM_NAME + " v" + prog.CDCRUSH.PROGRAM_VERSION;
			textbox.Text+= " - Detailed CD Info" + Environment.NewLine;
			textbox.Text+= "---------------------------------" + Environment.NewLine;
			textbox.Text+= cd.getDetailedInfo();
		}// -----------------------------------------

		// --
		private void btn_close_Click(object sender, EventArgs e)
		{
			Close();
		}// -----------------------------------------
	}// --
}// --
