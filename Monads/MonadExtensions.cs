using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Enumerables;
using Core.Matching;
using Core.Objects;
using Core.Strings;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public static class MonadExtensions
   {
      public static Maybe<T> Some<T>(this T obj) => obj is null ? none<T>() : new Some<T>(obj);

      public static bool NotNull<T>(this T obj, out T value) => obj.Some().If(out value);

      public static Maybe<string> SomeIfNotEmpty(this string text) => maybe(text.IsNotEmpty(), text.Some);

      public static Maybe<Type> UnderlyingType(this object obj)
      {
         if (obj is null)
         {
            return none<Type>();
         }
         else
         {
            var type = obj.GetType();
            return type.UnderlyingTypeOf();
         }
      }

      public static Maybe<Type> UnderlyingTypeOf(this Type type)
      {
         if (type.Name.IsMatch("^ ('IMaybe' | 'Some' | 'None') '`1'; f"))
         {
            return type.GetGenericArguments().FirstOrNone();
         }
         else
         {
            return none<Type>();
         }
      }

      [DebuggerStepThrough]
      public static Result<TResult> SelectMany<T, TResult>(this Maybe<T> maybe, Func<T, Result<TResult>> projection)
      {
         return maybe.Map(projection).DefaultTo(() => "Value not provided".Failure<TResult>());
      }

      [DebuggerStepThrough]
      public static Result<TResult> Select<T, TResult>(this Result<T> result, Func<T, TResult> func)
      {
         if (result.If(out var value, out var exception))
         {
            return func(value).Success();
         }
         else
         {
            return failure<TResult>(exception);
         }
      }

      public static Result<T> Success<T>(this T value) => value is null ? "Value cannot be null".Failure<T>() : new Success<T>(value);

      public static Result<T> Failure<T>(this string message) => failure<T>(new Exception(message));

      public static Result<T> Failure<T, TException>(this object firstItem, params object[] args) where TException : Exception
      {
         var list = new List<object> { firstItem };
         list.AddRange(args);

         return failure<T>((TException)typeof(TException).Create(list.ToArray()));
      }

      public static Matched<T> Match<T>(this T matches) => matches is null ? "Matches cannot be null".FailedMatch<T>() : new Match<T>(matches);

      public static Matched<T> MatchUnlessNull<T>(this T obj) => obj is null ? noMatch<T>() : obj.Match();

      public static Matched<T> FailedMatch<T>(this string message) => new FailedMatch<T>(new ApplicationException(message));

      public static Matched<T> FailedMatch<T, TException>(this object firstItem, params object[] args) where TException : Exception
      {
         var list = new List<object> { firstItem };
         list.AddRange(args);

         return failedMatch<T>((TException)typeof(TException).Create(list.ToArray()));
      }

      public static Result<T> Result<T>(this bool test, Func<T> ifFunc, string exceptionMessage)
      {
         if (test)
         {
            return ifFunc().Success();
         }
         else
         {
            return exceptionMessage.Failure<T>();
         }
      }

      public static Result<T> Result<T>(this bool test, Func<T> ifFunc, Func<string> exceptionMessage)
      {
         if (test)
         {
            return ifFunc().Success();
         }
         else
         {
            return exceptionMessage().Failure<T>();
         }
      }

      public static Result<T> Result<T>(this bool test, Func<Result<T>> ifFunc, string exceptionMessage)
      {
         if (test)
         {
            return ifFunc();
         }
         else
         {
            return exceptionMessage.Failure<T>();
         }
      }

      public static Result<T> Result<T>(this bool test, Func<Result<T>> ifFunc,
         Func<string> exceptionMessage)
      {
         if (test)
         {
            return ifFunc();
         }
         else
         {
            return exceptionMessage().Failure<T>();
         }
      }

      public static Matched<T> Matching<T>(this bool test, Func<T> ifFunc)
      {
         try
         {
            if (test)
            {
               return ifFunc().Match();
            }
            else
            {
               return noMatch<T>();
            }
         }
         catch (Exception exception)
         {
            return failedMatch<T>(exception);
         }
      }

      public static IEnumerable<T> WhereIsSome<T>(this IEnumerable<Maybe<T>> enumerable)
      {
         foreach (var maybe in enumerable)
         {
            if (maybe.If(out var value))
            {
               yield return value;
            }
         }
      }

      public static IEnumerable<(T item, TMaybe maybe)> WhereIsSome<T, TMaybe>(this IEnumerable<T> enumerable, Func<T, Maybe<TMaybe>> predicate)
      {
         foreach (var item in enumerable)
         {
            if (predicate(item).If(out var value))
            {
               yield return (item, value);
            }
         }
      }

      public static IEnumerable<T> WhereIsSuccessful<T>(this IEnumerable<Result<T>> enumerable)
      {
         foreach (var result in enumerable)
         {
            if (result.If(out var value))
            {
               yield return value;
            }
         }
      }

      public static IEnumerable<(T item, TResult result)> WhereIsSuccessful<T, TResult>(this IEnumerable<T> enumerable,
         Func<T, Result<TResult>> predicate)
      {
         foreach (var item in enumerable)
         {
            if (predicate(item).If(out var value))
            {
               yield return (item, value);
            }
         }
      }

      public static IEnumerable<T> WhereIsMatched<T>(this IEnumerable<Matched<T>> enumerable)
      {
         foreach (var matched in enumerable)
         {
            if (matched.If(out var value))
            {
               yield return value;
            }
         }
      }

      public static IEnumerable<(T item, TMatched matched)> WhereIsMatched<T, TMatched>(this IEnumerable<T> enumerable,
         Func<T, Matched<TMatched>> predicate)
      {
         foreach (var item in enumerable)
         {
            if (predicate(item).If(out var value))
            {
               yield return (item, value);
            }
         }
      }

      public static IEnumerable<T> WhereIsCompleted<T>(this IEnumerable<Completion<T>> enumerable)
      {
         foreach (var completion in enumerable)
         {
            if (completion.If(out var value))
            {
               yield return value;
            }
         }
      }

      public static IEnumerable<(T item, TCompletion completion)> WhereIsCompleted<T, TCompletion>(this IEnumerable<T> enumerable,
         Func<T, Completion<TCompletion>> predicate)
      {
         foreach (var item in enumerable)
         {
            if (predicate(item).If(out var value))
            {
               yield return (item, value);
            }
         }
      }

      public static Maybe<IEnumerable<T>> AllAreSome<T>(this IEnumerable<Maybe<T>> enumerable)
      {
         var result = new List<T>();
         foreach (var anyValue in enumerable)
         {
            if (anyValue.If(out var value))
            {
               result.Add(value);
            }
            else
            {
               return none<IEnumerable<T>>();
            }
         }

         return result.Some<IEnumerable<T>>();
      }

      public static IEnumerable<Result<T>> All<T>(this IEnumerable<Result<T>> enumerable, Action<T> success = null,
         Action<Exception> failure = null)
      {
         return new ResultIterator<T>(enumerable, success, failure).All();
      }

      public static IEnumerable<T> Successes<T>(this IEnumerable<Result<T>> enumerable, Action<T> success = null,
         Action<Exception> failure = null)
      {
         return new ResultIterator<T>(enumerable, success, failure).SuccessesOnly();
      }

      public static IEnumerable<T> Matches<T>(this IEnumerable<Matched<T>> enumerable, Action<T> matched = null,
         Action notMatched = null, Action<Exception> failure = null)
      {
         return new MatchedIterator<T>(enumerable, matched, notMatched, failure).MatchesOnly();
      }

      public static IEnumerable<Exception> Failures<T>(this IEnumerable<Result<T>> enumerable,
         Action<T> success = null, Action<Exception> failure = null)
      {
         return new ResultIterator<T>(enumerable, success, failure).FailuresOnly();
      }

      public static IEnumerable<Exception> Failures<T>(this IEnumerable<Matched<T>> enumerable, Action<T> matched = null,
         Action notMatched = null, Action<Exception> failure = null)
      {
         return new MatchedIterator<T>(enumerable, matched, notMatched, failure).FailuresOnly();
      }

      public static (IEnumerable<T> enumerable, Maybe<Exception> exception) SuccessesFirst<T>(this IEnumerable<Result<T>> enumerable,
         Action<T> success = null, Action<Exception> failure = null)
      {
         return new ResultIterator<T>(enumerable, success, failure).SuccessesThenFailure();
      }

      public static Result<IEnumerable<T>> IfAllSuccesses<T>(this IEnumerable<Result<T>> enumerable,
         Action<T> success = null, Action<Exception> failure = null)
      {
         return new ResultIterator<T>(enumerable, success, failure).IfAllSuccesses();
      }

      public static Result<TResult> ForAny<TSource, TResult>(this IEnumerable<TSource> enumerable,
         Func<TSource, TResult> func) => tryTo(() =>
      {
         var firstItem = none<TResult>();
         foreach (var result in enumerable.Select(item => tryTo(() => func(item))))
         {
            if (result.ValueOrOriginal(out var value, out var original))
            {
               if (firstItem.IsNone)
               {
                  firstItem = value.Some();
               }
            }
            else
            {
               return original;
            }
         }

         return firstItem.Result("Enumerable empty");
      });

      public static Result<TResult> ForAny<TSource, TResult>(this Result<IEnumerable<TSource>> enumerable,
         Func<TSource, TResult> func)
      {
         return enumerable.Map(e => e.ForAny(func));
      }

      public static Result<TResult> ForAny<TSource, TResult>(this IEnumerable<TSource> enumerable,
         Action<TSource> action, TResult result) => tryTo(() =>
      {
         foreach (var item in enumerable)
         {
            try
            {
               action(item);
            }
            catch (Exception exception)
            {
               return failure<TResult>(exception);
            }
         }

         return result.Success();
      });

      public static Result<TResult> ForAny<TSource, TResult>(this Result<IEnumerable<TSource>> enumerable,
         Action<TSource> action, TResult result)
      {
         return enumerable.Map(e => e.ForAny(action, result));
      }

      public static Result<TResult> ForAny<TSource, TResult>(this IEnumerable<TSource> enumerable,
         Action<TSource> action, Func<TResult> result) => tryTo(() =>
      {
         foreach (var item in enumerable)
         {
            try
            {
               action(item);
            }
            catch (Exception exception)
            {
               return failure<TResult>(exception);
            }
         }

         return result().Success();
      });

      public static Result<TResult> ForAny<TSource, TResult>(this Result<IEnumerable<TSource>> enumerable,
         Action<TSource> action, Func<TResult> result)
      {
         return enumerable.Map(e => e.ForAny(action, result));
      }

      public static Result<T> Flat<T>(this Result<Result<T>> result) => result.Recover(failure<T>);

      public static T ThrowIfFailed<T>(this Result<T> result)
      {
         if (result.If(out var value, out var exception))
         {
            return value;
         }
         else
         {
            throw exception;
         }
      }

      public static void ForEach<T>(this Result<IEnumerable<T>> enumerable, Action<T> ifSuccess,
         Action<Exception> ifFailure)
      {
         if (enumerable.If(out var e, out var exception))
         {
            e.ForEach(ifSuccess, ifFailure);
         }
         else
         {
            ifFailure(exception);
         }
      }

      public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> ifSuccess, Action<Exception> ifFailure)
      {
         using var enumerator = enumerable.GetEnumerator();

         while (true)
         {
            var movedNext = false;
            var value = default(T);
            try
            {
               movedNext = enumerator.MoveNext();
               if (movedNext)
               {
                  value = enumerator.Current;
               }
               else
               {
                  break;
               }
            }
            catch (Exception exception)
            {
               ifFailure(exception);
            }

            try
            {
               if (movedNext)
               {
                  ifSuccess(value);
               }
            }
            catch (Exception exception)
            {
               ifFailure(exception);
            }
         }
      }

      public static Maybe<T> IfCast<T>(this object obj) => obj is T t ? t.Some() : none<T>();

      public static bool If<T1, T2>(this Maybe<(T1, T2)> some, out T1 v1, out T2 v2)
      {
         if (some.If(out var value))
         {
            v1 = value.Item1;
            v2 = value.Item2;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;

            return false;
         }
      }

      public static bool If<T1, T2, T3>(this Maybe<(T1, T2, T3)> some, out T1 v1, out T2 v2, out T3 v3)
      {
         if (some.If(out var value))
         {
            v1 = value.Item1;
            v2 = value.Item2;
            v3 = value.Item3;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;
            v3 = default;

            return false;
         }
      }

      public static bool If<T1, T2, T3, T4>(this Maybe<(T1, T2, T3, T4)> some, out T1 v1, out T2 v2, out T3 v3,
         out T4 v4)
      {
         if (some.If(out var value))
         {
            v1 = value.Item1;
            v2 = value.Item2;
            v3 = value.Item3;
            v4 = value.Item4;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;
            v3 = default;
            v4 = default;

            return false;
         }
      }

      public static bool If<T1, T2>(this Result<(T1, T2)> result, out T1 v1, out T2 v2)
      {
         if (result.If(out var value))
         {
            v1 = value.Item1;
            v2 = value.Item2;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;

            return false;
         }
      }

      public static bool If<T1, T2>(this Result<(T1, T2)> result, out T1 v1, out T2 v2, out Exception exception)
      {
         if (result.If(out var value, out exception))
         {
            v1 = value.Item1;
            v2 = value.Item2;
            exception = default;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;

            return false;
         }
      }

      public static bool If<T1, T2, T3>(this Result<(T1, T2, T3)> result, out T1 v1, out T2 v2, out T3 v3)
      {
         if (result.If(out var value))
         {
            v1 = value.Item1;
            v2 = value.Item2;
            v3 = value.Item3;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;
            v3 = default;

            return false;
         }
      }

      public static bool If<T1, T2, T3>(this Result<(T1, T2, T3)> result, out T1 v1, out T2 v2, out T3 v3, out Exception exception)
      {
         if (result.If(out var value, out exception))
         {
            v1 = value.Item1;
            v2 = value.Item2;
            v3 = value.Item3;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;
            v3 = default;

            return false;
         }
      }

      public static bool If<T1, T2, T3, T4>(this Result<(T1, T2, T3, T4)> result, out T1 v1, out T2 v2, out T3 v3,
         out T4 v4)
      {
         if (result.If(out var value))
         {
            v1 = value.Item1;
            v2 = value.Item2;
            v3 = value.Item3;
            v4 = value.Item4;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;
            v3 = default;
            v4 = default;

            return false;
         }
      }

      public static bool If<T1, T2, T3, T4>(this Result<(T1, T2, T3, T4)> result, out T1 v1, out T2 v2, out T3 v3,
         out T4 v4, out Exception exception)
      {
         if (result.If(out var value, out exception))
         {
            v1 = value.Item1;
            v2 = value.Item2;
            v3 = value.Item3;
            v4 = value.Item4;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;
            v3 = default;
            v4 = default;

            return false;
         }
      }

      public static bool If<T1, T2>(this Matched<(T1, T2)> matched, out T1 v1, out T2 v2)
      {
         if (matched.If(out var value))
         {
            v1 = value.Item1;
            v2 = value.Item2;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;

            return false;
         }
      }

      public static bool If<T1, T2>(this Matched<(T1, T2)> matched, out T1 v1, out T2 v2, out Maybe<Exception> anyException)
      {
         if (matched.If(out var value, out anyException))
         {
            v1 = value.Item1;
            v2 = value.Item2;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;

            return false;
         }
      }

      public static bool If<T1, T2, T3>(this Matched<(T1, T2, T3)> matched, out T1 v1, out T2 v2, out T3 v3)
      {
         if (matched.If(out var value))
         {
            v1 = value.Item1;
            v2 = value.Item2;
            v3 = value.Item3;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;
            v3 = default;

            return false;
         }
      }

      public static bool If<T1, T2, T3>(this Matched<(T1, T2, T3)> matched, out T1 v1, out T2 v2, out T3 v3, out Maybe<Exception> anyException)
      {
         if (matched.If(out var value, out anyException))
         {
            v1 = value.Item1;
            v2 = value.Item2;
            v3 = value.Item3;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;
            v3 = default;

            return false;
         }
      }

      public static bool If<T1, T2, T3, T4>(this Matched<(T1, T2, T3, T4)> matched, out T1 v1, out T2 v2, out T3 v3,
         out T4 v4)
      {
         if (matched.If(out var value))
         {
            v1 = value.Item1;
            v2 = value.Item2;
            v3 = value.Item3;
            v4 = value.Item4;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;
            v3 = default;
            v4 = default;

            return false;
         }
      }

      public static bool If<T1, T2, T3, T4>(this Matched<(T1, T2, T3, T4)> matched, out T1 v1, out T2 v2, out T3 v3,
         out T4 v4, out Maybe<Exception> anyException)
      {
         if (matched.If(out var value, out anyException))
         {
            v1 = value.Item1;
            v2 = value.Item2;
            v3 = value.Item3;
            v4 = value.Item4;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;
            v3 = default;
            v4 = default;

            return false;
         }
      }

      public static bool If<T1, T2>(this Completion<(T1, T2)> completion, out T1 v1, out T2 v2)
      {
         if (completion.If(out var value))
         {
            v1 = value.Item1;
            v2 = value.Item2;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;

            return false;
         }
      }

      public static bool If<T1, T2>(this Completion<(T1, T2)> completion, out T1 v1, out T2 v2, out Maybe<Exception> anyException)
      {
         if (completion.If(out var value, out anyException))
         {
            v1 = value.Item1;
            v2 = value.Item2;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;

            return false;
         }
      }

      public static bool If<T1, T2, T3>(this Completion<(T1, T2, T3)> completion, out T1 v1, out T2 v2, out T3 v3)
      {
         if (completion.If(out var value))
         {
            v1 = value.Item1;
            v2 = value.Item2;
            v3 = value.Item3;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;
            v3 = default;

            return false;
         }
      }

      public static bool If<T1, T2, T3>(this Completion<(T1, T2, T3)> completion, out T1 v1, out T2 v2, out T3 v3,
         out Maybe<Exception> anyException)
      {
         if (completion.If(out var value, out anyException))
         {
            v1 = value.Item1;
            v2 = value.Item2;
            v3 = value.Item3;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;
            v3 = default;

            return false;
         }
      }

      public static bool If<T1, T2, T3, T4>(this Completion<(T1, T2, T3, T4)> completion, out T1 v1, out T2 v2, out T3 v3, out T4 v4)
      {
         if (completion.If(out var value))
         {
            v1 = value.Item1;
            v2 = value.Item2;
            v3 = value.Item3;
            v4 = value.Item4;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;
            v3 = default;
            v4 = default;

            return false;
         }
      }

      public static bool If<T1, T2, T3, T4>(this Completion<(T1, T2, T3, T4)> completion, out T1 v1, out T2 v2, out T3 v3, out T4 v4,
         out Maybe<Exception> anyException)
      {
         if (completion.If(out var value, out anyException))
         {
            v1 = value.Item1;
            v2 = value.Item2;
            v3 = value.Item3;
            v4 = value.Item4;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;
            v3 = default;
            v4 = default;

            return false;
         }
      }

      public static bool ValueOrOriginal<T1, T2>(this Result<(T1, T2)> result, out T1 v1, out T2 v2, out Result<(T1, T2)> original)
      {
         if (result.ValueOrOriginal(out var tuple, out original))
         {
            v1 = tuple.Item1;
            v2 = tuple.Item2;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;

            return false;
         }
      }

      public static bool ValueOrOriginal<T1, T2, T3>(this Result<(T1, T2, T3)> result, out T1 v1, out T2 v2, out T3 v3,
         out Result<(T1, T2, T3)> original)
      {
         if (result.ValueOrOriginal(out var tuple, out original))
         {
            v1 = tuple.Item1;
            v2 = tuple.Item2;
            v3 = tuple.Item3;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;
            v3 = default;

            return false;
         }
      }

      public static bool ValueOrOriginal<T1, T2, T3, T4>(this Result<(T1, T2, T3, T4)> result, out T1 v1, out T2 v2, out T3 v3, out T4 v4,
         out Result<(T1, T2, T3, T4)> original)
      {
         if (result.ValueOrOriginal(out var tuple, out original))
         {
            v1 = tuple.Item1;
            v2 = tuple.Item2;
            v3 = tuple.Item3;
            v4 = tuple.Item4;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;
            v3 = default;
            v4 = default;

            return false;
         }
      }

      public static bool ValueOrOriginal<T1, T2>(this Matched<(T1, T2)> matched, out T1 v1, out T2 v2, out Matched<(T1, T2)> original)
      {
         if (matched.ValueOrOriginal(out var tuple, out original))
         {
            v1 = tuple.Item1;
            v2 = tuple.Item2;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;

            return false;
         }
      }

      public static bool ValueOrOriginal<T1, T2, T3>(this Matched<(T1, T2, T3)> matched, out T1 v1, out T2 v2, out T3 v3,
         out Matched<(T1, T2, T3)> original)
      {
         if (matched.ValueOrOriginal(out var tuple, out original))
         {
            v1 = tuple.Item1;
            v2 = tuple.Item2;
            v3 = tuple.Item3;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;
            v3 = default;

            return false;
         }
      }

      public static bool ValueOrOriginal<T1, T2, T3, T4>(this Matched<(T1, T2, T3, T4)> matched, out T1 v1, out T2 v2, out T3 v3, out T4 v4,
         out Matched<(T1, T2, T3, T4)> original)
      {
         if (matched.ValueOrOriginal(out var tuple, out original))
         {
            v1 = tuple.Item1;
            v2 = tuple.Item2;
            v3 = tuple.Item3;
            v4 = tuple.Item4;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;
            v3 = default;
            v4 = default;

            return false;
         }
      }

      public static bool ValueOrOriginal<T1, T2>(this Completion<(T1, T2)> completion, out T1 v1, out T2 v2, out Completion<(T1, T2)> original)
      {
         if (completion.ValueOrOriginal(out var tuple, out original))
         {
            v1 = tuple.Item1;
            v2 = tuple.Item2;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;

            return false;
         }
      }

      public static bool ValueOrOriginal<T1, T2, T3>(this Completion<(T1, T2, T3)> completion, out T1 v1, out T2 v2, out T3 v3,
         out Completion<(T1, T2, T3)> original)
      {
         if (completion.ValueOrOriginal(out var tuple, out original))
         {
            v1 = tuple.Item1;
            v2 = tuple.Item2;
            v3 = tuple.Item3;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;
            v3 = default;

            return false;
         }
      }

      public static bool ValueOrOriginal<T1, T2, T3, T4>(this Completion<(T1, T2, T3, T4)> completion, out T1 v1, out T2 v2, out T3 v3, out T4 v4,
         out Completion<(T1, T2, T3, T4)> original)
      {
         if (completion.ValueOrOriginal(out var tuple, out original))
         {
            v1 = tuple.Item1;
            v2 = tuple.Item2;
            v3 = tuple.Item3;
            v4 = tuple.Item4;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;
            v3 = default;
            v4 = default;

            return false;
         }
      }

      public static bool ValueOrCast<T1, T2, TResult>(this Result<(T1, T2)> result, out T1 v1, out T2 v2, out Result<TResult> castAs)
      {
         if (result.ValueOrCast(out var tuple, out castAs))
         {
            v1 = tuple.Item1;
            v2 = tuple.Item2;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;

            return false;
         }
      }

      public static bool ValueOrCast<T1, T2, T3, TResult>(this Result<(T1, T2, T3)> result, out T1 v1, out T2 v2, out T3 v3,
         out Result<TResult> castAs)
      {
         if (result.ValueOrCast(out var tuple, out castAs))
         {
            v1 = tuple.Item1;
            v2 = tuple.Item2;
            v3 = tuple.Item3;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;
            v3 = default;

            return false;
         }
      }

      public static bool ValueOrCast<T1, T2, T3, T4, TResult>(this Result<(T1, T2, T3, T4)> result, out T1 v1, out T2 v2, out T3 v3, out T4 v4,
         out Result<TResult> castAs)
      {
         if (result.ValueOrCast(out var tuple, out castAs))
         {
            v1 = tuple.Item1;
            v2 = tuple.Item2;
            v3 = tuple.Item3;
            v4 = tuple.Item4;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;
            v3 = default;
            v4 = tuple.Item4;

            return false;
         }
      }

      public static bool ValueOrCast<T1, T2, TResult>(this Matched<(T1, T2)> matched, out T1 v1, out T2 v2, out Matched<TResult> castAs)
      {
         if (matched.ValueOrCast(out var tuple, out castAs))
         {
            v1 = tuple.Item1;
            v2 = tuple.Item2;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;

            return false;
         }
      }

      public static bool ValueOrCast<T1, T2, T3, TResult>(this Matched<(T1, T2, T3)> matched, out T1 v1, out T2 v2, out T3 v3,
         out Matched<TResult> castAs)
      {
         if (matched.ValueOrCast(out var tuple, out castAs))
         {
            v1 = tuple.Item1;
            v2 = tuple.Item2;
            v3 = tuple.Item3;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;
            v3 = default;

            return false;
         }
      }

      public static bool ValueOrCast<T1, T2, T3, T4, TResult>(this Matched<(T1, T2, T3, T4)> matched, out T1 v1, out T2 v2, out T3 v3, out T4 v4,
         out Matched<TResult> castAs)
      {
         if (matched.ValueOrCast(out var tuple, out castAs))
         {
            v1 = tuple.Item1;
            v2 = tuple.Item2;
            v3 = tuple.Item3;
            v4 = tuple.Item4;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;
            v3 = default;
            v4 = tuple.Item4;

            return false;
         }
      }

      public static bool ValueOrCast<T1, T2, TResult>(this Completion<(T1, T2)> completion, out T1 v1, out T2 v2, out Completion<TResult> castAs)
      {
         if (completion.ValueOrCast(out var tuple, out castAs))
         {
            v1 = tuple.Item1;
            v2 = tuple.Item2;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;

            return false;
         }
      }

      public static bool ValueOrCast<T1, T2, T3, TResult>(this Completion<(T1, T2, T3)> completion, out T1 v1, out T2 v2, out T3 v3,
         out Completion<TResult> castAs)
      {
         if (completion.ValueOrCast(out var tuple, out castAs))
         {
            v1 = tuple.Item1;
            v2 = tuple.Item2;
            v3 = tuple.Item3;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;
            v3 = default;

            return false;
         }
      }

      public static bool ValueOrCast<T1, T2, T3, T4, TResult>(this Completion<(T1, T2, T3, T4)> completion, out T1 v1, out T2 v2, out T3 v3,
         out T4 v4, out Completion<TResult> castAs)
      {
         if (completion.ValueOrCast(out var tuple, out castAs))
         {
            v1 = tuple.Item1;
            v2 = tuple.Item2;
            v3 = tuple.Item3;
            v4 = tuple.Item4;

            return true;
         }
         else
         {
            v1 = default;
            v2 = default;
            v3 = default;
            v4 = tuple.Item4;

            return false;
         }
      }

      public static IEnumerable<T> SomeValue<T>(this IEnumerable<Maybe<T>> enumerable)
      {
         foreach (var source in enumerable)
         {
            if (source.If(out var value))
            {
               yield return value;
            }
         }
      }

      public static IEnumerable<T> SuccessfulValue<T>(this IEnumerable<Result<T>> enumerable)
      {
         foreach (var result in enumerable)
         {
            if (result.If(out var value))
            {
               yield return value;
            }
         }
      }

      public static Completion<T> Completed<T>(this T value) => value is null ? "value cannot be null".Interrupted<T>() : new Completed<T>(value);

      public static Completion<T> Completed<T>(this T value, CancellationToken token)
      {
         if (token.IsCancellationRequested)
         {
            return cancelled<T>();
         }
         else
         {
            return value.Completed();
         }
      }

      public static Completion<T> Interrupted<T>(this string message) => new Interrupted<T>(new ApplicationException(message));

      public static Completion<T> Interrupted<T, TException>(this object firstItem, params object[] args) where TException : Exception
      {
         var list = new List<object> { firstItem };
         list.AddRange(args);

         return interrupted<T>((TException)typeof(TException).Create(list.ToArray()));
      }

      public static Completion<T> Completion<T>(this Result<T> result) => result.Map(v => v.Completed()).Recover(interrupted<T>);

      public static Completion<T> Completion<T>(this Result<T> result, CancellationToken token)
      {
         return result.Map(v => v.Completed(token)).Recover(interrupted<T>);
      }

      public static Completion<T> Completion<T>(this Matched<T> matched)
      {
         return matched.FlatMap(v => v.Completed(), cancelled<T>, interrupted<T>);
      }

      public static Completion<T> Completion<T>(this Matched<T> matched, CancellationToken token)
      {
         return matched.FlatMap(v => v.Completed(token), cancelled<T>, interrupted<T>);
      }

      public static Completion<T> Completion<T>(this Maybe<T> maybe) => maybe.Map(v => v.Completed()).DefaultTo(cancelled<T>);

      public static Completion<T> Completion<T>(this Maybe<T> maybe, CancellationToken token)
      {
         return maybe.Map(v => v.Completed(token)).DefaultTo(cancelled<T>);
      }

      private static Completion<T> cancelledOrInterrupted<T>(Exception exception) => exception switch
      {
         OperationCanceledException => cancelled<T>(),
         ObjectDisposedException => cancelled<T>(),
         FullStackException { InnerException: { } and not FullStackException } fullStackException => cancelledOrInterrupted<T>(fullStackException
            .InnerException),
         _ => interrupted<T>(exception)
      };

      public static async Task<Completion<T3>> SelectMany<T1, T2, T3>(this Task<Completion<T1>> source, Func<T1, Task<Completion<T2>>> func,
         Func<T1, T2, T3> projection)
      {
         var t = await source;
         if (t.If(out var tValue, out var _exception))
         {
            var u = await func(tValue);
            if (u.If(out var uValue, out _exception))
            {
               return projection(tValue, uValue).Completed();
            }
            else if (_exception.If(out var exception))
            {
               return cancelledOrInterrupted<T3>(exception);
            }
            else
            {
               return cancelled<T3>();
            }
         }
         else if (_exception.If(out var exception))
         {
            return cancelledOrInterrupted<T3>(exception);
         }
         else
         {
            return cancelled<T3>();
         }
      }

      public static Result<T> Result<T>(this Completion<T> completion)
      {
         if (completion.If(out var value, out var _exception))
         {
            return value.Success();
         }
         else if (_exception.If(out var exception))
         {
            return failure<T>(exception);
         }
         else
         {
            return "Cancelled".Failure<T>();
         }
      }

      public static Maybe<T> MaxOrNone<T>(this IEnumerable<T> enumerable)
      {
         var array = enumerable.ToArray();
         return maybe(array.Length > 0, () => array.Max());
      }

      public static Maybe<T> MaxOrNone<T, TMax>(this IEnumerable<T> enumerable, Func<T, TMax> maxOnFunc)
      {
         var array = enumerable.ToArray();
         return maybe(array.Length > 0, () =>
         {
            var max = array.Select((v, i) => (v: maxOnFunc(v), i)).Max();
            return array[max.i];
         });
      }

      public static Maybe<T> MinOrNone<T>(this IEnumerable<T> enumerable)
      {
         var array = enumerable.ToArray();
         return maybe(array.Length > 0, () => array.Min());
      }

      public static Maybe<T> MinOrNone<T, TMin>(this IEnumerable<T> enumerable, Func<T, TMin> minOnFunc)
      {
         var array = enumerable.ToArray();
         return maybe(array.Length > 0, () =>
         {
            var min = array.Select((v, i) => (v: minOnFunc(v), i)).Min();
            return array[min.i];
         });
      }

      public static Result<T> MaxOrFail<T>(this IEnumerable<T> enumerable, Func<string> exceptionMessage) => tryTo(() =>
      {
         var array = enumerable.ToArray();
         return assert(array.Length > 0, () => array.Max(), exceptionMessage);
      });

      public static Result<T> MaxOrFail<T, TMax>(this IEnumerable<T> enumerable, Func<T, TMax> maxOnFunc, Func<string> exceptionMessage)
      {
         return tryTo(() =>
         {
            var array = enumerable.ToArray();
            return assert(array.Length > 0, () =>
            {
               var max = array.Select((v, i) => (v: maxOnFunc(v), i)).Max();
               return array[max.i];
            }, exceptionMessage);
         });
      }

      public static Result<T> MinOrFail<T>(this IEnumerable<T> enumerable, Func<string> exceptionMessage) => tryTo(() =>
      {
         var array = enumerable.ToArray();
         return assert(array.Length > 0, () => array.Min(), exceptionMessage);
      });

      public static Result<T> MinOrFail<T, TMin>(this IEnumerable<T> enumerable, Func<T, TMin> minOnFunc, Func<string> exceptionMessage)
      {
         return tryTo(() =>
         {
            var array = enumerable.ToArray();
            return assert(array.Length > 0, () =>
            {
               var min = array.Select((v, i) => (v: minOnFunc(v), i)).Min();
               return array[min.i];
            }, exceptionMessage);
         });
      }

      public static Matched<T> MaxOrNotMatched<T>(this IEnumerable<T> enumerable)
      {
         var array = enumerable.ToArray();
         return isMatched(array.Length > 0, () => array.Max());
      }

      public static Matched<T> maxOrNotMatched<T, TMax>(this IEnumerable<T> enumerable, Func<T, TMax> maxOnFunc)
      {
         var array = enumerable.ToArray();
         return isMatched(array.Length > 0, () =>
         {
            var max = array.Select((v, i) => (v: maxOnFunc(v), i)).Max();
            return array[max.i];
         });
      }

      public static Matched<T> minOrNotMatched<T>(this IEnumerable<T> enumerable)
      {
         var array = enumerable.ToArray();
         return isMatched(array.Length > 0, () => array.Min());
      }

      public static Matched<T> minOrNotMatched<T, TMin>(this IEnumerable<T> enumerable, Func<T, TMin> minOnFunc)
      {
         var array = enumerable.ToArray();
         return isMatched(array.Length > 0, () =>
         {
            var min = array.Select((v, i) => (v: minOnFunc(v), i)).Max();
            return array[min.i];
         });
      }

      public static Maybe<TResult> Map<T1, T2, TResult>(this Maybe<(T1, T2)> maybe, Func<T1, T2, TResult> func)
      {
         return maybe.Map(t => func(t.Item1, t.Item2));
      }

      public static Maybe<TResult> Map<T1, T2, TResult>(this Maybe<(T1, T2)> maybe, Func<T1, T2, Maybe<TResult>> func)
      {
         return maybe.Map(t => func(t.Item1, t.Item2));
      }

      public static Maybe<TResult> Map<T1, T2, T3, TResult>(this Maybe<(T1, T2, T3)> maybe, Func<T1, T2, T3, TResult> func)
      {
         return maybe.Map(t => func(t.Item1, t.Item2, t.Item3));
      }

      public static Maybe<TResult> Map<T1, T2, T3, TResult>(this Maybe<(T1, T2, T3)> maybe, Func<T1, T2, T3, Maybe<TResult>> func)
      {
         return maybe.Map(t => func(t.Item1, t.Item2, t.Item3));
      }

      public static Maybe<TResult> Map<T1, T2, T3, T4, TResult>(this Maybe<(T1, T2, T3, T4)> maybe, Func<T1, T2, T3, T4, TResult> func)
      {
         return maybe.Map(t => func(t.Item1, t.Item2, t.Item3, t.Item4));
      }

      public static Maybe<TResult> Map<T1, T2, T3, T4, TResult>(this Maybe<(T1, T2, T3, T4)> maybe, Func<T1, T2, T3, T4, Maybe<TResult>> func)
      {
         return maybe.Map(t => func(t.Item1, t.Item2, t.Item3, t.Item4));
      }

      public static Result<TResult> Map<T1, T2, TResult>(this Result<(T1, T2)> result, Func<T1, T2, TResult> func)
      {
         return result.Map(t => func(t.Item1, t.Item2));
      }

      public static Result<TResult> Map<T1, T2, TResult>(this Result<(T1, T2)> result, Func<T1, T2, Result<TResult>> func)
      {
         return result.Map(t => func(t.Item1, t.Item2));
      }

      public static Result<TResult> Map<T1, T2, T3, TResult>(this Result<(T1, T2, T3)> result, Func<T1, T2, T3, TResult> func)
      {
         return result.Map(t => func(t.Item1, t.Item2, t.Item3));
      }

      public static Result<TResult> Map<T1, T2, T3, TResult>(this Result<(T1, T2, T3)> result, Func<T1, T2, T3, Result<TResult>> func)
      {
         return result.Map(t => func(t.Item1, t.Item2, t.Item3));
      }

      public static Result<TResult> Map<T1, T2, T3, T4, TResult>(this Result<(T1, T2, T3, T4)> result, Func<T1, T2, T3, T4, TResult> func)
      {
         return result.Map(t => func(t.Item1, t.Item2, t.Item3, t.Item4));
      }

      public static Result<TResult> Map<T1, T2, T3, T4, TResult>(this Result<(T1, T2, T3, T4)> result, Func<T1, T2, T3, T4, Result<TResult>> func)
      {
         return result.Map(t => func(t.Item1, t.Item2, t.Item3, t.Item4));
      }

      public static Matched<TResult> Map<T1, T2, TResult>(this Matched<(T1, T2)> result, Func<T1, T2, TResult> func)
      {
         return result.Map(t => func(t.Item1, t.Item2));
      }

      public static Matched<TResult> Map<T1, T2, TResult>(this Matched<(T1, T2)> result, Func<T1, T2, Matched<TResult>> func)
      {
         return result.Map(t => func(t.Item1, t.Item2));
      }

      public static Matched<TResult> Map<T1, T2, T3, TResult>(this Matched<(T1, T2, T3)> result, Func<T1, T2, T3, TResult> func)
      {
         return result.Map(t => func(t.Item1, t.Item2, t.Item3));
      }

      public static Matched<TResult> Map<T1, T2, T3, TResult>(this Matched<(T1, T2, T3)> result, Func<T1, T2, T3, Matched<TResult>> func)
      {
         return result.Map(t => func(t.Item1, t.Item2, t.Item3));
      }

      public static Matched<TResult> Map<T1, T2, T3, T4, TResult>(this Matched<(T1, T2, T3, T4)> result, Func<T1, T2, T3, T4, TResult> func)
      {
         return result.Map(t => func(t.Item1, t.Item2, t.Item3, t.Item4));
      }

      public static Completion<TResult> Map<T1, T2, TResult>(this Completion<(T1, T2)> completion, Func<T1, T2, TResult> func)
      {
         return completion.Map(t => func(t.Item1, t.Item2));
      }

      public static Completion<TResult> Map<T1, T2, TResult>(this Completion<(T1, T2)> completion, Func<T1, T2, Completion<TResult>> func)
      {
         return completion.Map(t => func(t.Item1, t.Item2));
      }

      public static Completion<TResult> Map<T1, T2, T3, TResult>(this Completion<(T1, T2, T3)> completion, Func<T1, T2, T3, TResult> func)
      {
         return completion.Map(t => func(t.Item1, t.Item2, t.Item3));
      }

      public static Completion<TResult> Map<T1, T2, T3, TResult>(this Completion<(T1, T2, T3)> completion,
         Func<T1, T2, T3, Completion<TResult>> func)
      {
         return completion.Map(t => func(t.Item1, t.Item2, t.Item3));
      }

      public static Completion<TResult> Map<T1, T2, T3, T4, TResult>(this Completion<(T1, T2, T3, T4)> completion,
         Func<T1, T2, T3, T4, TResult> func)
      {
         return completion.Map(t => func(t.Item1, t.Item2, t.Item3, t.Item4));
      }

      public static Completion<TResult> Map<T1, T2, T3, T4, TResult>(this Completion<(T1, T2, T3, T4)> completion,
         Func<T1, T2, T3, T4, Completion<TResult>> func)
      {
         return completion.Map(t => func(t.Item1, t.Item2, t.Item3, t.Item4));
      }

      public static Maybe<T> SomeIf<T>(this Func<bool> boolExpression, Func<T> value)
      {
         return boolExpression() ? value().Some() : none<T>();
      }

      public static Maybe<T> SomeIf<T>(this Func<bool> boolExpression, Func<Maybe<T>> value)
      {
         return boolExpression() ? value() : none<T>();
      }

      public static Maybe<T> SomeIf<T>(this bool boolExpression, Func<T> value)
      {
         return boolExpression ? value().Some() : none<T>();
      }

      public static Maybe<T> SomeIf<T>(this bool boolExpression, Func<Maybe<T>> value)
      {
         return boolExpression ? value() : none<T>();
      }
   }
}