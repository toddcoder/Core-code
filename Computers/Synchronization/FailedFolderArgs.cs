using System;

namespace Core.Computers.Synchronization;

public class FailedFolderArgs : FolderArgs
{
   public FailedFolderArgs(FolderName folder, Exception exception) : base(folder)
   {
      Exception = exception;
   }

   public Exception Exception { get; }

   public override string Message => Exception.Message;
}