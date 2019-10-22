using System;
using System.Threading;
using System.Threading.Tasks;
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

      public static IResult<T> assert<T>(bool test, Func<T> ifTrue, string messageIfFalse)
      {
         try
         {
            return test ? ifTrue().Success() : messageIfFalse.Failure<T>();
         }
         catch (Exception exception)
         {
            return failure<T>(exception);
         }
      }

      public static IResult<T> assert<T>(Func<bool> test, Func<T> ifTrue, string messageIfFalse)
      {
         try
         {
            return test() ? ifTrue().Success() : messageIfFalse.Failure<T>();
         }
         catch (Exception exception)
         {
            return failure<T>(exception);
         }
      }

      public static IResult<T> assert<T>(bool test, Func<T> ifTrue, Func<string> messageIfFalse)
      {
         try
         {
            return test ? ifTrue().Success() : messageIfFalse().Failure<T>();
         }
         catch (Exception exception)
         {
            return failure<T>(exception);
         }
      }

      public static IResult<T> assert<T>(Func<bool> test, Func<T> ifTrue, Func<string> messageIfFalse)
      {
         try
         {
            return test() ? ifTrue().Success() : messageIfFalse().Failure<T>();
         }
         catch (Exception exception)
         {
            return failure<T>(exception);
         }
      }

      public static IResult<T> assert<T>(bool test, Func<IResult<T>> ifTrue, string messageIfFalse)
      {
         try
         {
            return test ? ifTrue() : messageIfFalse.Failure<T>();
         }
         catch (Exception exception)
         {
            return failure<T>(exception);
         }
      }

      public static IResult<T> assert<T>(Func<bool> test, Func<IResult<T>> ifTrue, string messageIfFalse)
      {
         try
         {
            return test() ? ifTrue() : messageIfFalse.Failure<T>();
         }
         catch (Exception exception)
         {
            return failure<T>(exception);
         }
      }

      public static IResult<T> assert<T>(bool test, Func<IResult<T>> ifTrue, Func<string> messageIfFalse)
      {
         try
         {
            return test ? ifTrue() : messageIfFalse().Failure<T>();
         }
         catch (Exception exception)
         {
            return failure<T>(exception);
         }
      }

      public static IResult<T> assert<T>(Func<bool> test, Func<IResult<T>> ifTrue, Func<string> messageIfFalse)
      {
         try
         {
            return test() ? ifTrue() : messageIfFalse().Failure<T>();
         }
         catch (Exception exception)
         {
            return failure<T>(exception);
         }
      }

      public static IResult<T> assertNotNull<T>(T obj, Func<string> messageIfNull)
      {
         try
         {
            if (obj != null)
            {
               return obj.Success();
            }
            else
            {
               return messageIfNull().Failure<T>();
            }
         }
         catch (Exception exception)
         {
            return failure<T>(exception);
         }
      }

      public static IResult<T> assertNotNull<T>(T obj, string messageIfNull)
      {
         try
         {
            if (obj != null)
            {
               return obj.Success();
            }
            else
            {
               return messageIfNull.Failure<T>();
            }
         }
         catch (Exception exception)
         {
            return failure<T>(exception);
         }
      }

      public static IResult<T> reject<T>(bool test, Func<T> ifFalse, string messageIfTrue)
      {
         return assert(!test, ifFalse, messageIfTrue);
      }

      public static IResult<T> reject<T>(Func<bool> test, Func<T> ifFalse, string messageIfTrue)
      {
         return tryTo(() => assert(!test(), ifFalse, messageIfTrue));
      }

      public static IResult<T> reject<T>(bool test, Func<T> ifFalse, Func<string> messageIfTrue)
      {
         return assert(!test, ifFalse, messageIfTrue);
      }

      public static IResult<T> reject<T>(Func<bool> test, Func<T> ifFalse, Func<string> messageIfTrue)
      {
         return tryTo(() => assert(!test(), ifFalse, messageIfTrue));
      }

      public static IResult<T> reject<T>(bool test, Func<IResult<T>> ifFalse, string messageIfTrue)
      {
         return assert(!test, ifFalse, messageIfTrue);
      }

      public static IResult<T> reject<T>(Func<bool> test, Func<IResult<T>> ifFalse, string messageIfTrue)
      {
         return tryTo(() => assert(!test(), ifFalse, messageIfTrue));
      }

      public static IResult<T> reject<T>(bool test, Func<IResult<T>> ifFalse, Func<string> messageIfTrue)
      {
         return assert(!test, ifFalse, messageIfTrue);
      }

      public static IResult<T> reject<T>(Func<bool> test, Func<IResult<T>> ifFalse, Func<string> messageIfTrue)
      {
         return tryTo(() => assert(!test(), ifFalse, messageIfTrue));
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

      public static async Task<ICompletion<T>> runAsync<T>(Func<CancellationToken, ICompletion<T>> func, CancellationTokenSource source)
      {
         return await runAsync(func, source.Token);
      }

      public static async Task<ICompletion<T>> runAsync<T>(Func<CancellationToken, ICompletion<T>> func, CancellationToken token)
      {
         try
         {
            return await Task.Run(() => func(token), token);
         }
         catch (OperationCanceledException)
         {
            return cancelled<T>();
         }
         catch (Exception exception)
         {
            return interrupted<T>(exception);
         }
      }

      public static async Task<ICompletion<Unit>> runAsync(Action action, CancellationToken token)
      {
         try
         {
            await Task.Run(action, token);
            return Unit.Completed(token);
         }
         catch (OperationCanceledException)
         {
            return cancelled<Unit>();
         }
         catch (Exception exception)
         {
            return interrupted<Unit>(exception);
         }
      }

      public static async Task<ICompletion<T>> runAsync<T>(Func<Task<ICompletion<T>>> func)
      {
         try
         {
            return await func();
         }
         catch (Exception exception)
         {
            return interrupted<T>(exception);
         }
      }

      public static async Task<ICompletion<T>> runFromResultAsync<T>(Func<IResult<T>> func)
      {
         try
         {
            return await Task.Run(() => func().Completion());
         }
         catch (OperationCanceledException)
         {
            return cancelled<T>();
         }
         catch (Exception exception)
         {
            return interrupted<T>(exception);
         }
      }

      public static async Task<ICompletion<T>> runFromResultAsync<T>(Func<CancellationToken, IResult<T>> func, CancellationTokenSource source)
      {
         return await runFromResultAsync(func, source.Token);
      }

      public static async Task<ICompletion<T>> runFromResultAsync<T>(Func<CancellationToken, IResult<T>> func, CancellationToken token)
      {
         try
         {
            return await Task.Run(() => func(token).Completion(), token);
         }
         catch (OperationCanceledException)
         {
            return cancelled<T>();
         }
         catch (Exception exception)
         {
            return interrupted<T>(exception);
         }
      }

      public static async Task<ICompletion<T>> runInterrupted<T>(Exception exception) => await Task.Run(() => interrupted<T>(exception));

      public static async Task<ICompletion<T>> runInterrupted<T>(string message) => await Task.Run(message.Interrupted<T>);

      public static async Task<ICompletion<T>> runCancelled<T>() => await Task.Run(cancelled<T>);

      public static async Task<ICompletion<T>> assertAsync<T>(bool test, Func<T> ifTrue, string messageIfFalse, CancellationToken token)
      {
         try
         {
            if (test)
            {
               return await runAsync(t => ifTrue().Completed(t), token);
            }
            else
            {
               return messageIfFalse.Interrupted<T>();
            }
         }
         catch (OperationCanceledException)
         {
            return cancelled<T>();
         }
         catch (Exception exception)
         {
            return interrupted<T>(exception);
         }
      }

      public static async Task<ICompletion<T>> assertAsync<T>(Func<bool> test, Func<T> ifTrue, string messageIfFalse, CancellationToken token)
      {
         try
         {
            if (test())
            {
               return await runAsync(t => ifTrue().Completed(t), token);
            }
            else
            {
               return messageIfFalse.Interrupted<T>();
            }
         }
         catch (OperationCanceledException)
         {
            return cancelled<T>();
         }
         catch (Exception exception)
         {
            return interrupted<T>(exception);
         }
      }

      public static async Task<ICompletion<T>> assertAsync<T>(bool test, Func<T> ifTrue, Func<string> messageIfFalse, CancellationToken token)
      {
         try
         {
            if (test)
            {
               return await runAsync(t => ifTrue().Completed(t), token);
            }
            else
            {
               return messageIfFalse().Interrupted<T>();
            }
         }
         catch (OperationCanceledException)
         {
            return cancelled<T>();
         }
         catch (Exception exception)
         {
            return interrupted<T>(exception);
         }
      }

      public static async Task<ICompletion<T>> assertAsync<T>(Func<bool> test, Func<T> ifTrue, Func<string> messageIfFalse, CancellationToken token)
      {
         try
         {
            if (test())
            {
               return await runAsync(t => ifTrue().Completed(t), token);
            }
            else
            {
               return messageIfFalse().Interrupted<T>();
            }
         }
         catch (OperationCanceledException)
         {
            return cancelled<T>();
         }
         catch (Exception exception)
         {
            return interrupted<T>(exception);
         }
      }

      public static async Task<ICompletion<T>> assertAsync<T>(bool test, Func<ICompletion<T>> ifTrue, string messageIfFalse, CancellationToken token)
      {
         try
         {
            if (test)
            {
               return await runAsync(t => ifTrue(), token);
            }
            else
            {
               return messageIfFalse.Interrupted<T>();
            }
         }
         catch (OperationCanceledException)
         {
            return cancelled<T>();
         }
         catch (Exception exception)
         {
            return interrupted<T>(exception);
         }
      }

      public static async Task<ICompletion<T>> assertAsync<T>(bool test, Func<ICompletion<T>> ifTrue, Func<string> messageIfFalse, CancellationToken token)
      {
         try
         {
            if (test)
            {
               return await runAsync(t => ifTrue(), token);
            }
            else
            {
               return messageIfFalse().Interrupted<T>();
            }
         }
         catch (OperationCanceledException)
         {
            return cancelled<T>();
         }
         catch (Exception exception)
         {
            return interrupted<T>(exception);
         }
      }

      public static async Task<ICompletion<T>> assertAsync<T>(Func<bool> test, Func<ICompletion<T>> ifTrue, Func<string> messageIfFalse, CancellationToken token)
      {
         try
         {
            if (test())
            {
               return await runAsync(t => ifTrue(), token);
            }
            else
            {
               return messageIfFalse().Interrupted<T>();
            }
         }
         catch (OperationCanceledException)
         {
            return cancelled<T>();
         }
         catch (Exception exception)
         {
            return interrupted<T>(exception);
         }
      }
   }
}