using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Enumerables;
using Core.Objects;
using Core.RegularExpressions;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public static class MonadExtensions
   {
      [Obsolete("Use Some()")]
      public static IMaybe<T> SomeIf<T>(this T obj, Predicate<T> predicate) => maybe(predicate(obj), () => obj);

      [Obsolete("Use Some()")]
      public static IMaybe<T> SomeIfNotNull<T>(this T obj) => obj.SomeIf(o => !o.IsNull());

      public static IMaybe<T> Some<T>(this T obj) => obj.IsNull() ? none<T>() : new Some<T>(obj);

      public static bool NotNull<T>(this T obj, out T value) => obj.Some().If(out value);

      public static IMaybe<Type> UnderlyingType(this object obj)
      {
         if (obj.IsNull())
         {
            return none<Type>();
         }
         else
         {
            var type = obj.GetType();
            return type.UnderlyingTypeOf();
         }
      }

      public static IMaybe<Type> UnderlyingTypeOf(this Type type)
      {
         if (type.Name.IsMatch("^ ('IMaybe' | 'Some' | 'None') '`1'"))
         {
            return type.GetGenericArguments().FirstOrNone();
         }
         else
         {
            return none<Type>();
         }
      }

      public static IMaybe<TResult> SelectMany<T, TResult>(this IMaybe<T> maybe, Func<T, IMaybe<TResult>> projection)
      {
         return maybe.Map(projection);
      }

      [DebuggerStepThrough]
      public static IResult<TResult> SelectMany<T, TResult>(this IMaybe<T> maybe, Func<T, IResult<TResult>> projection)
      {
         return maybe.Map(projection).DefaultTo(() => "Value not provided".Failure<TResult>());
      }

      public static IMaybe<T3> SelectMany<T1, T2, T3>(this IMaybe<T1> first, Func<T1, IMaybe<T2>> func,
         Func<T1, T2, T3> projection)
      {
         return first.Map(outer => func(outer).Map(inner => projection(outer, inner)));
      }

      public static IMaybe<TResult> Select<T, TResult>(this IMaybe<T> maybe, Func<T, TResult> func) => maybe.Map(func);

      [DebuggerStepThrough]
      public static IResult<TResult> Select<T, TResult>(this IResult<T> result, Func<T, TResult> func)
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

      public static bool Assign<T>(this IMaybe<T> maybe, out T value)
      {
         value = maybe.DefaultTo(() => default);
         return maybe.IsSome;
      }

      public static IResult<T> Success<T>(this T value) => value.IsNull() ? "Value cannot be null".Failure<T>() : new Success<T>(value);

      public static IResult<T> Failure<T>(this string message) => failure<T>(new Exception(message));

      public static IResult<T> Failure<T, TException>(this object firstItem, params object[] args) where TException : Exception
      {
         var list = new List<object> { firstItem };
         list.AddRange(args);

         return failure<T>((TException)typeof(TException).Create(list.ToArray()));
      }

      public static IMatched<T> Matched<T>(this T matches) => matches.IsNull() ? "Matches cannot be null".FailedMatch<T>() : new Matched<T>(matches);

      public static IMatched<T> MatchedUnlessNull<T>(this T obj) => obj.IsNull() ? notMatched<T>() : obj.Matched();

      public static IMatched<T> FailedMatch<T>(this string message)
      {
         return new FailedMatch<T>(new ApplicationException(message));
      }

      public static IMatched<T> FailedMatch<T, TException>(this object firstItem, params object[] args) where TException : Exception
      {
         var list = new List<object> { firstItem };
         list.AddRange(args);

         return failedMatch<T>((TException)typeof(TException).Create(list.ToArray()));
      }

      public static IResult<T> Result<T>(this bool test, Func<T> ifFunc, string exceptionMessage)
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

      public static IResult<T> Result<T>(this bool test, Func<T> ifFunc, Func<string> exceptionMessage)
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

      public static IResult<T> Result<T>(this bool test, Func<IResult<T>> ifFunc, string exceptionMessage)
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

      public static IResult<T> Result<T>(this bool test, Func<IResult<T>> ifFunc,
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

      public static IMatched<T> Matching<T>(this bool test, Func<T> ifFunc)
      {
         try
         {
            if (test)
            {
               return ifFunc().Matched();
            }
            else
            {
               return notMatched<T>();
            }
         }
         catch (Exception exception)
         {
            return failedMatch<T>(exception);
         }
      }

      public static IMaybe<T> Tap<T>(this IMaybe<T> maybe, Action<IMaybe<T>> action)
      {
         action(maybe);
         return maybe;
      }

      public static IMatched<T> Tap<T>(this IMatched<T> matched, Action<IMatched<T>> action)
      {
         action(matched);
         return matched;
      }

      public static IResult<T> Tap<T>(this IResult<T> result, Action<IResult<T>> action) => tryTo(() =>
      {
         action(result);
         return result;
      });

      public static IEnumerable<T> WhereIsSome<T>(this IEnumerable<IMaybe<T>> enumerable)
      {
         foreach (var maybe in enumerable)
         {
            if (maybe.If(out var value))
            {
               yield return value;
            }
         }
      }

      public static IEnumerable<T> WhereIsSuccessful<T>(this IEnumerable<IResult<T>> enumerable)
      {
         foreach (var result in enumerable)
         {
            if (result.If(out var value))
            {
               yield return value;
            }
         }
      }

      public static IEnumerable<T> WhereIsMatched<T>(this IEnumerable<IMatched<T>> enumerable)
      {
         foreach (var matched in enumerable)
         {
            if (matched.If(out var value))
            {
               yield return value;
            }
         }
      }

      public static IEnumerable<T> WhereIsCompleted<T>(this IEnumerable<ICompletion<T>> enumerable)
      {
         foreach (var completion in enumerable)
         {
            if (completion.If(out var value))
            {
               yield return value;
            }
         }
      }

      public static IMaybe<IEnumerable<T>> AllAreSome<T>(this IEnumerable<IMaybe<T>> enumerable)
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

      public static IEnumerable<IResult<T>> All<T>(this IEnumerable<IResult<T>> enumerable, Action<T> success = null,
         Action<Exception> failure = null)
      {
         return new ResultIterator<T>(enumerable, success, failure).All();
      }

      public static IEnumerable<T> Successes<T>(this IEnumerable<IResult<T>> enumerable, Action<T> success = null,
         Action<Exception> failure = null)
      {
         return new ResultIterator<T>(enumerable, success, failure).SuccessesOnly();
      }

      public static IEnumerable<T> Matches<T>(this IEnumerable<IMatched<T>> enumerable, Action<T> matched = null,
         Action notMatched = null, Action<Exception> failure = null)
      {
         return new MatchedIterator<T>(enumerable, matched, notMatched, failure).MatchesOnly();
      }

      public static IEnumerable<Exception> Failures<T>(this IEnumerable<IResult<T>> enumerable,
         Action<T> success = null, Action<Exception> failure = null)
      {
         return new ResultIterator<T>(enumerable, success, failure).FailuresOnly();
      }

      public static IEnumerable<Exception> Failures<T>(this IEnumerable<IMatched<T>> enumerable, Action<T> matched = null,
         Action notMatched = null, Action<Exception> failure = null)
      {
         return new MatchedIterator<T>(enumerable, matched, notMatched, failure).FailuresOnly();
      }

      public static (IEnumerable<T> enumerable, IMaybe<Exception> exception) SuccessesFirst<T>(this IEnumerable<IResult<T>> enumerable,
         Action<T> success = null, Action<Exception> failure = null)
      {
         return new ResultIterator<T>(enumerable, success, failure).SuccessesThenFailure();
      }

      public static IResult<IEnumerable<T>> IfAllSuccesses<T>(this IEnumerable<IResult<T>> enumerable,
         Action<T> success = null, Action<Exception> failure = null)
      {
         return new ResultIterator<T>(enumerable, success, failure).IfAllSuccesses();
      }

      public static IResult<TResult> ForAny<TSource, TResult>(this IEnumerable<TSource> enumerable,
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

      public static IResult<TResult> ForAny<TSource, TResult>(this IResult<IEnumerable<TSource>> enumerable,
         Func<TSource, TResult> func)
      {
         return enumerable.Map(e => e.ForAny(func));
      }

      public static IResult<TResult> ForAny<TSource, TResult>(this IEnumerable<TSource> enumerable,
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

      public static IResult<TResult> ForAny<TSource, TResult>(this IResult<IEnumerable<TSource>> enumerable,
         Action<TSource> action, TResult result)
      {
         return enumerable.Map(e => e.ForAny(action, result));
      }

      public static IResult<TResult> ForAny<TSource, TResult>(this IEnumerable<TSource> enumerable,
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

      public static IResult<TResult> ForAny<TSource, TResult>(this IResult<IEnumerable<TSource>> enumerable,
         Action<TSource> action, Func<TResult> result)
      {
         return enumerable.Map(e => e.ForAny(action, result));
      }

      public static IResult<T> Flat<T>(this IResult<IResult<T>> result) => result.Recover(failure<T>);

      public static T ThrowIfFailed<T>(this IResult<T> result)
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

      public static void ForEach<T>(this IResult<IEnumerable<T>> enumerable, Action<T> ifSuccess,
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
         using (var enumerator = enumerable.GetEnumerator())
         {
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
      }

      public static IMaybe<T> IfCast<T>(this object obj) => obj is T t ? t.Some() : none<T>();

      public static bool If<T1, T2>(this IMaybe<(T1, T2)> some, out T1 v1, out T2 v2)
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

      public static bool If<T1, T2, T3>(this IMaybe<(T1, T2, T3)> some, out T1 v1, out T2 v2, out T3 v3)
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

      public static bool If<T1, T2, T3, T4>(this IMaybe<(T1, T2, T3, T4)> some, out T1 v1, out T2 v2, out T3 v3,
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

      public static bool If<T1, T2>(this IResult<(T1, T2)> result, out T1 v1, out T2 v2)
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

      public static bool If<T1, T2>(this IResult<(T1, T2)> result, out T1 v1, out T2 v2, out Exception exception)
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

      public static bool If<T1, T2, T3>(this IResult<(T1, T2, T3)> result, out T1 v1, out T2 v2, out T3 v3)
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

      public static bool If<T1, T2, T3>(this IResult<(T1, T2, T3)> result, out T1 v1, out T2 v2, out T3 v3, out Exception exception)
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

      public static bool If<T1, T2, T3, T4>(this IResult<(T1, T2, T3, T4)> result, out T1 v1, out T2 v2, out T3 v3,
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

      public static bool If<T1, T2, T3, T4>(this IResult<(T1, T2, T3, T4)> result, out T1 v1, out T2 v2, out T3 v3,
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

      public static bool If<T1, T2>(this IMatched<(T1, T2)> matched, out T1 v1, out T2 v2)
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

      public static bool If<T1, T2>(this IMatched<(T1, T2)> matched, out T1 v1, out T2 v2, out IMaybe<Exception> anyException)
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

      public static bool If<T1, T2, T3>(this IMatched<(T1, T2, T3)> matched, out T1 v1, out T2 v2, out T3 v3)
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

      public static bool If<T1, T2, T3>(this IMatched<(T1, T2, T3)> matched, out T1 v1, out T2 v2, out T3 v3, out IMaybe<Exception> anyException)
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

      public static bool If<T1, T2, T3, T4>(this IMatched<(T1, T2, T3, T4)> matched, out T1 v1, out T2 v2, out T3 v3,
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

      public static bool If<T1, T2, T3, T4>(this IMatched<(T1, T2, T3, T4)> matched, out T1 v1, out T2 v2, out T3 v3,
         out T4 v4, out IMaybe<Exception> anyException)
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

      public static bool If<T1, T2>(this ICompletion<(T1, T2)> completion, out T1 v1, out T2 v2)
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

      public static bool If<T1, T2>(this ICompletion<(T1, T2)> completion, out T1 v1, out T2 v2, out IMaybe<Exception> anyException)
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

      public static bool If<T1, T2, T3>(this ICompletion<(T1, T2, T3)> completion, out T1 v1, out T2 v2, out T3 v3)
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

      public static bool If<T1, T2, T3>(this ICompletion<(T1, T2, T3)> completion, out T1 v1, out T2 v2, out T3 v3,
         out IMaybe<Exception> anyException)
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

      public static bool If<T1, T2, T3, T4>(this ICompletion<(T1, T2, T3, T4)> completion, out T1 v1, out T2 v2, out T3 v3, out T4 v4)
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

      public static bool If<T1, T2, T3, T4>(this ICompletion<(T1, T2, T3, T4)> completion, out T1 v1, out T2 v2, out T3 v3, out T4 v4,
         out IMaybe<Exception> anyException)
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

      public static bool ValueOrOriginal<T1, T2>(this IResult<(T1, T2)> result, out T1 v1, out T2 v2, out IResult<(T1, T2)> original)
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

      public static bool ValueOrOriginal<T1, T2, T3>(this IResult<(T1, T2, T3)> result, out T1 v1, out T2 v2, out T3 v3,
         out IResult<(T1, T2, T3)> original)
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

      public static bool ValueOrOriginal<T1, T2, T3, T4>(this IResult<(T1, T2, T3, T4)> result, out T1 v1, out T2 v2, out T3 v3, out T4 v4,
         out IResult<(T1, T2, T3, T4)> original)
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

      public static bool ValueOrOriginal<T1, T2>(this IMatched<(T1, T2)> matched, out T1 v1, out T2 v2, out IMatched<(T1, T2)> original)
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

      public static bool ValueOrOriginal<T1, T2, T3>(this IMatched<(T1, T2, T3)> matched, out T1 v1, out T2 v2, out T3 v3,
         out IMatched<(T1, T2, T3)> original)
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

      public static bool ValueOrOriginal<T1, T2, T3, T4>(this IMatched<(T1, T2, T3, T4)> matched, out T1 v1, out T2 v2, out T3 v3, out T4 v4,
         out IMatched<(T1, T2, T3, T4)> original)
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

      public static bool ValueOrOriginal<T1, T2>(this ICompletion<(T1, T2)> completion, out T1 v1, out T2 v2, out ICompletion<(T1, T2)> original)
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

      public static bool ValueOrOriginal<T1, T2, T3>(this ICompletion<(T1, T2, T3)> completion, out T1 v1, out T2 v2, out T3 v3,
         out ICompletion<(T1, T2, T3)> original)
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

      public static bool ValueOrOriginal<T1, T2, T3, T4>(this ICompletion<(T1, T2, T3, T4)> completion, out T1 v1, out T2 v2, out T3 v3, out T4 v4,
         out ICompletion<(T1, T2, T3, T4)> original)
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

      public static bool ValueOrCast<T1, T2, TResult>(this IResult<(T1, T2)> result, out T1 v1, out T2 v2, out IResult<TResult> castAs)
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

      public static bool ValueOrCast<T1, T2, T3, TResult>(this IResult<(T1, T2, T3)> result, out T1 v1, out T2 v2, out T3 v3,
         out IResult<TResult> castAs)
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

      public static bool ValueOrCast<T1, T2, T3, T4, TResult>(this IResult<(T1, T2, T3, T4)> result, out T1 v1, out T2 v2, out T3 v3, out T4 v4,
         out IResult<TResult> castAs)
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

      public static bool ValueOrCast<T1, T2, TResult>(this IMatched<(T1, T2)> matched, out T1 v1, out T2 v2, out IMatched<TResult> castAs)
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

      public static bool ValueOrCast<T1, T2, T3, TResult>(this IMatched<(T1, T2, T3)> matched, out T1 v1, out T2 v2, out T3 v3,
         out IMatched<TResult> castAs)
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

      public static bool ValueOrCast<T1, T2, T3, T4, TResult>(this IMatched<(T1, T2, T3, T4)> matched, out T1 v1, out T2 v2, out T3 v3, out T4 v4,
         out IMatched<TResult> castAs)
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

      public static bool ValueOrCast<T1, T2, TResult>(this ICompletion<(T1, T2)> completion, out T1 v1, out T2 v2, out ICompletion<TResult> castAs)
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

      public static bool ValueOrCast<T1, T2, T3, TResult>(this ICompletion<(T1, T2, T3)> completion, out T1 v1, out T2 v2, out T3 v3,
         out ICompletion<TResult> castAs)
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

      public static bool ValueOrCast<T1, T2, T3, T4, TResult>(this ICompletion<(T1, T2, T3, T4)> completion, out T1 v1, out T2 v2, out T3 v3,
         out T4 v4, out ICompletion<TResult> castAs)
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

      public static IEnumerable<T> SomeValue<T>(this IEnumerable<IMaybe<T>> enumerable)
      {
         foreach (var source in enumerable)
         {
            if (source.If(out var value))
            {
               yield return value;
            }
         }
      }

      public static IEnumerable<T> SuccessfulValue<T>(this IEnumerable<IResult<T>> enumerable)
      {
         foreach (var result in enumerable)
         {
            if (result.If(out var value))
            {
               yield return value;
            }
         }
      }

      public static ICompletion<T> Completed<T>(this T value) => value.IsNull() ? "value cannot be null".Interrupted<T>() : new Completed<T>(value);

      public static ICompletion<T> Completed<T>(this T value, CancellationToken token)
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

      public static ICompletion<T> Interrupted<T>(this string message) => new Interrupted<T>(new ApplicationException(message));

      public static ICompletion<T> Interrupted<T, TException>(this object firstItem, params object[] args) where TException : Exception
      {
         var list = new List<object> { firstItem };
         list.AddRange(args);

         return interrupted<T>((TException)typeof(TException).Create(list.ToArray()));
      }

      public static ICompletion<T> Completion<T>(this IResult<T> result) => result.Map(v => v.Completed()).Recover(interrupted<T>);

      public static ICompletion<T> Completion<T>(this IResult<T> result, CancellationToken token)
      {
         return result.Map(v => v.Completed(token)).Recover(interrupted<T>);
      }

      public static ICompletion<T> Completion<T>(this IMatched<T> matched)
      {
         return matched.FlatMap(v => v.Completed(), cancelled<T>, interrupted<T>);
      }

      public static ICompletion<T> Completion<T>(this IMatched<T> matched, CancellationToken token)
      {
         return matched.FlatMap(v => v.Completed(token), cancelled<T>, interrupted<T>);
      }

      public static ICompletion<T> Completion<T>(this IMaybe<T> maybe) => maybe.Map(v => v.Completed()).DefaultTo(cancelled<T>);

      public static ICompletion<T> Completion<T>(this IMaybe<T> maybe, CancellationToken token)
      {
         return maybe.Map(v => v.Completed(token)).DefaultTo(cancelled<T>);
      }

      private static ICompletion<T> cancelledOrInterrupted<T>(Exception exception)
      {
         switch (exception)
         {
            case OperationCanceledException _:
            case ObjectDisposedException _:
               return cancelled<T>();
            case FullStackException fullStackException
               when fullStackException.InnerException != null && !(fullStackException.InnerException is FullStackException):
               return cancelledOrInterrupted<T>(fullStackException.InnerException);
            default:
               return interrupted<T>(exception);
         }
      }

      public static async Task<ICompletion<T3>> SelectMany<T1, T2, T3>(this Task<ICompletion<T1>> source, Func<T1, Task<ICompletion<T2>>> func,
         Func<T1, T2, T3> projection)
      {
         var t = await source;
         if (t.If(out var tValue, out var anyException))
         {
            var u = await func(tValue);
            if (u.If(out var uValue, out anyException))
            {
               return projection(tValue, uValue).Completed();
            }
            else if (anyException.If(out var exception))
            {
               return cancelledOrInterrupted<T3>(exception);
            }
            else
            {
               return cancelled<T3>();
            }
         }
         else if (anyException.If(out var exception))
         {
            return cancelledOrInterrupted<T3>(exception);
         }
         else
         {
            return cancelled<T3>();
         }
      }

      public static IResult<T> Result<T>(this ICompletion<T> completion)
      {
         if (completion.If(out var value, out var anyException))
         {
            return value.Success();
         }
         else if (anyException.If(out var exception))
         {
            return failure<T>(exception);
         }
         else
         {
            return "Cancelled".Failure<T>();
         }
      }

      public static IMaybe<T> MaxOrNone<T>(this IEnumerable<T> enumerable)
      {
         var array = enumerable.ToArray();
         if (array.Length == 0)
         {
            return none<T>();
         }
         else
         {
            return array.Max().Some();
         }
      }

      public static IMaybe<T> MaxOrNone<T, TMax>(this IEnumerable<T> enumerable, Func<T, TMax> maxOnFunc)
      {
         var array = enumerable.ToArray();
         if (array.Length == 0)
         {
            return none<T>();
         }
         else
         {
            var max = array.Select((v, i) => (v: maxOnFunc(v), i)).Max();
            return array[max.i].Some();
         }
      }

      public static IMaybe<T> MinOrNone<T>(this IEnumerable<T> enumerable)
      {
         var array = enumerable.ToArray();
         if (array.Length == 0)
         {
            return none<T>();
         }
         else
         {
            return array.Min().Some();
         }
      }

      public static IMaybe<T> MinOrNone<T, TMin>(this IEnumerable<T> enumerable, Func<T, TMin> minOnFunc)
      {
         var array = enumerable.ToArray();
         if (array.Length == 0)
         {
            return none<T>();
         }
         else
         {
            var min = array.Select((v, i) => (v: minOnFunc(v), i)).Min();
            return array[min.i].Some();
         }
      }

      public static IResult<T> MaxOrFail<T>(this IEnumerable<T> enumerable, Func<string> exceptionMessage) => tryTo(() =>
      {
         var array = enumerable.ToArray();
         if (array.Length == 0)
         {
            return exceptionMessage().Failure<T>();
         }
         else
         {
            return array.Max().Success();
         }
      });

      public static IResult<T> MaxOrFail<T, TMax>(this IEnumerable<T> enumerable, Func<T, TMax> maxOnFunc, Func<string> exceptionMessage)
      {
         return tryTo(() =>
         {
            var array = enumerable.ToArray();
            if (array.Length == 0)
            {
               return exceptionMessage().Failure<T>();
            }
            else
            {
               var max = array.Select((v, i) => (v: maxOnFunc(v), i)).Max();
               return array[max.i].Success();
            }
         });
      }

      public static IResult<T> MinOrFail<T>(this IEnumerable<T> enumerable, Func<string> exceptionMessage) => tryTo(() =>
      {
         var array = enumerable.ToArray();
         if (array.Length == 0)
         {
            return exceptionMessage().Failure<T>();
         }
         else
         {
            return array.Min().Success();
         }
      });

      public static IResult<T> MinOrFail<T, TMin>(this IEnumerable<T> enumerable, Func<T, TMin> minOnFunc, Func<string> exceptionMessage)
      {
         return tryTo(() =>
         {
            var array = enumerable.ToArray();
            if (array.Length == 0)
            {
               return exceptionMessage().Failure<T>();
            }
            else
            {
               var min = array.Select((v, i) => (v: minOnFunc(v), i)).Min();
               return array[min.i].Success();
            }
         });
      }

      public static IMatched<T> MaxOrNotMatched<T>(this IEnumerable<T> enumerable)
      {
         var array = enumerable.ToArray();
         if (array.Length == 0)
         {
            return notMatched<T>();
         }
         else
         {
            return array.Max().Matched();
         }
      }

      public static IMatched<T> maxOrNotMatched<T, TMax>(this IEnumerable<T> enumerable, Func<T, TMax> maxOnFunc)
      {
         var array = enumerable.ToArray();
         if (array.Length == 0)
         {
            return notMatched<T>();
         }
         else
         {
            var max = array.Select((v, i) => (v: maxOnFunc(v), i)).Max();
            return array[max.i].Matched();
         }
      }

      public static IMatched<T> minOrNotMatched<T>(this IEnumerable<T> enumerable)
      {
         var array = enumerable.ToArray();
         if (array.Length == 0)
         {
            return notMatched<T>();
         }
         else
         {
            return array.Min().Matched();
         }
      }

      public static IMatched<T> minOrNotMatched<T, TMin>(this IEnumerable<T> enumerable, Func<T, TMin> minOnFunc)
      {
         var array = enumerable.ToArray();
         if (array.Length == 0)
         {
            return notMatched<T>();
         }
         else
         {
            var min = array.Select((v, i) => (v: minOnFunc(v), i)).Min();
            return array[min.i].Matched();
         }
      }

      public static IMaybe<TResult> Map<T1, T2, TResult>(this IMaybe<(T1, T2)> maybe, Func<T1, T2, TResult> func)
      {
         return maybe.Map(t => func(t.Item1, t.Item2));
      }

      public static IMaybe<TResult> Map<T1, T2, TResult>(this IMaybe<(T1, T2)> maybe, Func<T1, T2, IMaybe<TResult>> func)
      {
         return maybe.Map(t => func(t.Item1, t.Item2));
      }

      public static IMaybe<TResult> Map<T1, T2, T3, TResult>(this IMaybe<(T1, T2, T3)> maybe, Func<T1, T2, T3, TResult> func)
      {
         return maybe.Map(t => func(t.Item1, t.Item2, t.Item3));
      }

      public static IMaybe<TResult> Map<T1, T2, T3, TResult>(this IMaybe<(T1, T2, T3)> maybe, Func<T1, T2, T3, IMaybe<TResult>> func)
      {
         return maybe.Map(t => func(t.Item1, t.Item2, t.Item3));
      }

      public static IMaybe<TResult> Map<T1, T2, T3, T4, TResult>(this IMaybe<(T1, T2, T3, T4)> maybe, Func<T1, T2, T3, T4, TResult> func)
      {
         return maybe.Map(t => func(t.Item1, t.Item2, t.Item3, t.Item4));
      }

      public static IMaybe<TResult> Map<T1, T2, T3, T4, TResult>(this IMaybe<(T1, T2, T3, T4)> maybe, Func<T1, T2, T3, T4, IMaybe<TResult>> func)
      {
         return maybe.Map(t => func(t.Item1, t.Item2, t.Item3, t.Item4));
      }

      public static IResult<TResult> Map<T1, T2, TResult>(this IResult<(T1, T2)> result, Func<T1, T2, TResult> func)
      {
         return result.Map(t => func(t.Item1, t.Item2));
      }

      public static IResult<TResult> Map<T1, T2, TResult>(this IResult<(T1, T2)> result, Func<T1, T2, IResult<TResult>> func)
      {
         return result.Map(t => func(t.Item1, t.Item2));
      }

      public static IResult<TResult> Map<T1, T2, T3, TResult>(this IResult<(T1, T2, T3)> result, Func<T1, T2, T3, TResult> func)
      {
         return result.Map(t => func(t.Item1, t.Item2, t.Item3));
      }

      public static IResult<TResult> Map<T1, T2, T3, TResult>(this IResult<(T1, T2, T3)> result, Func<T1, T2, T3, IResult<TResult>> func)
      {
         return result.Map(t => func(t.Item1, t.Item2, t.Item3));
      }

      public static IResult<TResult> Map<T1, T2, T3, T4, TResult>(this IResult<(T1, T2, T3, T4)> result, Func<T1, T2, T3, T4, TResult> func)
      {
         return result.Map(t => func(t.Item1, t.Item2, t.Item3, t.Item4));
      }

      public static IResult<TResult> Map<T1, T2, T3, T4, TResult>(this IResult<(T1, T2, T3, T4)> result, Func<T1, T2, T3, T4, IResult<TResult>> func)
      {
         return result.Map(t => func(t.Item1, t.Item2, t.Item3, t.Item4));
      }

      public static IMatched<TResult> Map<T1, T2, TResult>(this IMatched<(T1, T2)> result, Func<T1, T2, TResult> func)
      {
         return result.Map(t => func(t.Item1, t.Item2));
      }

      public static IMatched<TResult> Map<T1, T2, TResult>(this IMatched<(T1, T2)> result, Func<T1, T2, IMatched<TResult>> func)
      {
         return result.Map(t => func(t.Item1, t.Item2));
      }

      public static IMatched<TResult> Map<T1, T2, T3, TResult>(this IMatched<(T1, T2, T3)> result, Func<T1, T2, T3, TResult> func)
      {
         return result.Map(t => func(t.Item1, t.Item2, t.Item3));
      }

      public static IMatched<TResult> Map<T1, T2, T3, TResult>(this IMatched<(T1, T2, T3)> result, Func<T1, T2, T3, IMatched<TResult>> func)
      {
         return result.Map(t => func(t.Item1, t.Item2, t.Item3));
      }

      public static IMatched<TResult> Map<T1, T2, T3, T4, TResult>(this IMatched<(T1, T2, T3, T4)> result, Func<T1, T2, T3, T4, TResult> func)
      {
         return result.Map(t => func(t.Item1, t.Item2, t.Item3, t.Item4));
      }

      public static ICompletion<TResult> Map<T1, T2, TResult>(this ICompletion<(T1, T2)> completion, Func<T1, T2, TResult> func)
      {
         return completion.Map(t => func(t.Item1, t.Item2));
      }

      public static ICompletion<TResult> Map<T1, T2, TResult>(this ICompletion<(T1, T2)> completion, Func<T1, T2, ICompletion<TResult>> func)
      {
         return completion.Map(t => func(t.Item1, t.Item2));
      }

      public static ICompletion<TResult> Map<T1, T2, T3, TResult>(this ICompletion<(T1, T2, T3)> completion, Func<T1, T2, T3, TResult> func)
      {
         return completion.Map(t => func(t.Item1, t.Item2, t.Item3));
      }

      public static ICompletion<TResult> Map<T1, T2, T3, TResult>(this ICompletion<(T1, T2, T3)> completion,
         Func<T1, T2, T3, ICompletion<TResult>> func)
      {
         return completion.Map(t => func(t.Item1, t.Item2, t.Item3));
      }

      public static ICompletion<TResult> Map<T1, T2, T3, T4, TResult>(this ICompletion<(T1, T2, T3, T4)> completion,
         Func<T1, T2, T3, T4, TResult> func)
      {
         return completion.Map(t => func(t.Item1, t.Item2, t.Item3, t.Item4));
      }

      public static ICompletion<TResult> Map<T1, T2, T3, T4, TResult>(this ICompletion<(T1, T2, T3, T4)> completion,
         Func<T1, T2, T3, T4, ICompletion<TResult>> func)
      {
         return completion.Map(t => func(t.Item1, t.Item2, t.Item3, t.Item4));
      }

      public static IMaybe<T> SomeIf<T>(this Func<bool> boolExpression, Func<T> value)
      {
         return boolExpression() ? value().Some() : none<T>();
      }

      public static IMaybe<T> SomeIf<T>(this Func<bool> boolExpression, Func<IMaybe<T>> value)
      {
         return boolExpression() ? value() : none<T>();
      }

      public static IMaybe<T> SomeIf<T>(this bool boolExpression, Func<T> value)
      {
         return boolExpression ? value().Some() : none<T>();
      }

      public static IMaybe<T> SomeIf<T>(this bool boolExpression, Func<IMaybe<T>> value)
      {
         return boolExpression ? value() : none<T>();
      }
   }
}