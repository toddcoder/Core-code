﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using Core.Monads;
using Microsoft.Win32.SafeHandles;
using static Core.Monads.AttemptFunctions;

namespace Core.Applications
{
	public static class Kernel32
	{
		[DllImport("kernel32.dll")]
		static extern IntPtr GetConsoleWindow();

		public static IntPtr consoleWindow() => GetConsoleWindow();

		[DllImport("kernel32.dll")]
		static extern bool AttachConsole(int processID);

		public static bool consoleAttach(int processID = -1) => AttachConsole(processID);

		[DllImport("kernel32.dll")]
		static extern bool AllocConsole();

		public static bool consoleAllocate() => AllocConsole();

		const uint GENERIC_WRITE = 0x40000000;
		const uint GENERIC_READ = 0x80000000;
		const uint FILE_SHARE_READ = 0x00000001;
		const uint FILE_SHARE_WRITE = 0x00000002;
		const uint OPEN_EXISTING = 0x00000003;
		const uint FILE_ATTRIBUTE_NORMAL = 0x80;

		[DllImport("kernel32.dll")]
		static extern IntPtr CreateFileW(string fileName, uint desiredAccess, uint shareMode, IntPtr securityAttributes,
			uint creationDisposition, uint flagsAndAttributes, IntPtr templateFile);

		static IResult<FileStream> fileStream(string fileName, uint fileAccessMode, uint fileShareMode, FileAccess fileAccess) => tryTo(
			() =>
			{
				var handle = CreateFileW(fileName, fileAccessMode, fileShareMode, IntPtr.Zero, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL,
					IntPtr.Zero);
				var file = new SafeFileHandle(handle, true);
				if (!file.IsInvalid)
					return new FileStream(file, fileAccess).Success();
				else
					return "Invalid file handle".Failure<FileStream>();
			});

		internal static IResult<Unit> initializeOutStream() =>
			from fs in fileStream("CONOUT$", GENERIC_WRITE, FILE_SHARE_WRITE, FileAccess.Write)
			from writer in getTextWriter(fs)
			select Unit.Value;

		internal static IResult<Unit> initializeInStream() =>
			from fs in fileStream("CONIN$", GENERIC_READ, FILE_SHARE_READ, FileAccess.Read)
			from reader in getTextReader(fs)
			select Unit.Value;

		static IResult<TextWriter> getTextWriter(FileStream fileStream) => tryTo(() =>
		{
			var writer = new StreamWriter(fileStream) { AutoFlush = true };
			Console.SetOut(writer);
			Console.SetError(writer);

			return writer.Success<TextWriter>();
		});

		static IResult<TextReader> getTextReader(FileStream fileStream) => tryTo(() =>
		{
			var reader = new StreamReader(fileStream);
			Console.SetIn(reader);

			return reader.Success<TextReader>();
		});
	}
}