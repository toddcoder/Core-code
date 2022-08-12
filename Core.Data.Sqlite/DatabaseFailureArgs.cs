using System;

namespace Core.Data.Sqlite;

public class DatabaseFailureArgs : EventArgs
{
   public DatabaseFailureArgs(string message)
   {
      Message = message;
   }

   public string Message { get; }
}