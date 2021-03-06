﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Core.Collections;
using Core.Enumerables;
using Core.Exceptions;
using Core.Monads;
using Core.RegularExpressions;
using Core.Strings;
using static Core.Applications.Async.AsyncFunctions;
using static Core.Monads.MonadFunctions;

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

         return list.ToString(", ");
      }

      public static string dictionaryImage<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
      {
         var keys = dictionary.Count > 10 ? dictionary.Keys.Take(10) : dictionary.Keys;
         return keys.Select(k => $"[{k}] = {dictionary[k]}").ToString(", ");
      }

      public static string hashImage<TKey, TValue>(IHash<TKey, TValue> hash)
      {
         return hash.AnyHash().If(out var h) ? dictionaryImage(h) : hash.ToString();
      }

      public static string maybeImage<T>(IMaybe<T> maybe)
      {
         return maybe.Map(v => v.ToNonNullString()).DefaultTo(() => $"none<{typeof(T).Name}>");
      }

      public static string resultImage<T>(IResult<T> result)
      {
         return result.Map(v => v.ToNonNullString()).Recover(e => $"failure<{typeof(T).Name}>({e.Message})");
      }

      public static string matchedImage<T>(IMatched<T> matched)
      {
         if (matched.If(out var value, out var anyException))
         {
            return value.ToNonNullString();
         }
         else if (anyException.If(out var exception))
         {
            return $"failedMatch<{typeof(T).Name}>({exception.Message})";
         }
         else
         {
            return $"notMatched<{typeof(T).Name}>";
         }
      }

      public static string completionImage<T>(ICompletion<T> completion)
      {
         if (completion.If(out var value, out var anyException))
         {
            return value.ToNonNullString();
         }
         else if (anyException.If(out var exception))
         {
            return $"interrupted<{typeof(T).Name}>({exception.Message})";
         }
         else
         {
            return $"cancelled<{typeof(T).Name}>";
         }
      }

      public static bool and(ICanBeTrue x, ICanBeTrue y) => x.BeEquivalentToTrue() && y.BeEquivalentToTrue();

      public static bool or(ICanBeTrue x, ICanBeTrue y) => x.BeEquivalentToTrue() || y.BeEquivalentToTrue();

      public static bool beEquivalentToTrue<T>(IAssertion<T> assertion) => assertion.Constraints.All(c => c.IsTrue());

      public static void orThrow<T>(IAssertion<T> assertion)
      {
         if (assertion.Constraints.FirstOrNone(c => !c.IsTrue()).If(out var constraint))
         {
            throw constraint.Message.Throws();
         }
      }

      public static void orThrow<T>(IAssertion<T> assertion, string message)
      {
         if (assertion.Constraints.Any(c => !c.IsTrue()))
         {
            throw message.Throws();
         }
      }

      public static void orThrow<T>(IAssertion<T> assertion, Func<string> messageFunc)
      {
         if (assertion.Constraints.Any(c => !c.IsTrue()))
         {
            throw messageFunc().Throws();
         }
      }

      public static void orThrow<TException, T>(IAssertion<T> assertion, params object[] args) where TException : Exception
      {
         if (assertion.Constraints.Any(c => !c.IsTrue()))
         {
            throw getException<TException>(args);
         }
      }

      public static T force<T>(IAssertion<T> assertion)
      {
         orThrow(assertion);
         return assertion.Value;
      }

      public static T force<T>(IAssertion<T> assertion, string message)
      {
         orThrow(assertion, message);
         return assertion.Value;
      }

      public static T force<T>(IAssertion<T> assertion, Func<string> messageFunc)
      {
         orThrow(assertion, messageFunc);
         return assertion.Value;
      }

      public static T force<TException, T>(IAssertion<T> assertion, params object[] args) where TException : Exception
      {
         orThrow<TException, T>(assertion, args);
         return assertion.Value;
      }

      private static TResult convert<T, TResult>(IAssertion<T> assertion)
      {
         var converter = TypeDescriptor.GetConverter(typeof(T));
         return (TResult)converter.ConvertTo(assertion.Value, typeof(TResult));
      }

      public static TResult forceConvert<T, TResult>(IAssertion<T> assertion)
      {
         orThrow(assertion);
         return convert<T, TResult>(assertion);
      }

      public static TResult forceConvert<T, TResult>(IAssertion<T> assertion, string message)
      {
         orThrow(assertion, message);
         return convert<T, TResult>(assertion);
      }

      public static TResult forceConvert<T, TResult>(IAssertion<T> assertion, Func<string> messageFunc)
      {
         orThrow(assertion, messageFunc);
         return convert<T, TResult>(assertion);
      }

      public static TResult forceConvert<T, TException, TResult>(IAssertion<T> assertion, params object[] args)
         where TException : Exception
      {
         orThrow<TException, T>(assertion, args);
         return convert<T, TResult>(assertion);
      }

      public static IResult<T> orFailure<T>(IAssertion<T> assertion)
      {
         return assertion.Constraints.FirstOrNone(c => !c.IsTrue()).Map(c => c.Message.Failure<T>())
            .DefaultTo(() => assertion.Value.Success());
      }

      public static IResult<T> orFailure<T>(IAssertion<T> assertion, string message)
      {
         return assertion.Constraints.Any(c => !c.IsTrue()) ? message.Failure<T>() : assertion.Value.Success();
      }

      public static IResult<T> orFailure<T>(IAssertion<T> assertion, Func<string> messageFunc)
      {
         return assertion.Constraints.Any(c => !c.IsTrue()) ? messageFunc().Failure<T>() : assertion.Value.Success();
      }

      public static IMaybe<T> orNone<T>(IAssertion<T> assertion)
      {
         return maybe(assertion.Constraints.All(c => c.IsTrue()), () => assertion.Value);
      }

      public static async Task<ICompletion<T>> orFailureAsync<T>(IAssertion<T> assertion, CancellationToken token) => await runAsync(
         t =>
         {
            return assertion.Constraints
               .FirstOrNone(c => !c.IsTrue())
               .Map(c => c.Message.Interrupted<T>())
               .DefaultTo(() => assertion.Value.Completed(t));
         }, token);

      public static async Task<ICompletion<T>> orFailureAsync<T>(IAssertion<T> assertion, string message, CancellationToken token) =>
         await runAsync(
            t => { return assertion.Constraints.Any(c => !c.IsTrue()) ? message.Interrupted<T>() : assertion.Value.Completed(t); },
            token);

      public static async Task<ICompletion<T>> orFailureAsync<T>(IAssertion<T> assertion, Func<string> messageFunc,
         CancellationToken token)
      {
         return await runAsync(
            t =>
            {
               return assertion.Constraints.Any(c => !c.IsTrue()) ? messageFunc().Interrupted<T>() : assertion.Value.Completed(t);
            }, token);
      }

      public static bool orReturn<T>(IAssertion<T> assertion) => !assertion.BeEquivalentToTrue();

      public static Expression<Func<T>> assert<T>(Expression<Func<T>> func) => func;

      public static Expression<Func<object>> asObject(Expression<Func<object>> func) => func;

      public static (string name, T value) resolve<T>(Expression<Func<T>> expression)
      {
         var expressionBody = expression.Body;
         var name = expressionBody.ToString();

         var matcher = new Matcher();
         if (matcher.IsMatch(name, "'value(' .+ ').' /(.+?) ')'* $"))
         {
            name = matcher.FirstGroup;
         }

         var value = expression.Compile()();
         if (name.IsEmpty())
         {
            name = value.ToNonNullString();
         }

         return (name, value);
      }
   }
}