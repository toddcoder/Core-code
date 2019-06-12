using System;

namespace Standard.Types.Monads
{
   public class FullStackException : ApplicationException
   {
      public FullStackException(Exception exception) : base(exception.Message)
      {
         FullStackTrace = exception.StackTrace + Environment.StackTrace;
	      InnerException = exception.InnerException;
      }

      public FullStackException(string message) : base(message) => FullStackTrace = Environment.StackTrace;

      public FullStackException(string message, Exception innerException) : base(message, innerException)
      {
         FullStackTrace = Environment.StackTrace;
	      InnerException = innerException;
      }

      public string FullStackTrace { get; }

		public new Exception InnerException { get; }
   }
}