using System;
using System.IO;
using System.Runtime.InteropServices;
using Core.Monads;
using Microsoft.Win32.SafeHandles;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Applications;

public static class Kernel32
{
   [DllImport("kernel32.dll")]
   private static extern IntPtr GetConsoleWindow();

   public static IntPtr consoleWindow() => GetConsoleWindow();

   [DllImport("kernel32.dll")]
   private static extern bool AttachConsole(int processID);

   public static bool consoleAttach(int processID = -1) => AttachConsole(processID);

   [DllImport("kernel32.dll")]
   private static extern bool AllocConsole();

   public static bool consoleAllocate() => AllocConsole();

   private const uint GENERIC_WRITE = 0x40000000;
   private const uint GENERIC_READ = 0x80000000;
   private const uint FILE_SHARE_READ = 0x00000001;
   private const uint FILE_SHARE_WRITE = 0x00000002;
   private const uint OPEN_EXISTING = 0x00000003;
   private const uint FILE_ATTRIBUTE_NORMAL = 0x80;

   [DllImport("kernel32.dll")]
   private static extern IntPtr CreateFileW(string fileName, uint desiredAccess, uint shareMode, IntPtr securityAttributes,
      uint creationDisposition, uint flagsAndAttributes, IntPtr templateFile);

   private static Result<FileStream> fileStream(string fileName, uint fileAccessMode, uint fileShareMode, FileAccess fileAccess) => tryTo(() =>
   {
      var handle = CreateFileW(fileName, fileAccessMode, fileShareMode, IntPtr.Zero, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);
      var file = new SafeFileHandle(handle, true);
      if (!file.IsInvalid)
      {
         return new FileStream(file, fileAccess).Success();
      }
      else
      {
         return "Invalid file handle".Failure<FileStream>();
      }
   });

   internal static Result<Unit> initializeOutStream() =>
      from fs in fileStream("CONOUT$", GENERIC_WRITE, FILE_SHARE_WRITE, FileAccess.Write)
      from writer in getTextWriter(fs)
      select unit;

   internal static Result<Unit> initializeInStream() =>
      from fs in fileStream("CONIN$", GENERIC_READ, FILE_SHARE_READ, FileAccess.Read)
      from reader in getTextReader(fs)
      select unit;

   private static Result<TextWriter> getTextWriter(FileStream fileStream) => tryTo(() =>
   {
      var writer = new StreamWriter(fileStream) { AutoFlush = true };
      Console.SetOut(writer);
      Console.SetError(writer);

      return writer.Success<TextWriter>();
   });

   private static Result<TextReader> getTextReader(FileStream fileStream) => tryTo(() =>
   {
      var reader = new StreamReader(fileStream);
      Console.SetIn(reader);

      return reader.Success<TextReader>();
   });
}