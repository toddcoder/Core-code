using System;

namespace Core.Data.Sqlite;

public class DatabaseEventArgs : EventArgs
{
   public DatabaseEventArgs(string message)
   {
      Message = message;
   }

   public string Message { get; }
}