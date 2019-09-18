using System;

namespace Core.Assertions
{
   public static class AssertionFunctions
   {
      public static TException getException<TException>(params object[] args) where TException : Exception
      {
         return (TException)Activator.CreateInstance(typeof(TException), args);
      }
   }
}