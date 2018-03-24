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
		}// -----------------------------------------

	}// --
}// --
