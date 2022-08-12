using System;

namespace Core.Data.Sqlite;

public class DatabaseExceptionArgs : EventArgs
{
   public DatabaseExceptionArgs(Exception exception)
   {
      Exception = exception;
   }

   public Exception Exception { get; }
}