using System;
using System.Collections.Generic;
using Core.Monads;

namespace Core.Exceptions;

public static class ExceptionExtensions
{
   [Obsolete("Use fail function")]
   public static ApplicationException Throws(this string message) => new FullStackException(message);

   [Obsolete("Use fail function")]
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

   [Obsolete("Use fail function")]
   public static ApplicationException Fail(this string message) => new FullStackException(message);

   [Obsolete("Use fail function")]
   public static ApplicationException Fail(this string message, Exception innerException)
   {
      return new FullStackException(message, innerException);
   }

   [Obsolete("Use fail function")]
   public static TException Fail<TException>(this object firstParameter, params object[] parameters) where TException : Exception
   {
      var list = new List<object> { firstParameter };
      list.AddRange(parameters);
      return (TException)Activator.CreateInstance(typeof(TException), list.ToArray());
   }

   [Obsolete("Use fail function")]
   public static TException Fail<TException>() where TException : Exception, new()
   {
      return Activator.CreateInstance<TException>();
   }
}