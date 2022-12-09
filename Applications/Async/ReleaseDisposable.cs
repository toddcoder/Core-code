using System;
using System.Threading;

namespace Core.Applications.Async;

public class ReleaseDisposable : IDisposable
{
   protected SemaphoreSlim semaphore;
   protected bool isDisposed;

   public ReleaseDisposable(SemaphoreSlim semaphore)
   {
      this.semaphore = semaphore;
      isDisposed = false;
   }

   public void Dispose()
   {
      if (!isDisposed)
      {
         semaphore.Dispose();
         isDisposed = true;
      }
   }
}