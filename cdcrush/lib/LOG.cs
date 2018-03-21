/**
 * LOG.cs
 * Author: John Dimi 2017
 * ----------------------------------------------
 * Provide basic and simple tracing and logging
 */

using System.Diagnostics;
using System.Windows.Forms;

namespace cdcrush.lib {


/**
 * - Exports to file, Textbox, console
 * - Set up custom listeners
 * - Easy universal Logging
 * - Writes to DEBUG by default
 * 
 * FUTURE:
 *		- Log to Files
 *		- Log to HTTP
 */
public class LOG
{
	// - If you want to use a textbox listener
	static private TextBoxTraceListener textBoxTrace = null;
	
	static private string indentSTR = "";
	// -----------------------------------------

	/// <summary>
	/// Set a textbox to mirror the debug output
	/// </summary>
	/// <param name="box"></param>
	//[Conditional("DEBUG")]
	public static void attachTextBox(TextBox box)
	{
		// Just point to a new box
		detachTextBox(); // Just in case
		textBoxTrace = new TextBoxTraceListener(box);
		Debug.Listeners.Add(textBoxTrace);
	}// -----------------------------------------

	// --
	// Call this on the destructor of forms you attach a box
	public static void detachTextBox()
	{
		if(textBoxTrace!=null)
		{
			LOG.log("[LOG] Detaching Text Box");
			Debug.Listeners.Remove(textBoxTrace);
			textBoxTrace.Dispose();
			textBoxTrace = null;
		}
	}// -----------------------------------------



	/// <summary>
	/// Write a horizontal line
	/// </summary>
	/// <param name="size"></param>
	//[Conditional("DEBUG")]
	public static void line(int size = 40)
	{
		Debug.WriteLine(new string('-', size));
	}// -----------------------------------------

	public static void log(string msg)
	{
		Debug.WriteLine(indentSTR + msg);
	}// -----------------------------------------

	public static void log(object value)
	{
		Debug.WriteLine(indentSTR + value);
	}// -----------------------------------------

	public static void log(string format,params object[]args)
	{
		Debug.WriteLine(indentSTR + format, args);
	}// -----------------------------------------

	/// <summary>
	/// 
	/// </summary>
	/// <param name="dir">-1 for less, 0 to clear 1 for more</param>
	public static void indent(int dir = 0)
	{
		if (dir > 0) indentSTR += "    ";
		else if (dir < 0) indentSTR = indentSTR.Substring(0, indentSTR.Length - 4);
		else indentSTR = "";
	}// -----------------------------------------

	/// <summary>
	/// Free Resources, Stop Logging
	/// </summary>
	public static void kill()
	{
		if(textBoxTrace!=null)
		{
			textBoxTrace.Dispose();
		}

		Debug.Listeners.Clear();
	}// -----------------------------------------

}// -- end class




// ::  Help From
//	; https://stackoverflow.com/questions/1389264/trace-listener-to-write-to-a-text-box-wpf-application
//  ; https://www.codeproject.com/Articles/21009/A-Simple-TextBox-TraceListener
//  ;
class TextBoxTraceListener : TraceListener
{
	internal TextBox box;
	System.Action<string> writeFunction;
	public TextBoxTraceListener(TextBox b)
	{
		box = b;
		Name = "BoxLog";
		writeFunction = (string s) => 
		{
			if(box.IsDisposed) return;
			if( IndentLevel > 0) {
				s = new string(' ', IndentLevel * IndentSize) + s;
			}
			box.AppendText(s);
		};
	}// -----------------------------------------
	public override void Write(string message)
	{
		if (box.InvokeRequired) {
			box.BeginInvoke(writeFunction, message);
		}
		else {
			writeFunction(message);
		}
	}// -----------------------------------------
	public override void WriteLine(string message)
	{
		Write(message + System.Environment.NewLine);
	}// -----------------------------------------
}// -- end TextBoxStreamWriter

}// -- end namespace