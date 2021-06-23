using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.Async
{
   public static class AsyncFunctions
   {
      public static async Task<IDisposable> asyncLock(CancellationToken token)
      {
         var semaphore = new SemaphoreSlim(1, 1);

         await semaphore.WaitAsync(token).ConfigureAwait(false);
         return new ReleaseDisposable(semaphore);
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

      public static async Task<ICompletion<T>> runAsync<T>(Func<CancellationToken, T> func, CancellationTokenSource source)
      {
         return await runAsync(func, source.Token);
      }

      public static async Task<ICompletion<T>> runAsync<T>(Func<CancellationToken, T> func, CancellationToken token)
      {
         try
         {
            return await Task.Run(() => func(token).Completed(token), token);
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

      public static async Task<ICompletion<T>> runWithSourceAsync<T>(Func<CancellationToken, ICompletion<T>> func)
      {
         using (var source = new CancellationTokenSource())
         {
            try
            {
               return await Task.Run(() => func(source.Token), source.Token);
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

      public static async Task<ICompletion<T>> withWithSourceAsync<T>(Func<CancellationToken, Task<ICompletion<T>>> func)
      {
         using (var source = new CancellationTokenSource())
         {
            try
            {
               return await Task.Run(() => func(source.Token), source.Token);
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

      public static async Task<ICompletion<Unit>> runAsync(Action<CancellationToken> action, CancellationTokenSource source)
      {
         try
         {
            await Task.Run(() => action(source.Token));
            return Unit.Completed(source.Token);
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

      public static async Task<ICompletion<T>> runFromResultAsync<T>(Func<Result<T>> func)
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

      public static async Task<ICompletion<T>> runFromResultAsync<T>(Func<CancellationToken, Result<T>> func, CancellationTokenSource source)
      {
         return await runFromResultAsync(func, source.Token);
      }

      public static async Task<ICompletion<T>> runFromResultAsync<T>(Func<CancellationToken, Result<T>> func, CancellationToken token)
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

      public static Task<TResult> taskFromFunction<TResult>(Func<TResult> func, CancellationToken token)
      {
         var taskCompletionSource = new TaskCompletionSource<TResult>();
         token.Register(() => taskCompletionSource.TrySetCanceled());

         ThreadPool.QueueUserWorkItem(_ =>
         {
            try
            {
               var result = func();
               taskCompletionSource.SetResult(result);
            }
            catch (Exception exception)
            {
               taskCompletionSource.SetException(exception);
            }
         });

         return taskCompletionSource.Task;
      }

      public static Task taskFromAction(Action action, CancellationToken token)
      {
         var taskCompletionSource = new TaskCompletionSource<Unit>();
         token.Register(() => taskCompletionSource.TrySetCanceled());

         ThreadPool.QueueUserWorkItem(_ =>
         {
            try
            {
               action();
               taskCompletionSource.SetResult(Unit.Value);
            }
            catch (Exception exception)
            {
               taskCompletionSource.SetException(exception);
            }
         });

         return taskCompletionSource.Task;
      }
   }
}