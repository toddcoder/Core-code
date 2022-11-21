using System;

namespace Core.Computers.Synchronization;

public class FolderArgs : EventArgs
{
   public FolderArgs(FolderName folder, string message)
   {
      Folder = folder;
      Message = message;
   }

   protected FolderArgs(FolderName folder)
   {
      Folder = folder;
   }

   public FolderName Folder { get; }

   public virtual string Message { get; }
}