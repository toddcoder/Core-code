using System;
using Core.Monads;

namespace Core.Exceptions
{
   public static class ExceptionExtensions
   {
      public static ApplicationException Throws(this string message) => new FullStackException(message);

      public static ApplicationException Throws(this string message, Exception innerException)
      {
         return new FullStackException(message, innerException);
      }

      public static TException Throws<TException>(object[] parameters) where TException : Exception
      {
         return (TException)Activator.CreateInstance(typeof(TException), parameters);
      }

      public static TException Throws<TException>() where TException : Exception, new()
      {
         return Activator.CreateInstance<TException>();
      }

      public static ApplicationException Fail(this string message) => new FullStackException(message);

      public static ApplicationException Fail(this string message, Exception innerException)
      {
         return new FullStackException(message, innerException);
      }

      public static TException Fail<TException>(object[] parameters) where TException : Exception
      {
         return (TException)Activator.CreateInstance(typeof(TException), parameters);
      }

      public static TException Fail<TException>() where TException : Exception, new()
      {
         return Activator.CreateInstance<TException>();
      }
   }
}