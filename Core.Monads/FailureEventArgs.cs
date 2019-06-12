using System;

namespace Standard.Types.Monads
{
   public class FailureEventArgs : EventArgs
   {
      public FailureEventArgs(Exception exception) => Exception = exception;

      public Exception Exception { get; }
   }
}