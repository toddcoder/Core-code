using System;

namespace Core.Applications.Invokers;

public class LambdaInvoker : AsyncInvoker
{
   protected Action begin;
   protected Action<IAsyncResult> end;

   public LambdaInvoker(Action begin, Action<IAsyncResult> end)
   {
      this.begin = begin;
      this.end = end;
   }

   public override void Begin() => begin();

   public override void End(IAsyncResult result) => end(result);
}