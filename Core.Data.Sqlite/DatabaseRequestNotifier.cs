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
      CurrentNotification.IfThen(n => n.Message(message));
      if (CurrentNotification)
      {
         ((IDatabaseRequestNotification)CurrentNotification).Message(message);
      }
   }

   public static void ShowSuccess(string message) => CurrentNotification.IfThen(n => n.Success(message));

   public static void ShowFailure(string message) => CurrentNotification.IfThen(n => n.Failure(message));

   public static void ShowException(Exception exception) => CurrentNotification.IfThen(n => n.Exception(exception));
}