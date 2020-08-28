using System;
using System.Collections.Generic;
using System.Linq;
using Core.Arrays;
using Core.Assertions;
using Core.Collections;
using Core.Exceptions;
using Core.Monads;
using Core.Numbers;
using Core.Objects;
using Core.Strings;
using static Core.Assertions.AssertionFunctions;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Enumerables
{
   public static class EnumerableExtensions
   {
      [Obsolete("Use .ToString(connector)")]
      public static string Stringify<T>(this IEnumerable<T> enumerable, string connector = ", ")
      {
         return string.Join(connector ?? "", enumerable);
      }

      public static string ToString<T>(this IEnumerable<T> enumerable, string connector)
      {
         assert(() => enumerable).Must().Not.BeNull().OrThrow();
         assert(() => connector).Must().Not.BeNull().OrThrow();

         return string.Join(connector, enumerable.Select(i => i.ToNonNullString()));
      }

      public static IEnumerable<IEnumerable<T>> Pivot<T>(this IEnumerable<IEnumerable<T>> source, Func<T> defaultValue)
      {
         var array = source.Select(row => row.ToArray()).ToArray();
         if (array.Length != 0)
         {
            var maxRowLen = array.Select(a => a.Length).Max();
            var minRowLen = array.Select(a => a.Length).Min();
            var squared = array;
            if (minRowLen != maxRowLen)
            {
               squared = array.Select(row => row.Pad(maxRowLen, defaultValue())).ToArray();
            }

            return 0.Until(maxRowLen).Select(i => squared.Select(row => row[i]).ToArray());
         }
         else
         {
            return array;
         }
      }

      public static IEnumerable<IEnumerable<T>> Pivot<T>(this IEnumerable<IEnumerable<T>> source) => source.Pivot(() => default);

      public static IResult<T[]> ToResultOfArray<T>(this IEnumerable<T> enumerable) => tryTo(enumerable.ToArray);

      public static IResult<List<T>> ToResultOfList<T>(this IEnumerable<T> enumerable) => tryTo(enumerable.ToList);

      public static IEnumerable<int> UpTo(this int from, int to, int by = 1)
      {
         for (var i = from; i <= to; i += by)
         {
            yield return i;
         }
      }

      public static IEnumerable<char> UpTo(this char from, char to, int by = 1)
      {
         for (var i = from; i <= to; i = (char)(i + by))
         {
            yield return i;
         }
      }

      public static IEnumerable<int> UpUntil(this int from, int to, int by = 1)
      {
         for (var i = from; i < to; i += by)
         {
            yield return i;
         }
      }

      public static IEnumerable<char> UpUntil(this char from, char to, int by = 1)
      {
         for (var i = from; i < to; i = (char)(i + by))
         {
            yield return i;
         }
      }

      public static IEnumerable<int> DownTo(this int from, int to, int by = -1)
      {
         for (var i = from; i >= to; i += by)
         {
            yield return i;
         }
      }

      public static IEnumerable<char> DownTo(this char from, char to, int by = -1)
      {
         for (var i = from; i >= to; i = (char)(i + by))
         {
            yield return i;
         }
      }

      public static IEnumerable<int> DownUntil(this int from, int to, int by = -1)
      {
         for (var i = from; i > to; i += by)
         {
            yield return i;
         }
      }

      public static IEnumerable<char> DownUntil(this char from, char to, int by = -1)
      {
         for (var i = from; i > to; i = (char)(i + by))
         {
            yield return i;
         }
      }

      public static IEnumerable<T> Then<T>(this T seed, Func<T, T> next, Predicate<T> stop)
      {
         var current = seed;

         yield return current;

         while (!stop(current))
         {
            current = next(current);
            yield return current;
         }
      }

      public static IEnumerable<T> Then<T>(this T seed, Func<T, T> next) => seed.Then(next, v => false);

      public static IEnumerable<(int, T)> Indexed<T>(this IEnumerable<T> enumerable)
      {
         var index = 0;
         foreach (var item in enumerable)
         {
            yield return (index++, item);
         }
      }

      public static IMaybe<T> FirstOrNone<T>(this IEnumerable<T> enumerable)
      {
         var first = enumerable.FirstOrDefault();

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

      public static IMaybe<T> LastOrNone<T>(this IEnumerable<T> enumerable)
      {
         var last = enumerable.LastOrDefault();

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

      public static IResult<T> FirstOrFail<T>(this IEnumerable<T> enumerable, string failureMessage = "Default value")
      {
         try
         {
            return enumerable.First().Success();
         }
         catch (InvalidOperationException)
         {
            return failureMessage.Failure<T>();
         }
         catch (Exception exception)
         {
            return failure<T>(exception);
         }
      }

      public static IResult<T> FirstOrFail<T>(this IEnumerable<T> enumerable, Predicate<T> predicate, string failureMessage = "Default value")
      {
         try
         {
            return enumerable.First(i => predicate(i)).Success();
         }
         catch (InvalidOperationException)
         {
            return failureMessage.Failure<T>();
         }
         catch (Exception exception)
         {
            return failure<T>(exception);
         }
      }

      public static IResult<(T1, T2)> FirstOrFail<T1, T2>(this IEnumerable<(T1, T2)> enumerable, Func<T1, T2, bool> predicate,
         string failureMessage = "Default value")
      {
         try
         {
            return enumerable.First(i => predicate(i.Item1, i.Item2)).Success();
         }
         catch (InvalidOperationException)
         {
            return failureMessage.Failure<(T1, T2)>();
         }
         catch (Exception exception)
         {
            return failure<(T1, T2)>(exception);
         }
      }

      public static IResult<(T1, T2, T3)> FirstOrFail<T1, T2, T3>(this IEnumerable<(T1, T2, T3)> enumerable, Func<T1, T2, T3, bool> predicate,
         string failureMessage = "Default value")
      {
         try
         {
            return enumerable.First(i => predicate(i.Item1, i.Item2, i.Item3)).Success();
         }
         catch (InvalidOperationException)
         {
            return failureMessage.Failure<(T1, T2, T3)>();
         }
         catch (Exception exception)
         {
            return failure<(T1, T2, T3)>(exception);
         }
      }

      public static IResult<(T1, T2, T3, T4)> FirstOrFail<T1, T2, T3, T4>(this IEnumerable<(T1, T2, T3, T4)> enumerable,
         Func<T1, T2, T3, T4, bool> predicate, string failureMessage = "Default value")
      {
         try
         {
            return enumerable.First(i => predicate(i.Item1, i.Item2, i.Item3, i.Item4)).Success();
         }
         catch (InvalidOperationException)
         {
            return failureMessage.Failure<(T1, T2, T3, T4)>();
         }
         catch (Exception exception)
         {
            return failure<(T1, T2, T3, T4)>(exception);
         }
      }

      public static IResult<T> FirstOrFail<T>(this IEnumerable<T> enumerable, Func<string> failureMessage)
      {
         try
         {
            return enumerable.First().Success();
         }
         catch (InvalidOperationException)
         {
            return failureMessage().Failure<T>();
         }
         catch (Exception exception)
         {
            return failure<T>(exception);
         }
      }

      public static IResult<T> FirstOrFail<T>(this IEnumerable<T> enumerable, Predicate<T> predicate, Func<string> failureMessage)
      {
         try
         {
            return enumerable.First(i => predicate(i)).Success();
         }
         catch (InvalidOperationException)
         {
            return failureMessage().Failure<T>();
         }
         catch (Exception exception)
         {
            return failure<T>(exception);
         }
      }

      public static IResult<(T1, T2)> FirstOrFail<T1, T2>(this IEnumerable<(T1, T2)> enumerable, Func<T1, T2, bool> predicate,
         Func<string> failureMessage)
      {
         try
         {
            return enumerable.First(i => predicate(i.Item1, i.Item2)).Success();
         }
         catch (InvalidOperationException)
         {
            return failureMessage().Failure<(T1, T2)>();
         }
         catch (Exception exception)
         {
            return failure<(T1, T2)>(exception);
         }
      }

      public static IResult<(T1, T2, T3)> FirstOrFail<T1, T2, T3>(this IEnumerable<(T1, T2, T3)> enumerable, Func<T1, T2, T3, bool> predicate,
         Func<string> failureMessage)
      {
         try
         {
            return enumerable.First(i => predicate(i.Item1, i.Item2, i.Item3)).Success();
         }
         catch (InvalidOperationException)
         {
            return failureMessage().Failure<(T1, T2, T3)>();
         }
         catch (Exception exception)
         {
            return failure<(T1, T2, T3)>(exception);
         }
      }

      public static IResult<(T1, T2, T3, T4)> FirstOrFail<T1, T2, T3, T4>(this IEnumerable<(T1, T2, T3, T4)> enumerable,
         Func<T1, T2, T3, T4, bool> predicate, Func<string> failureMessage)
      {
         try
         {
            return enumerable.First(i => predicate(i.Item1, i.Item2, i.Item3, i.Item4)).Success();
         }
         catch (InvalidOperationException)
         {
            return failureMessage().Failure<(T1, T2, T3, T4)>();
         }
         catch (Exception exception)
         {
            return failure<(T1, T2, T3, T4)>(exception);
         }
      }

      public static IResult<T> LastOrFail<T>(this IEnumerable<T> enumerable, string failureMessage = "Default value")
      {
         try
         {
            return enumerable.Last().Success();
         }
         catch (InvalidOperationException)
         {
            return failureMessage.Failure<T>();
         }
         catch (Exception exception)
         {
            return failure<T>(exception);
         }
      }

      public static IResult<T> LastOrFail<T>(this IEnumerable<T> enumerable, Predicate<T> predicate, string failureMessage = "Default value")
      {
         try
         {
            return enumerable.Last(i => predicate(i)).Success();
         }
         catch (InvalidOperationException)
         {
            return failureMessage.Failure<T>();
         }
         catch (Exception exception)
         {
            return failure<T>(exception);
         }
      }

      public static IResult<(T1, T2)> LastOrFail<T1, T2>(this IEnumerable<(T1, T2)> enumerable, Func<T1, T2, bool> predicate,
         string failureMessage = "Default value")
      {
         try
         {
            return enumerable.Last(i => predicate(i.Item1, i.Item2)).Success();
         }
         catch (InvalidOperationException)
         {
            return failureMessage.Failure<(T1, T2)>();
         }
         catch (Exception exception)
         {
            return failure<(T1, T2)>(exception);
         }
      }

      public static IResult<(T1, T2, T3)> LastOrFail<T1, T2, T3>(this IEnumerable<(T1, T2, T3)> enumerable, Func<T1, T2, T3, bool> predicate,
         string failureMessage = "Default value")
      {
         try
         {
            return enumerable.Last(i => predicate(i.Item1, i.Item2, i.Item3)).Success();
         }
         catch (InvalidOperationException)
         {
            return failureMessage.Failure<(T1, T2, T3)>();
         }
         catch (Exception exception)
         {
            return failure<(T1, T2, T3)>(exception);
         }
      }

      public static IResult<T> LastOrFail<T>(this IEnumerable<T> enumerable, Func<string> failureMessage)
      {
         try
         {
            return enumerable.Last().Success();
         }
         catch (InvalidOperationException)
         {
            return failureMessage().Failure<T>();
         }
         catch (Exception exception)
         {
            return failure<T>(exception);
         }
      }

      public static IResult<T> LastOrFail<T>(this IEnumerable<T> enumerable, Predicate<T> predicate, Func<string> failureMessage)
      {
         return tryTo(() =>
         {
            try
            {
               return enumerable.Last(i => predicate(i)).Success();
            }
            catch
            {
               return failureMessage().Failure<T>();
            }
         });
      }

      public static IResult<(T1, T2)> LastOrFail<T1, T2>(this IEnumerable<(T1, T2)> enumerable, Func<T1, T2, bool> predicate,
         Func<string> failureMessage)
      {
         return tryTo(() =>
         {
            try
            {
               return enumerable.Last(i => predicate(i.Item1, i.Item2)).Success();
            }
            catch
            {
               return failureMessage().Failure<(T1, T2)>();
            }
         });
      }

      public static IResult<(T1, T2, T3)> LastOrFail<T1, T2, T3>(this IEnumerable<(T1, T2, T3)> enumerable, Func<T1, T2, T3, bool> predicate,
         Func<string> failureMessage)
      {
         return tryTo(() =>
         {
            try
            {
               return enumerable.Last(i => predicate(i.Item1, i.Item2, i.Item3)).Success();
            }
            catch
            {
               return failureMessage().Failure<(T1, T2, T3)>();
            }
         });
      }

      public static IResult<(T1, T2, T3, T4)> LastOrFail<T1, T2, T3, T4>(this IEnumerable<(T1, T2, T3, T4)> enumerable,
         Func<T1, T2, T3, T4, bool> predicate, Func<string> failureMessage)
      {
         return tryTo(() =>
         {
            try
            {
               return enumerable.Last(i => predicate(i.Item1, i.Item2, i.Item3, i.Item4)).Success();
            }
            catch
            {
               return failureMessage().Failure<(T1, T2, T3, T4)>();
            }
         });
      }

      public static IMaybe<T> FirstOrNone<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
      {
         var first = enumerable.FirstOrDefault(predicate);

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

      public static IMaybe<(T1, T2)> FirstOrNone<T1, T2>(this IEnumerable<(T1, T2)> enumerable, Func<T1, T2, bool> predicate)
      {
         var first = enumerable.FirstOrDefault(i => predicate(i.Item1, i.Item2));

         if (first.IsNull())
         {
            return none<(T1, T2)>();
         }
         else if (first.Equals(default))
         {
            return none<(T1, T2)>();
         }
         else
         {
            return first.Some();
         }
      }

      public static IMaybe<(T1, T2, T3)> FirstOrNone<T1, T2, T3>(this IEnumerable<(T1, T2, T3)> enumerable, Func<T1, T2, T3, bool> predicate)
      {
         var first = enumerable.FirstOrDefault(i => predicate(i.Item1, i.Item2, i.Item3));

         if (first.IsNull())
         {
            return none<(T1, T2, T3)>();
         }
         else if (first.Equals(default))
         {
            return none<(T1, T2, T3)>();
         }
         else
         {
            return first.Some();
         }
      }

      public static IMaybe<(T1, T2, T3, T4)> FirstOrNone<T1, T2, T3, T4>(this IEnumerable<(T1, T2, T3, T4)> enumerable,
         Func<T1, T2, T3, T4, bool> predicate)
      {
         var first = enumerable.FirstOrDefault(i => predicate(i.Item1, i.Item2, i.Item3, i.Item4));

         if (first.IsNull())
         {
            return none<(T1, T2, T3, T4)>();
         }
         else if (first.Equals(default))
         {
            return none<(T1, T2, T3, T4)>();
         }
         else
         {
            return first.Some();
         }
      }

      public static IMaybe<T> LastOrNone<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
      {
         var last = enumerable.LastOrDefault(predicate);

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

      public static IMaybe<(T1, T2)> LastOrNone<T1, T2>(this IEnumerable<(T1, T2)> enumerable, Func<T1, T2, bool> predicate)
      {
         var last = enumerable.LastOrDefault(i => predicate(i.Item1, i.Item2));

         if (last.IsNull())
         {
            return none<(T1, T2)>();
         }
         else if (last.Equals(default))
         {
            return none<(T1, T2)>();
         }
         else
         {
            return last.Some();
         }
      }

      public static IMaybe<(T1, T2, T3)> LastOrNone<T1, T2, T3>(this IEnumerable<(T1, T2, T3)> enumerable, Func<T1, T2, T3, bool> predicate)
      {
         var last = enumerable.LastOrDefault(i => predicate(i.Item1, i.Item2, i.Item3));

         if (last.IsNull())
         {
            return none<(T1, T2, T3)>();
         }
         else if (last.Equals(default))
         {
            return none<(T1, T2, T3)>();
         }
         else
         {
            return last.Some();
         }
      }

      public static IMaybe<(T1, T2, T3, T4)> LastOrNone<T1, T2, T3, T4>(this IEnumerable<(T1, T2, T3, T4)> enumerable,
         Func<T1, T2, T3, T4, bool> predicate)
      {
         var last = enumerable.LastOrDefault(i => predicate(i.Item1, i.Item2, i.Item3, i.Item4));

         if (last.IsNull())
         {
            return none<(T1, T2, T3, T4)>();
         }
         else if (last.Equals(default))
         {
            return none<(T1, T2, T3, T4)>();
         }
         else
         {
            return last.Some();
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
         catch (InvalidOperationException)
         {
            return notMatched<T>();
         }
         catch (Exception exception)
         {
            return failedMatch<T>(exception);
         }
      }

      public static IMatched<(T1, T2)> FirstOrNotMatched<T1, T2>(this IEnumerable<(T1, T2)> enumerable, Func<T1, T2, bool> predicate)
      {
         try
         {
            var first = enumerable.First(i => predicate(i.Item1, i.Item2));

            if (first.IsNull())
            {
               return notMatched<(T1, T2)>();
            }
            else if (first.Equals(default))
            {
               return notMatched<(T1, T2)>();
            }
            else
            {
               return first.Matched();
            }
         }
         catch (InvalidOperationException)
         {
            return notMatched<(T1, T2)>();
         }
         catch (Exception exception)
         {
            return failedMatch<(T1, T2)>(exception);
         }
      }

      public static IMatched<(T1, T2, T3)> FirstOrNotMatched<T1, T2, T3>(this IEnumerable<(T1, T2, T3)> enumerable, Func<T1, T2, T3, bool> predicate)
      {
         try
         {
            var first = enumerable.First(i => predicate(i.Item1, i.Item2, i.Item3));

            if (first.IsNull())
            {
               return notMatched<(T1, T2, T3)>();
            }
            else if (first.Equals(default))
            {
               return notMatched<(T1, T2, T3)>();
            }
            else
            {
               return first.Matched();
            }
         }
         catch (InvalidOperationException)
         {
            return notMatched<(T1, T2, T3)>();
         }
         catch (Exception exception)
         {
            return failedMatch<(T1, T2, T3)>(exception);
         }
      }

      public static IMatched<(T1, T2, T3, T4)> FirstOrNotMatched<T1, T2, T3, T4>(this IEnumerable<(T1, T2, T3, T4)> enumerable,
         Func<T1, T2, T3, T4, bool> predicate)
      {
         try
         {
            var first = enumerable.First(i => predicate(i.Item1, i.Item2, i.Item3, i.Item4));

            if (first.IsNull())
            {
               return notMatched<(T1, T2, T3, T4)>();
            }
            else if (first.Equals(default))
            {
               return notMatched<(T1, T2, T3, T4)>();
            }
            else
            {
               return first.Matched();
            }
         }
         catch (InvalidOperationException)
         {
            return notMatched<(T1, T2, T3, T4)>();
         }
         catch (Exception exception)
         {
            return failedMatch<(T1, T2, T3, T4)>(exception);
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
         catch (InvalidOperationException)
         {
            return notMatched<T>();
         }
         catch (Exception exception)
         {
            return failedMatch<T>(exception);
         }
      }

      public static IMatched<(T1, T2)> LastOrNotMatched<T1, T2>(this IEnumerable<(T1, T2)> enumerable, Func<T1, T2, bool> predicate)
      {
         try
         {
            var last = enumerable.Last(i => predicate(i.Item1, i.Item2));

            if (last.IsNull())
            {
               return notMatched<(T1, T2)>();
            }
            else if (last.Equals(default))
            {
               return notMatched<(T1, T2)>();
            }
            else
            {
               return last.Matched();
            }
         }
         catch (InvalidOperationException)
         {
            return notMatched<(T1, T2)>();
         }
         catch (Exception exception)
         {
            return failedMatch<(T1, T2)>(exception);
         }
      }

      public static IMatched<(T1, T2, T3)> LastOrNotMatched<T1, T2, T3>(this IEnumerable<(T1, T2, T3)> enumerable, Func<T1, T2, T3, bool> predicate)
      {
         try
         {
            var last = enumerable.Last(i => predicate(i.Item1, i.Item2, i.Item3));

            if (last.IsNull())
            {
               return notMatched<(T1, T2, T3)>();
            }
            else if (last.Equals(default))
            {
               return notMatched<(T1, T2, T3)>();
            }
            else
            {
               return last.Matched();
            }
         }
         catch (InvalidOperationException)
         {
            return notMatched<(T1, T2, T3)>();
         }
         catch (Exception exception)
         {
            return failedMatch<(T1, T2, T3)>(exception);
         }
      }

      public static IMatched<(T1, T2, T3, T4)> LastOrNotMatched<T1, T2, T3, T4>(this IEnumerable<(T1, T2, T3, T4)> enumerable,
         Func<T1, T2, T3, T4, bool> predicate)
      {
         try
         {
            var last = enumerable.Last(i => predicate(i.Item1, i.Item2, i.Item3, i.Item4));

            if (last.IsNull())
            {
               return notMatched<(T1, T2, T3, T4)>();
            }
            else if (last.Equals(default))
            {
               return notMatched<(T1, T2, T3, T4)>();
            }
            else
            {
               return last.Matched();
            }
         }
         catch (InvalidOperationException)
         {
            return notMatched<(T1, T2, T3, T4)>();
         }
         catch (Exception exception)
         {
            return failedMatch<(T1, T2, T3, T4)>(exception);
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
         catch (InvalidOperationException)
         {
            return notMatched<T>();
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
         catch (InvalidOperationException)
         {
            return notMatched<T>();
         }
         catch (Exception exception)
         {
            return failedMatch<T>(exception);
         }
      }

      public static Hash<TKey, TValue> ToHash<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> enumerable)
      {
         return enumerable.ToHash(kv => kv.Key, kv => kv.Value);
      }

      public static Hash<TKey, TValue> ToHash<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> enumerable, IEqualityComparer<TKey> comparer)
      {
         return enumerable.ToHash(kv => kv.Key, kv => kv.Value, comparer);
      }

      public static IEnumerable<TResult> Map<TSource, TResult>(this IEnumerable<TSource> enumerable, Func<TSource, TResult> mapFunc)
      {
         return enumerable.Select(mapFunc);
      }

      public static IEnumerable<TResult> FlatMap<TSource, TResult>(this IEnumerable<IEnumerable<TSource>> enumerable,
         Func<TSource, TResult> mapFunc)
      {
         foreach (var outer in enumerable)
         {
            foreach (var inner in outer)
            {
               yield return mapFunc(inner);
            }
         }
      }

      public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable)
      {
         foreach (var outer in enumerable)
         {
            foreach (var inner in outer)
            {
               yield return inner;
            }
         }
      }

      public static IEnumerable<T> If<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate) => enumerable.Where(predicate);

      public static TResult FoldLeft<TSource, TResult>(this IEnumerable<TSource> enumerable, TResult init,
         Func<TResult, TSource, TResult> foldFunc)
      {
         return enumerable.Aggregate(init, foldFunc);
      }

      public static TResult FoldRight<TSource, TResult>(this IEnumerable<TSource> enumerable, TResult init,
         Func<TSource, TResult, TResult> foldFunc)
      {
         var list = enumerable.ToList();
         var accumulator = init;
         for (var i = list.Count - 1; i >= 0; i--)
         {
            accumulator = foldFunc(list[i], accumulator);
         }

         return accumulator;
      }

      public static T FoldLeft<T>(this IEnumerable<T> enumerable, Func<T, T, T> foldFunc) => enumerable.Aggregate(foldFunc);

      public static T FoldRight<T>(this IEnumerable<T> enumerable, Func<T, T, T> foldFunc)
      {
         var list = enumerable.ToList();
         if (list.Count == 0)
         {
            throw "Enumerable can't be empty".Throws();
         }

         var accumulator = list[list.Count - 1];
         for (var i = list.Count - 2; i >= 0; i--)
         {
            accumulator = foldFunc(list[i], accumulator);
         }

         return accumulator;
      }

      public static Hash<TKey, TValue[]> Group<TKey, TValue>(this IEnumerable<TValue> enumerable, Func<TValue, TKey> groupingFunc)
      {
         var hash = new Hash<TKey, List<TValue>>();
         foreach (var value in enumerable)
         {
            var key = groupingFunc(value);
            if (hash.ContainsKey(key))
            {
               hash[key].Add(value);
            }
            else
            {
               hash[key] = new List<TValue> { value };
            }
         }

         var result = new Hash<TKey, TValue[]>();
         foreach (var (key, value) in hash)
         {
            result[key] = value.ToArray();
         }

         return result;
      }

      public static (IEnumerable<T> isTrue, IEnumerable<T> isFalse) Partition<T>(this IEnumerable<T> enumerable,
         Predicate<T> predicate)
      {
         var isTrue = new List<T>();
         var isFalse = new List<T>();
         foreach (var item in enumerable)
         {
            if (predicate(item))
            {
               isTrue.Add(item);
            }
            else
            {
               isFalse.Add(item);
            }
         }

         return (isTrue, isFalse);
      }

      public static IMaybe<int> IndexOfMax<T>(this IEnumerable<T> enumerable)
         where T : IComparable<T>
      {
         var index = none<int>();
         var currentIndex = 0;
         var anyCurrentValue = none<T>();
         foreach (var item in enumerable)
         {
            if (anyCurrentValue.If(out var currentValue))
            {
               if (item.ComparedTo(currentValue) > 0)
               {
                  index = currentIndex.Some();
                  anyCurrentValue = item.Some();
               }
            }
            else
            {
               index = currentIndex.Some();
               anyCurrentValue = item.Some();
            }

            currentIndex++;
         }

         return index;
      }

      public static IMaybe<int> IndexOfMax<TSource, TResult>(this IEnumerable<TSource> enumerable, Func<TSource, TResult> mappingFunc)
         where TResult : IComparable<TResult>
      {
         var index = none<int>();
         var currentIndex = 0;
         var anyCurrentValue = none<TResult>();
         foreach (var item in enumerable)
         {
            var mappedItem = mappingFunc(item);
            if (anyCurrentValue.If(out var currentValue))
            {
               if (mappedItem.ComparedTo(currentValue) > 0)
               {
                  index = currentIndex.Some();
                  anyCurrentValue = mappedItem.Some();
               }
            }
            else
            {
               index = currentIndex.Some();
               anyCurrentValue = mappedItem.Some();
            }

            currentIndex++;
         }

         return index;
      }

      public static IMaybe<int> IndexOfMin<T>(this IEnumerable<T> enumerable)
         where T : IComparable<T>
      {
         var index = none<int>();
         var currentIndex = 0;
         var anyCurrentValue = none<T>();
         foreach (var item in enumerable)
         {
            if (anyCurrentValue.If(out var currentValue))
            {
               if (item.ComparedTo(currentValue) < 0)
               {
                  index = currentIndex.Some();
                  anyCurrentValue = item.Some();
               }
            }
            else
            {
               index = currentIndex.Some();
               anyCurrentValue = item.Some();
            }

            currentIndex++;
         }

         return index;
      }

      public static IMaybe<int> IndexOfMin<TSource, TResult>(this IEnumerable<TSource> enumerable, Func<TSource, TResult> mappingFunc)
         where TResult : IComparable<TResult>
      {
         var index = none<int>();
         var currentIndex = 0;
         var anyCurrentValue = none<TResult>();
         foreach (var item in enumerable)
         {
            var mappedItem = mappingFunc(item);
            if (anyCurrentValue.If(out var currentValue))
            {
               if (mappedItem.ComparedTo(currentValue) < 0)
               {
                  index = currentIndex.Some();
                  anyCurrentValue = mappedItem.Some();
               }
            }
            else
            {
               index = currentIndex.Some();
               anyCurrentValue = mappedItem.Some();
            }

            currentIndex++;
         }

         return index;
      }

      public static IEnumerable<T> Reversed<T>(this IEnumerable<T> enumerable)
      {
         var list = enumerable.ToList();
         list.Reverse();

         return list;
      }

      public static IMaybe<TResult> FirstOrNoneAs<T, TResult>(this IEnumerable<T> enumerable)
      {
         return enumerable.FirstOrNone(i => i is TResult).CastAs<TResult>();
      }

      public static IResult<TResult> FirstOrFailAs<T, TResult>(this IEnumerable<T> enumerable)
      {
         return enumerable.FirstOrFail(i => i is TResult).CastAs<TResult>();
      }

      public static IMatched<TResult> FirstOrNotMatchedAs<T, TResult>(this IEnumerable<T> enumerable)
      {
         return enumerable.FirstOrNotMatched(i => i is TResult).CastAs<TResult>();
      }

      public static bool AtLeastOne<T>(this IEnumerable<T> enumerable) => enumerable.FirstOrNone().HasValue;

      public static bool AtLeastOne<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate) => enumerable.FirstOrNone(predicate).HasValue;

      public static IEnumerable<T> Do<T>(this IEnumerable<T> enumerable, Action<T> action)
      {
         foreach (var value in enumerable)
         {
            action(value);
            yield return value;
         }
      }

      public static IEnumerable<T> DoIf<T>(this IEnumerable<T> enumerable, Predicate<T> predicate, Action<T> action)
      {
         foreach (var value in enumerable)
         {
            if (predicate(value))
            {
               action(value);
            }

            yield return value;
         }
      }

      public static IEnumerable<T> DoIfElse<T>(this IEnumerable<T> enumerable, Predicate<T> predicate, Action<T> ifTrue, Action<T> ifFalse)
      {
         foreach (var value in enumerable)
         {
            if (predicate(value))
            {
               ifTrue(value);
            }
            else
            {
               ifFalse(value);
            }

            yield return value;
         }
      }

      public static bool AllMatch<T1, T2>(this IEnumerable<T1> leftEnumerable, IEnumerable<T2> rightEnumerable, Func<T1, T2, bool> matcher,
         bool mustBeSameLength = true)
      {
         var left = leftEnumerable.ToArray();
         var right = rightEnumerable.ToArray();

         if (mustBeSameLength && left.Length != right.Length)
         {
            return false;
         }

         foreach (var leftItem in left)
         {
            if (!right.AtLeastOne(r => matcher(leftItem, r)))
            {
               return false;
            }
         }

         return true;
      }

      public static IEnumerable<(T1, IMaybe<T2>)> AllMatched<T1, T2>(this IEnumerable<T1> leftEnumerable, IEnumerable<T2> rightEnumerable,
         Func<T1, T2, bool> matcher)
      {
         var rightArray = rightEnumerable.ToArray();
         foreach (var left in leftEnumerable)
         {
            yield return (left, rightArray.FirstOrNone(r => matcher(left, r)));
         }
      }
   }
}