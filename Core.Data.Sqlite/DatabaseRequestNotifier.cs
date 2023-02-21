using System;
using Core.DataStructures;
using Core.Monads;

namespace Core.Data.Sqlite;

public static class DatabaseRequestNotifier
{
   private static MaybeStack<IDatabaseRequestNotification> notifications;

   static DatabaseRequestNotifier()
   {
      notifications = new MaybeStack<IDatabaseRequestNotification>();
   }

   public static void EnterScope(IDatabaseRequestNotification notification) => notifications.Push(notification);

   public static void LeaveScope() => notifications.Pop();

   public static Maybe<IDatabaseRequestNotification> CurrentNotification => notifications.Peek();

   public static void ShowMessage(string message)
   {
      if (CurrentNotification is (true, var currentNotification))
      {
         currentNotification.Message(message);
      }
   }

   public static void ShowSuccess(string message)
   {
      if (CurrentNotification is (true, var currentNotification))
      {
         currentNotification.Success(message);
      }
   }

   public static void ShowFailure(string message)
   {
      if (CurrentNotification is (true, var currentNotification))
      {
         currentNotification.Failure(message);
      }
   }

   public static void ShowException(Exception exception)
   {
      if (CurrentNotification is (true, var currentNotification))
      {
         currentNotification.Exception(exception);
      }
   }
}