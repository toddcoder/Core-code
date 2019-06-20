using System;
using Core.Collections;
using Core.Exceptions;

namespace Core.Booleans
{
   public static class Assertions
   {
      public static void Assert(bool test, string message)
      {
         if (!test)
         {
            throw message.Throws();
         }
      }

      public static void Assert(bool test, Func<string> func)
      {
         if (!test)
         {
            throw func().Throws();
         }
      }

      public static void Assert(bool test, string message, Exception innerException)
      {
         if (!test)
         {
            throw message.Throws(innerException);
         }
      }

      public static void Assert<TException>(bool test)
         where TException : Exception, new()
      {
         if (!test)
         {
            throw Throwing.Throws<TException>();
         }
      }

      public static void Assert<TException>(bool test, params object[] parameters)
         where TException : Exception
      {
         if (!test)
         {
            throw Throwing.Throws<TException>(parameters);
         }
      }

      public static void Assert<TKey, TValue>(Hash<TKey, TValue> dictionary, TKey key)
      {
         Assert(dictionary.ContainsKey(key), $"Dictionary doesn't contain key {key}");
      }

      public static void Reject(bool test, string message)
      {
         if (test)
         {
            throw message.Throws();
         }
      }

      public static void Reject(bool test, Func<string> func)
      {
         if (test)
         {
            throw func().Throws();
         }
      }

      public static void Reject(bool test, string message, Exception innerException)
      {
         if (test)
         {
            throw message.Throws(innerException);
         }
      }

      public static void Reject<TException>(bool test)
         where TException : Exception, new()
      {
         if (test)
         {
            throw Throwing.Throws<TException>();
         }
      }

      public static void Reject<TException>(bool test, params object[] parameters)
         where TException : Exception
      {
         if (test)
         {
            throw Throwing.Throws<TException>(parameters);
         }
      }

      public static T Ensure<T>(T value, Func<T, bool> test, string message)
      {
         if (test(value))
         {
            return value;
         }
         else
         {
            throw message.Throws();
         }
      }

      public static T Ensure<T, TException>(T value, Func<T, bool> test)
         where TException : Exception, new()
      {
         if (test(value))
         {
            return value;
         }
         else
         {
            throw new TException();
         }
      }

      public static T Ensure<T, TException>(T value, Func<T, bool> test, params object[] parameters)
         where TException : Exception
      {
         if (test(value))
         {
            return value;
         }
         else
         {
            throw Throwing.Throws<TException>(parameters);
         }
      }
   }
}