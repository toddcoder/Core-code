using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Core.Objects;
using Core.RegularExpressions;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
	public static class MonadExtensions
	{
		public static IMaybe<T> SomeIfNotNull<T>(this T obj) => maybe(!obj.IsNull(), () => obj);

		public static IMaybe<T> Some<T>(this T obj) => new Some<T>(obj);

		public static IMaybe<T> FirstOrNone<T>(this IEnumerable<T> enumerable)
		{
			try
			{
				var first = enumerable.First();

				if (first.IsNull())
            {
               return none<T>();
            }
            else if (first.Equals(default))
            {
               return none<T>();
            }
            else
            {
               return first.Some();
            }
         }
			catch
			{
				return none<T>();
			}
		}

		public static IMaybe<T> LastOrNone<T>(this IEnumerable<T> enumerable)
		{
			try
			{
				var last = enumerable.Last();

				if (last.IsNull())
            {
               return none<T>();
            }
            else if (last.Equals(default))
            {
               return none<T>();
            }
            else
            {
               return last.Some();
            }
         }
			catch
			{
				return none<T>();
			}
		}

		public static IResult<T> FirstOrFail<T>(this IEnumerable<T> enumerable) => tryTo(() =>
		{
			var first = enumerable.First();
			if (first.IsNull())
         {
            return "Default value".Failure<T>();
         }
         else
         {
            return first.Success();
         }
      });

		public static IResult<T> LastOrFail<T>(this IEnumerable<T> enumerable) => tryTo(() =>
		{
			var last = enumerable.Last();
			if (last.IsNull())
         {
            return "Default value".Failure<T>();
         }
         else
         {
            return last.Success();
         }
      });

		public static IMaybe<T> FirstOrNone<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
		{
			try
			{
				var first = enumerable.First(predicate);

				if (first.IsNull())
            {
               return none<T>();
            }
            else if (first.Equals(default))
            {
               return none<T>();
            }
            else
            {
               return first.Some();
            }
         }
			catch
			{
				return none<T>();
			}
		}

		public static IMaybe<T> LastOrNone<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
		{
			try
			{
				var last = enumerable.Last(predicate);

				if (last.IsNull())
            {
               return none<T>();
            }
            else if (last.Equals(default))
            {
               return none<T>();
            }
            else
            {
               return last.Some();
            }
         }
			catch
			{
				return none<T>();
			}
		}

		public static IMatched<T> FirstOrNotMatched<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
		{
			try
			{
				var first = enumerable.First(predicate);

				if (first.IsNull())
            {
               return notMatched<T>();
            }
            else if (first.Equals(default))
            {
               return notMatched<T>();
            }
            else
            {
               return first.Matched();
            }
         }
			catch (Exception exception)
			{
				return failedMatch<T>(exception);
			}
		}

		public static IMatched<T> LastOrNotMatched<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
		{
			try
			{
				var last = enumerable.Last(predicate);

				if (last.IsNull())
            {
               return notMatched<T>();
            }
            else if (last.Equals(default))
            {
               return notMatched<T>();
            }
            else
            {
               return last.Matched();
            }
         }
			catch (Exception exception)
			{
				return failedMatch<T>(exception);
			}
		}

		public static IMatched<T> FirstOrNotMatched<T>(this IEnumerable<T> enumerable)
		{
			try
			{
				var first = enumerable.First();

				if (first.IsNull())
            {
               return notMatched<T>();
            }
            else if (first.Equals(default))
            {
               return notMatched<T>();
            }
            else
            {
               return first.Matched();
            }
         }
			catch (Exception exception)
			{
				return failedMatch<T>(exception);
			}
		}

		public static IMatched<T> LastOrNotMatched<T>(this IEnumerable<T> enumerable)
		{
			try
			{
				var last = enumerable.Last();

				if (last.IsNull())
            {
               return notMatched<T>();
            }
            else if (last.Equals(default))
            {
               return notMatched<T>();
            }
            else
            {
               return last.Matched();
            }
         }
			catch (Exception exception)
			{
				return failedMatch<T>(exception);
			}
		}

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
		public static IResult<TResult> SelectMany<T, TResult>(this IMaybe<T> maybe,
			Func<T, IResult<TResult>> projection)
		{
			return maybe.FlatMap(projection, () => "Value not provided".Failure<TResult>());
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
			if (result.If(out var value))
         {
            return func(value).Success();
         }
         else
         {
            return failure<TResult>(result.Exception);
         }
      }

		public static bool Assign<T>(this IMaybe<T> maybe, out T value)
		{
			value = maybe.FlatMap(v => v, () => default);
			return maybe.IsSome;
		}

		public static IResult<T> Success<T>(this T value) => new Success<T>(value);

		public static IResult<T> Failure<T>(this string message) => failure<T>(new Exception(message));

		public static IMatched<T> Matched<T>(this T matches) => new Matched<T>(matches);

		public static IMatched<T> MatchedUnlessNull<T>(this T obj) => obj.IsNull() ? new NotMatched<T>() : obj.Matched();

		public static IMatched<T> FailedMatch<T>(this string message)
		{
			return new FailedMatch<T>(new ApplicationException(message));
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
            if (result.Out(out var value, out var original))
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

/*		public static IEither<MultiExceptions, TResult> ForAll<TSource, TResult>(this IEnumerable<TSource> enumerable,
			Action<TSource> action, TResult result)
		{
			var exceptions = new MultiExceptions();
			foreach (var item in enumerable)
				try
				{
					action(item);
				}
				catch (Exception exception)
				{
					exceptions.Add(exception);
				}

			if (exceptions.Count == 0)
				return result.RightHand<MultiExceptions, TResult>();
			else
				return exceptions.LeftHand<MultiExceptions, TResult>();
		}

		public static IEither<MultiExceptions, TResult> ForAll<TSource, TResult>(this IEnumerable<TSource> enumerable,
			Action<TSource> action, Func<TResult> result)
		{
			var exceptions = new MultiExceptions();
			foreach (var item in enumerable)
				try
				{
					action(item);
				}
				catch (Exception exception)
				{
					exceptions.Add(exception);
				}

			if (exceptions.Count == 0)
				return result().RightHand<MultiExceptions, TResult>();
			else
				return exceptions.LeftHand<MultiExceptions, TResult>();
		}*/

		public static IResult<T> Flat<T>(this IResult<IResult<T>> result) => result.FlatMap(r => r, failure<T>);

		public static T ThrowIfFailed<T>(this IResult<T> result)
		{
			if (result.If(out var value))
         {
            return value;
         }
         else
         {
            throw result.Exception;
         }
      }

		public static void ForEach<T>(this IResult<IEnumerable<T>> enumerable, Action<T> ifSuccess,
			Action<Exception> ifFailure)
		{
			if (enumerable.If(out var e))
         {
            e.ForEach(ifSuccess, ifFailure);
         }
         else
         {
            ifFailure(enumerable.Exception);
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
	}
}