using System;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public static class AttemptFunctions
   {
      public static IResult<T> tryTo<T>(Func<T> func)
      {
         try
         {
            return func().Success();
         }
         catch (Exception exception)
         {
            return failure<T>(exception);
         }
      }

      public static ICompletion<T> tryToComplete<T>(Func<T> func)
      {
         try
         {
            return func().Completed();
         }
         catch (Exception exception)
         {
            return interrupted<T>(exception);
         }
      }

      public static ICompletion<T> tryToComplete<T>(Func<IResult<T>> func) => func().Completion();

      public static IResult<T> tryTo<T>(Func<IResult<T>> func)
      {
         try
         {
            return func();
         }
         catch (Exception exception)
         {
            return failure<T>(exception);
         }
      }

      public static ICompletion<T> tryToComplete<T>(Func<ICompletion<T>> func)
      {
         try
         {
            return func();
         }
         catch (Exception exception)
         {
            return interrupted<T>(exception);
         }
      }

      public static IResult<Unit> tryTo(Action action)
      {
         try
         {
            action();
            return Unit.Success();
         }
         catch (Exception exception)
         {
            return failure<Unit>(exception);
         }
      }

      public static IResult<Unit> attempt(Action<int> action, int attempts)
      {
         var result = "Action to try hasn't been executed".Failure<Unit>();

         for (var attempt = 0; attempt < attempts; attempt++)
         {
            result = tryTo(() => action(attempt));
            if (result.IsSuccessful)
            {
               return result;
            }
         }

         return result;
      }
   }
}