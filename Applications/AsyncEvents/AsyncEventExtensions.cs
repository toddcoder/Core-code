using System;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Applications.AsyncEvents
{
   public static class AsyncEventExtensions
   {
      public static async Task InvokeAsync<TArgs>(this AsyncEventHandler<TArgs> eventHandler, object sender, TArgs args) where TArgs : EventArgs
      {
         var localHandler = eventHandler;
         if (localHandler != null)
         {
            await Task.WhenAll(localHandler.GetInvocationList().Select(del => ((AsyncEventHandler<TArgs>)del).Invoke(sender, args)));
         }
      }
   }
}