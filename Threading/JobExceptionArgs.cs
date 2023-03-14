using System;

namespace Core.Threading;

public class JobExceptionArgs : EventArgs
{
   public JobExceptionArgs(int affinity, Exception exception)
   {
      Affinity = affinity;
      Exception = exception;
   }

   public int Affinity { get; }

   public Exception Exception { get; }
}