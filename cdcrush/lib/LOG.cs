/**
 * LOG.cs
 * Author: John Dimi 2017
 * ----------------------------------------------
 * Provide basic and simple tracing and logging
 */

using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace cdcrush.lib {


/**
 * WARNING: This class is a mess
 *
 * - Simple global logging
 * - Log History
 * - Can attach a textbox
 * 
 * FUTURE:
 *		- Log to Files
 *		- Log to HTTP
 */
public class LOG
{
	// If you want to use a textbox listener
	static private TextBoxTraceListener textBoxTrace = null;
	
	// Indentation string written at the start of every line
	static private string indentSTR = "";

	// Keep a history of the log
	static private StringBuilder history;

	// How large the log buffer will be
	const int HISTORY_MAX_CAP = 524288; // Half a meg for logging.
	// -----------------------------------------

	// - Constructor
	static LOG()
	{
		history = new StringBuilder(32, HISTORY_MAX_CAP);
	}// -----------------------------------------

	/// <summary>
	/// Set a textbox to mirror the debug output. 
	/// ALSO it copies the entire log history so far.
	/// </summary>
	/// <param name="box"></param>
	public static void attachTextBox(TextBoxBase box,bool copyHistory = false)
	{
		// Just point to a new box
		detachTextBox(); // Just in case
		textBoxTrace = new TextBoxTraceListener(box);
		Trace.Listeners.Add(textBoxTrace);

		// --
		if(copyHistory){
			box.AppendText(history.ToString());
		}
	}// -----------------------------------------

	// --
	// Call this on the destructor of forms you attach a box
	public static void detachTextBox()
	{
		if(textBoxTrace!=null)
		{
			Trace.Listeners.Remove(textBoxTrace);
			textBoxTrace.Dispose();
			textBoxTrace = null;
		}
	}// -----------------------------------------

	/// <summary>
	/// Write to the history and Trace
	/// </summary>
	/// <param name="str"></param>
	static void writeMain(string line)
	{
		try{
			history.AppendLine(line);
		}
		catch(System.ArgumentOutOfRangeException)
		{
			history.Remove(0,history.Length/2); // Just delete half of it from the beginning
			history.AppendLine(line);
		}
		Trace.WriteLine(line);
	}// -----------------------------------------

	/// <summary>
	/// Write a horizontal line
	/// </summary>
	/// <param name="size"></param>
	//[Conditional("DEBUG")]
	public static void line(int size = 40)
	{
		writeMain(new string('-',size));
	}// -----------------------------------------
	// --
	public static void log(string msg)
	{
		writeMain(indentSTR + msg);
	}// -----------------------------------------
	// --
	public static void log(object value)
	{
		writeMain(indentSTR + value.ToString());
	}// -----------------------------------------
	// --
	public static void log(string format,params object[]args)
	{
		writeMain(string.Format(format,args));
	}// -----------------------------------------

	/// <summary>
	/// Put a fake indent on the loggings
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
		if(textBoxTrace!=null) {
			textBoxTrace.Dispose();
		}

		Trace.Listeners.Clear();
	}// -----------------------------------------

}// -- end class


// --
// Listen to Traces and copy to a textbox
class TextBoxTraceListener : TraceListener
{
	internal TextBoxBase box;
	System.Action<string> writeFunction;
	public TextBoxTraceListener(TextBoxBase b)
	{
		box = b;
		Name = "TextBoxLog";
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
		} else {
			writeFunction(message);
		}
	}// -----------------------------------------
	public override void WriteLine(string message)
	{
		Write(message + System.Environment.NewLine);
	}// -----------------------------------------
}// -- end TextBoxStreamWriter

}// -- end namespace