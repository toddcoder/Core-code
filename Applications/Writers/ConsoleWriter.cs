﻿using System;

namespace Core.Applications.Writers
{
	public class ConsoleWriter : BaseWriter
	{
		ConsoleColor originalForeground;
		ConsoleColor originalBackground;

		public ConsoleWriter()
		{
			originalForeground = Console.ForegroundColor;
			originalBackground = Console.BackgroundColor;
			setStandardColors();
		}

		public ConsoleColor ForegroundColor { get; set; }

		public ConsoleColor BackgroundColor { get; set; }

		static void setConsoleColors(ConsoleColor foregroundColor, ConsoleColor backgroundColor)
		{
			Console.ForegroundColor = foregroundColor;
			Console.BackgroundColor = backgroundColor;
		}

		void setStandardColors() => setConsoleColors(originalForeground, originalBackground);

		static void setErrorColors() => setConsoleColors(ConsoleColor.Red, ConsoleColor.White);

		protected override void writeRaw(string text) => Console.Write(text);

		public override void WriteException(Exception exception)
		{
			setErrorColors();
			base.WriteException(exception);
			setStandardColors();
		}

		public override void WriteExceptionLine(Exception exception)
		{
			setErrorColors();
			base.WriteExceptionLine(exception);
			setStandardColors();
		}
	}
}