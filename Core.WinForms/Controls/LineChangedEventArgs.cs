using System;

namespace Core.WinForms.Controls
{
	public class LineChangedEventArgs : EventArgs
	{
		public LineChangedEventArgs(int lineNumber) => LineNumber = lineNumber;

		public int LineNumber { get; }
	}
}