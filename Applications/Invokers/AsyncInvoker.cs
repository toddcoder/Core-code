using System;

namespace Core.Applications.Invokers
{
   public abstract class AsyncInvoker
   {
      public abstract void Begin();

      public abstract void End(IAsyncResult result);

      public IAsyncResult Invoke()
      {
         Action action = Begin;
         return action.BeginInvoke(End, null);
      }
   }
}