using System;
using static Core.Monads.MonadFunctions;

namespace Core.Monads;

public static class AttemptFunctions
{
   public static Result<T> tryTo<T>(Func<T> func)
   {
      try
      {
         return func();
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Completion<T> tryToComplete<T>(Func<T> func)
   {
      try
      {
         return func();
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Completion<T> tryToComplete<T>(Func<Result<T>> func) => func().Completion();

   public static Result<T> tryTo<T>(Func<Result<T>> func)
   {
      try
      {
         return func();
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Completion<T> tryToComplete<T>(Func<Completion<T>> func)
   {
      try
      {
         return func();
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<Unit> tryTo(Action action)
   {
      try
      {
         action();
         return unit;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<Unit> attempt(Action<int> action, int attempts)
   {
      Result<Unit> result = fail("Action to try hasn't been executed");

      for (var attempt = 0; attempt < attempts; attempt++)
      {
         result = tryTo(() => action(attempt));
         if (result)
         {
            return result;
         }
      }

      return result;
   }
}