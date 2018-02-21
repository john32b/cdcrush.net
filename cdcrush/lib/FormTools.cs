using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cdcrush.lib
{

/**
 * Various general use functions
 */
class FormTools
{

	public static string bytesToMB(int bytes)
	{
		return Math.Ceiling( (double) bytes / (1024*1024) ).ToString();
	}// -----------------------------------------




	/// <summary>
	/// Tries to put an image to a picturebox
	/// </summary>
	/// <param name="path"></param>
	/// <returns>TRUE if success, FALSE if couldn't do it</returns>
	public static bool imageSetFile(PictureBox box, string file)
	{
		if (file == null) return false;

		try{
			Image image;
			using (var bmpTemp = new Bitmap(file)) {
				image = new Bitmap(bmpTemp);
			}
			box.Image = image;
			box.SizeMode = PictureBoxSizeMode.Zoom;
			return true;
		}
		catch(System.IO.IOException) {return false;}
		catch(NotSupportedException) {return false;}
		catch(ArgumentException) {return false;}

	}// -----------------------------------------

	// ----------------------------------------------------
	// DRAG DROP
	// ----------------------------------------------------

	/// <summary>
	/// Quickly add drag drop functionality to a form
	/// </summary>
	/// <param name="F">The FORM to apply drag drop to</param>
	/// <param name="onDropFileManager">Callback with list of files</param>
	public static void dragDropFormEnable(System.Windows.Forms.Form F,Action<string[]> onDropFileManager)
	{
		F.AllowDrop = true;

		F.DragEnter += new System.Windows.Forms.DragEventHandler(
			(object sender, System.Windows.Forms.DragEventArgs e) =>
			{
				if (e.Data.GetDataPresent(System.Windows.Forms.DataFormats.FileDrop)) 
					e.Effect = System.Windows.Forms.DragDropEffects.Copy;
			});

		F.DragDrop += new System.Windows.Forms.DragEventHandler(
			(object sender, System.Windows.Forms.DragEventArgs e) =>
			{
				string[] files = (string[])e.Data.GetData(System.Windows.Forms.DataFormats.FileDrop);
				onDropFileManager(files);
			});

	}// -----------------------------------------
	

	// ----------------------------------------------------
	// FILE & FOLDER DIALOGS
	// ----------------------------------------------------

	// General use file open dialog
	static OpenFileDialog fileOpenD;
	// General use file save dialog
	//static SaveFileDialog fileSaveDialog;
	// Associate ID with file filters, so you don't have to pass filters all the time
	static Dictionary<string, string> fileDialogDict;

	/// <summary>
	/// Prepare the Open File Dialog and add a filter to the filter pool for quick retrieval
	/// 
	/// </summary>
	/// <param name="id"></param>
	/// <param name="filter">"ARC files (*.arc)|*.arc|All files (*.*)|*.*" Check inside for more </param>
	public static void fileLoadDialogPrepare(string id = "",string filter = "")
	{
		if (id == "") id = "all";
		if (filter=="") filter = "All files (*.*)|*.*";

		// Filter Examples::
		// "ARC files (*.arc)|*.arc|All files (*.*)|*.*"

		// "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*"

		if(fileDialogDict==null)
		{
			fileDialogDict = new Dictionary<string,string>();
		}
		
		if(!fileDialogDict.ContainsKey(id))
		{
			fileDialogDict.Add(id, filter);
		}

		if(fileOpenD==null)
		{
			fileOpenD = new OpenFileDialog();
			fileOpenD.AutoUpgradeEnabled = true;
			fileOpenD.AddExtension = true;
			fileOpenD.DereferenceLinks = true;
			fileOpenD.RestoreDirectory = true;	// Remember the last directory used in the current session
		}

		LOG.log(" - FileLoadDialog init, {0},{1}", id, filter);
	}// -----------------------------------------

	/// <summary>
	/// Currently just ONE file
	/// </summary>
	/// <param name="id">An ID previously set with prepareDialogFileLoad()</param>
	/// <param name="dir"></param>
	/// <param name="multi"></param>
	public static string[] fileLoadDialog(string id="all",string dir=null,bool multi=false)
	{
		#if DEBUG
		if (fileOpenD == null) {
			LOG.log("ERROR: you must call prepareDialogFileLoad() to init");
			return null;
		}
		#endif

		if(dir!=null) fileOpenD.InitialDirectory = dir;
		fileOpenD.Filter = fileDialogDict[id];
		fileOpenD.Multiselect = multi;
		fileOpenD.CheckFileExists = true;
		fileOpenD.CheckPathExists = true;

		var res = fileOpenD.ShowDialog();

		if(res==DialogResult.OK)
		{
			if (multi)
				return new[] { fileOpenD.FileName };
			else
				return fileOpenD.FileNames;
		}

		return null;
	}// -----------------------------------------



	/// <summary>
	/// Invoke call a function, useful when calling from other threads
	/// </summary>
	/// <param name="ctrl">Any valid control</param>
	/// <param name="act"></param>
	public static void invoke(Control ctrl, MethodInvoker act)
	{
		ctrl.BeginInvoke((MethodInvoker) act);
	}// -----------------------------------------

}// -- end class

}// -- end namespace
