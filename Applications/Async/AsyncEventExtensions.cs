using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Applications.Async;

public static class AsyncEventExtensions
{
   public static async Task InvokeAsync<TArgs>(this AsyncEventHandler<TArgs> eventHandler, object sender, TArgs args) where TArgs : EventArgs
   {
      if (eventHandler != null)
      {
         await Task.WhenAll(eventHandler.GetInvocationList().Select(del => ((AsyncEventHandler<TArgs>)del).Invoke(sender, args)));
      }
   }

   public static async Task InvokeAsync<TArgs>(this AsyncEventHandler<TArgs> eventHandler, object sender, TArgs args, CancellationToken token)
      where TArgs : EventArgs
   {
      if (!token.IsCancellationRequested)
      {
         await eventHandler.InvokeAsync(sender, args);
      }
   }
}