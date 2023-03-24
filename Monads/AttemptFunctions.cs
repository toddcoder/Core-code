using System;
using static Core.Monads.MonadFunctions;

namespace Core.Monads;

public static class AttemptFunctions
{
   public static Optional<T> tryTo<T>(Func<T> func)
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

   public static Completion<T> tryToComplete<T>(Func<Optional<T>> func) => func().Completion();

   public static Optional<T> tryTo<T>(Func<Optional<T>> func)
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

   public static Optional<Unit> tryTo(Action action)
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

   public static Optional<Unit> attempt(Action<int> action, int attempts)
   {
      Optional<Unit> result = fail("Action to try hasn't been executed");

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