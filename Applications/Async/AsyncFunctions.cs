using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.Async;

public static class AsyncFunctions
{
   public static async Task<IDisposable> asyncLock(CancellationToken token)
   {
      var semaphore = new SemaphoreSlim(1, 1);

      await semaphore.WaitAsync(token).ConfigureAwait(false);
      return new ReleaseDisposable(semaphore);
   }

   public static async Task<Completion<T>> runAsync<T>(Func<CancellationToken, Completion<T>> func, CancellationTokenSource source)
   {
      return await runAsync(func, source.Token);
   }

   public static async Task<Completion<T>> runAsync<T>(Func<CancellationToken, Completion<T>> func, CancellationToken token)
   {
      try
      {
         return await Task.Run(() => func(token), token);
      }
      catch (OperationCanceledException)
      {
         return nil;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static async Task<Completion<T>> runAsync<T>(Func<CancellationToken, T> func, CancellationTokenSource source)
   {
      return await runAsync(func, source.Token);
   }

   public static async Task<Completion<T>> runAsync<T>(Func<CancellationToken, T> func, CancellationToken token)
   {
      try
      {
         return await Task.Run(() => func(token).Completed(token), token);
      }
      catch (OperationCanceledException)
      {
         return nil;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static async Task<Completion<T>> runWithSourceAsync<T>(Func<CancellationToken, Completion<T>> func)
   {
      using var source = new CancellationTokenSource();
      try
      {
         return await Task.Run(() => func(source.Token), source.Token);
      }
      catch (OperationCanceledException)
      {
         return nil;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static async Task<Completion<T>> withWithSourceAsync<T>(Func<CancellationToken, Task<Completion<T>>> func)
   {
      using var source = new CancellationTokenSource();
      try
      {
         return await Task.Run(() => func(source.Token), source.Token);
      }
      catch (OperationCanceledException)
      {
         return nil;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static async Task<Completion<Unit>> runAsync(Action<CancellationToken> action, CancellationTokenSource source)
   {
      try
      {
         await Task.Run(() => action(source.Token));
         return unit.Completed(source.Token);
      }
      catch (OperationCanceledException)
      {
         return nil;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static async Task<Completion<Unit>> runAsync(Action action, CancellationToken token)
   {
      try
      {
         await Task.Run(action, token);
         return unit.Completed(token);
      }
      catch (OperationCanceledException)
      {
         return nil;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static async Task<Completion<T>> runAsync<T>(Func<Task<Completion<T>>> func)
   {
      try
      {
         return await func();
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static async Task<Completion<T>> runFromResultAsync<T>(Func<Result<T>> func)
   {
      try
      {
         return await Task.Run(() => func().Completion());
      }
      catch (OperationCanceledException)
      {
         return nil;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static async Task<Completion<T>> runFromResultAsync<T>(Func<CancellationToken, Result<T>> func, CancellationTokenSource source)
   {
      return await runFromResultAsync(func, source.Token);
   }

   public static async Task<Completion<T>> runFromResultAsync<T>(Func<CancellationToken, Result<T>> func, CancellationToken token)
   {
      try
      {
         return await Task.Run(() => func(token).Completion(), token);
      }
      catch (OperationCanceledException)
      {
         return nil;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static async Task<Completion<T>> runInterrupted<T>(Exception exception) => await Task.Run(() => exception);

   public static async Task<Completion<T>> runInterrupted<T>(string message) => await Task.Run(message.Interrupted<T>);

   public static async Task<Completion<T>> runCancelled<T>() => await Task.Run(() => nil);

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
            taskCompletionSource.SetResult(unit);
         }
         catch (Exception exception)
         {
            taskCompletionSource.SetException(exception);
         }
      });

      return taskCompletionSource.Task;
   }
}