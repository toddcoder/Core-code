using System;

namespace Core.Computers;

public class FileArgs : EventArgs
{
   public FileArgs(FileName sourceFile, FileName targetFile, string message)
   {
      SourceFile = sourceFile;
      TargetFile = targetFile;
      Message = message;
   }

   public FileName SourceFile { get; }

   public FileName TargetFile { get; }

   public string Message { get; }
}