using System;

namespace Core.Computers.Synchronization;

public class FailedFileArgs : FileArgs
{
   public FailedFileArgs(FileName sourceFile, FileName targetFile, Exception exception) : base(sourceFile, targetFile)
   {
      Exception = exception;
   }

   public Exception Exception { get; }

   public override string Message => Exception.Message;
}