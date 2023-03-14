using System;

namespace Core.Monads;

public class FailureEventArgs : EventArgs
{
   public FailureEventArgs(Exception exception) => Exception = exception;

   public Exception Exception { get; }
}