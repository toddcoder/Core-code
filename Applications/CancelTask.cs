using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Applications;

public abstract class CancelTask<TState> : IDisposable
{
   protected Task task;
   protected CancellationTokenSource source;
   protected CancellationToken token;
   protected ManualResetEvent reset;

   public Task Task => task;

   public CancellationTokenSource Source => source;

   public CancellationToken Token => token;

   public ManualResetEvent Reset => reset;

   public bool IsCompleted => task.IsCompleted;

   public bool IsFaulted => task.IsFaulted;

   public bool IsCanceled => task.IsCanceled;

   protected CancelTask(TState state)
   {
      source = new CancellationTokenSource();
      token = source.Token;
      reset = new ManualResetEvent(false);
      task = new Task(o =>
      {
         Dispatch((TState)o);
         reset.Set();
      }, state, token);
   }

   public void Dispose()
   {
      dispose();
      GC.SuppressFinalize(this);
   }

   protected void dispose()
   {
      task?.Dispose();
      source?.Dispose();
      reset?.Dispose();
   }

   public void Start() => task.Start();

   public abstract void Dispatch(TState state);

   public void Cancel() => source.Cancel();

   ~CancelTask() => dispose();
}