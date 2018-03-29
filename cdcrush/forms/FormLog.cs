using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cdcrush.forms
{
	public partial class FormLog: Form
	{

		public static bool ISOPEN = false;

		public FormLog()
		{
			InitializeComponent();
		}// -------------------

		private void FormLog_Load(object sender, EventArgs e)
		{
			lib.LOG.attachTextBox(textBox,true);
			ISOPEN = true;
		}// -------------------

		private void FormLog_FormClosing(object sender, FormClosingEventArgs e)
		{
			lib.LOG.detachTextBox();
			ISOPEN = false;
		}// -------------------

	}//-- 
}// --
