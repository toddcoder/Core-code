using System;

namespace Core.Threading;

public class CompletedArgs : EventArgs
{
   public CompletedArgs(TimeSpan elapsed)
   {
      Elapsed = elapsed;
   }

   public TimeSpan Elapsed { get; }
}