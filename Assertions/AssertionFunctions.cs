using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Enumerables;
using Core.Exceptions;
using Core.Monads;
using static Core.Monads.AttemptFunctions;

namespace Core.Assertions
{
   public static class AssertionFunctions
   {
      public static TException getException<TException>(params object[] args) where TException : Exception
      {
         return (TException)Activator.CreateInstance(typeof(TException), args);
      }

      public static string enumerableImage<T>(IEnumerable<T> enumerable)
      {
         var list = enumerable.ToList();
         if (list.Count > 10)
         {
            list = list.Take(10).ToList();
         }

         return list.Stringify();
      }

      public static bool and(ICanBeTrue x, ICanBeTrue y) => x.BeTrue() && y.BeTrue();

      public static bool or(ICanBeTrue x, ICanBeTrue y) => x.BeTrue() || y.BeTrue();

      public static bool beTrue<T>(IAssertion<T> assertion) => assertion.Constraints.All(c => c.IsTrue());

      public static void assert<T>(IAssertion<T> assertion)
      {
         if (assertion.Constraints.FirstOrNone(c => !c.IsTrue()).If(out var constraint))
         {
            throw constraint.Message.Throws();
         }
      }

      public static void assert<T>(IAssertion<T> assertion, string message)
      {
         if (assertion.Constraints.Any(c => !c.IsTrue()))
         {
            throw message.Throws();
         }
      }

      public static void assert<T>(IAssertion<T> assertion, Func<string> messageFunc)
      {
         if (assertion.Constraints.Any(c => !c.IsTrue()))
         {
            throw messageFunc().Throws();
         }
      }

      public static void assert<TException, T>(IAssertion<T> assertion, params object[] args) where TException : Exception
      {
         if (assertion.Constraints.Any(c => !c.IsTrue()))
         {
            throw getException<TException>(args);
         }
      }

      public static T ensure<T>(IAssertion<T> assertion)
      {
         assert(assertion);
         return assertion.Value;
      }

      public static T ensure<T>(IAssertion<T> assertion, string message)
      {
         assert(assertion, message);
         return assertion.Value;
      }

      public static T ensure<T>(IAssertion<T> assertion, Func<string> messageFunc)
      {
         assert(assertion, messageFunc);
         return assertion.Value;
      }

      public static T ensure<TException, T>(IAssertion<T> assertion, params object[] args) where TException : Exception
      {
         assert<TException, T>(assertion, args);
         return assertion.Value;
      }

      static TResult convert<T, TResult>(IAssertion<T> assertion)
      {
         var converter = TypeDescriptor.GetConverter(typeof(T));
         return (TResult)converter.ConvertTo(assertion.Value, typeof(TResult));
      }

      public static TResult ensureConvert<T, TResult>(IAssertion<T> assertion)
      {
         assert(assertion);
         return convert<T, TResult>(assertion);
      }

      public static TResult ensureConvert<T, TResult>(IAssertion<T> assertion, string message)
      {
         assert(assertion, message);
         return convert<T, TResult>(assertion);
      }

      public static TResult ensureConvert<T, TResult>(IAssertion<T> assertion, Func<string> messageFunc)
      {
         assert(assertion, messageFunc);
         return convert<T, TResult>(assertion);
      }

      public static TResult ensureConvert<T, TException, TResult>(IAssertion<T> assertion, params object[] args) where TException : Exception
      {
         assert<TException, T>(assertion, args);
         return convert<T, TResult>(assertion);
      }

      public static IResult<T> @try<T>(IAssertion<T> assertion)
      {
         if (assertion.Constraints.FirstOrNone(c => !c.IsTrue()).If(out var constraint))
         {
            return constraint.Message.Failure<T>();
         }
         else
         {
            return assertion.Value.Success();
         }
      }

      public static IResult<T> @try<T>(IAssertion<T> assertion, string message)
      {
         if (assertion.Constraints.Any(c => !c.IsTrue()))
         {
            return message.Failure<T>();
         }
         else
         {
            return assertion.Value.Success();
         }
      }

      public static IResult<T> @try<T>(IAssertion<T> assertion, Func<string> messageFunc)
      {
         if (assertion.Constraints.Any(c => !c.IsTrue()))
         {
            return messageFunc().Failure<T>();
         }
         else
         {
            return assertion.Value.Success();
         }
      }

      public static async Task<ICompletion<T>> tryAsync<T>(IAssertion<T> assertion, CancellationToken token) => await runAsync(t =>
      {
         if (assertion.Constraints.FirstOrNone(c => !c.IsTrue()).If(out var constraint))
         {
            return constraint.Message.Interrupted<T>();
         }
         else
         {
            return assertion.Value.Completed(t);
         }
      }, token);

      public static async Task<ICompletion<T>> tryAsync<T>(IAssertion<T> assertion, string message, CancellationToken token) => await runAsync(t =>
      {
         if (assertion.Constraints.Any(c => !c.IsTrue()))
         {
            return message.Interrupted<T>();
         }
         else
         {
            return assertion.Value.Completed(t);
         }
      }, token);

      public static async Task<ICompletion<T>> tryAsync<T>(IAssertion<T> assertion, Func<string> messageFunc, CancellationToken token)
      {
         return await runAsync(t =>
         {
            if (assertion.Constraints.Any(c => !c.IsTrue()))
            {
               return messageFunc().Interrupted<T>();
            }
            else
            {
               return assertion.Value.Completed(t);
            }
         }, token);
      }
   }
}