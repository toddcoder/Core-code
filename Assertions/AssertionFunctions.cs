using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Core.Applications.Messaging;
using Core.Collections;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Applications.Async.AsyncFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Assertions;

public static class AssertionFunctions
{
   private static readonly AutoHash<string, string> nameCache;
   private static readonly Hash<string, object> valueCache;

   static AssertionFunctions()
   {
      static string getName(string expressionText)
      {
         var name = expressionText;
         var _name = name.Matches("'value(' .+ ').' /(.+?) ')'* $; f").Map(r => r.FirstGroup);
         if (_name)
         {
            name = _name;
         }

         return name;
      }

      nameCache = new AutoHash<string, string>(getName, true);
      valueCache = new Hash<string, object>();
   }

   public static TException getException<TException>(params object[] args) where TException : Exception
   {
      return (TException)Activator.CreateInstance(typeof(TException), args);
   }

   public static string enumerableImage<T>(IEnumerable<T> enumerable, int limit = 10)
   {
      var list = enumerable.ToList();
      if (list.Count > limit)
      {
         list = list.Take(limit).ToList();
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
      return hash.AnyHash().Map(dictionaryImage) | hash.ToString;
   }

   public static string maybeImage<T>(Maybe<T> maybe)
   {
      return maybe.Map(v => v.ToNonNullString()) | (() => $"none<{typeof(T).Name}>");
   }

   public static string resultImage<T>(Result<T> result)
   {
      return result.Map(v => v.ToNonNullString()).Recover(e => $"failure<{typeof(T).Name}>({e.Message})");
   }

   public static string optionalImage<T>(Optional<T> optional)
   {
      if (optional is (true, var optionalValue))
      {
         return optionalValue.ToNonNullString();
      }
      else if (optional.AnyException)
      {
         return $"failed<{typeof(T).Name}>({optional.Exception.Message})";
      }
      else
      {
         return $"empty<{typeof(T).Name}>";
      }
   }

   public static string completionImage<T>(Completion<T> completion)
   {
      if (completion is (true, var completionValue))
      {
         return completionValue.ToNonNullString();
      }
      else if (completion.AnyException)
      {
         return $"interrupted<{typeof(T).Name}>({completion.Exception.Message})";
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
      var _constraint = assertion.Constraints.FirstOrNone(c => !c.IsTrue());
      if (_constraint is (true, var constraint))
      {
         throw fail(constraint.Message);
      }
   }

   public static void orThrow<T>(IAssertion<T> assertion, string message)
   {
      if (assertion.Constraints.Any(c => !c.IsTrue()))
      {
         throw fail(message);
      }
   }

   public static void orThrow<T>(IAssertion<T> assertion, Func<string> messageFunc)
   {
      if (assertion.Constraints.Any(c => !c.IsTrue()))
      {
         throw fail(messageFunc());
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

   public static TResult forceConvert<T, TException, TResult>(IAssertion<T> assertion, params object[] args) where TException : Exception
   {
      orThrow<TException, T>(assertion, args);
      return convert<T, TResult>(assertion);
   }

   public static Result<T> orFailure<T>(IAssertion<T> assertion)
   {
      return assertion.Constraints.FirstOrNone(c => !c.IsTrue()).Map(c => c.Message.Failure<T>()) | (() => assertion.Value);
   }

   public static Result<T> orFailure<T>(IAssertion<T> assertion, string message)
   {
      return assertion.Constraints.Any(c => !c.IsTrue()) ? message.Failure<T>() : assertion.Value;
   }

   public static Result<T> orFailure<T>(IAssertion<T> assertion, Func<string> messageFunc)
   {
      return assertion.Constraints.Any(c => !c.IsTrue()) ? messageFunc().Failure<T>() : assertion.Value;
   }

   public static Maybe<T> orNone<T>(IAssertion<T> assertion)
   {
      return maybe(assertion.Constraints.All(c => c.IsTrue()), () => assertion.Value);
   }

   public static async Task<Completion<T>> orFailureAsync<T>(IAssertion<T> assertion, CancellationToken token)
   {
      return await runAsync(t =>
         assertion.Constraints
            .FirstOrNone(c => !c.IsTrue())
            .Map(c => c.Message.Interrupted<T>()) | (() => assertion.Value.Completed(t)), token);
   }

   public static async Task<Completion<T>> orFailureAsync<T>(IAssertion<T> assertion, string message, CancellationToken token)
   {
      return await runAsync(t => assertion.Constraints.Any(c => !c.IsTrue()) ? message.Interrupted<T>() : assertion.Value.Completed(t), token);
   }

   public static async Task<Completion<T>> orFailureAsync<T>(IAssertion<T> assertion, Func<string> messageFunc, CancellationToken token)
   {
      return await runAsync(t => assertion.Constraints.Any(c => !c.IsTrue()) ? messageFunc().Interrupted<T>() : assertion.Value.Completed(t),
         token);
   }

   public static Optional<T> orEmpty<T>(IAssertion<T> assertion)
   {
      return optional<T>() & assertion.Constraints.All(c => c.IsTrue()) & assertion.Value;
   }

   public static Optional<T> orFailed<T>(IAssertion<T> assertion)
   {
      var _failedConstraint = assertion.Constraints.FirstOrNone(c => !c.IsTrue());
      if (_failedConstraint is (true, var failedConstraint))
      {
         return fail(failedConstraint.Message);
      }
      else
      {
         return assertion.Value;
      }
   }

   public static Optional<T> orFailed<T>(IAssertion<T> assertion, string message)
   {
      var _failedConstraint = assertion.Constraints.FirstOrNone(c => !c.IsTrue());
      if (_failedConstraint)
      {
         return fail(message);
      }
      else
      {
         return assertion.Value;
      }
   }

   public static Optional<T> orFailed<T>(IAssertion<T> assertion, Func<string> messageFunc)
   {
      var _failedConstraint = assertion.Constraints.FirstOrNone(c => !c.IsTrue());
      if (_failedConstraint)
      {
         return fail(messageFunc());
      }
      else
      {
         return assertion.Value;
      }
   }

   public static bool orReturn<T>(IAssertion<T> assertion) => !assertion.BeEquivalentToTrue();

   [Obsolete("Use Named extension")]
   public static Expression<Func<T>> assert<T>(Expression<Func<T>> func) => func;

   public static Expression<Func<object>> asObject(Expression<Func<object>> func) => func;

   public static (string name, T value) resolve<T>(Expression<Func<T>> expression)
   {
      var expressionBody = expression.Body;
      var key = expressionBody.ToString();

      var name = nameCache[key];

      var _obj = valueCache.Maybe[key];
      if (_obj is (true, var obj))
      {
         return (name, (T)obj);
      }

      var value = expression.Compile()();
      if (name.IsEmpty())
      {
         name = value.ToNonNullString();
      }

      valueCache[key] = value;

      return (name, value);
   }
}