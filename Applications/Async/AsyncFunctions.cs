using System;
using System.Threading;
using System.Threading.Tasks;

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
   }
}