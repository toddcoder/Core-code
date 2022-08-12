using System;

namespace Core.Data.Sqlite;

public interface IDatabaseRequestNotification
{
   void Message(string message);

   void Success(string message);

   void Failure(string message);

   void Exception(Exception exception);
}