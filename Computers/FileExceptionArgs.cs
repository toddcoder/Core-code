using System;

namespace Core.Computers;

public class FileExceptionArgs : EventArgs
{
   public FileExceptionArgs(FileName file, Exception exception)
   {
      File = file;
      Exception = exception;
   }

   public FileName File { get; }

   public Exception Exception { get; }
}