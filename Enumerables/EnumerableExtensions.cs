using System;
using System.Collections.Generic;
using System.Linq;
using Core.Arrays;
using Core.Monads;
using Core.Numbers;
using Core.Objects;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Enumerables
{
   public static class EnumerableExtensions
   {
      public static string Stringify<T>(this IEnumerable<T> enumerable, string connector = ", ")
      {
         return string.Join(connector ?? "", enumerable);
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

      public static IEnumerable<int> UpUntil(this int from, int to, int by = 1)
      {
         for (var i = from; i < to; i += by)
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

      public static IEnumerable<int> DownUntil(this int from, int to, int by = -1)
      {
         for (var i = from; i > to; i += by)
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

      public static IResult<T> FirstOrFail<T>(this IEnumerable<T> enumerable, string failureMessage = "Default value") => tryTo(() =>
      {
         try
         {
            return enumerable.First().Success();
         }
         catch
         {
            return failureMessage.Failure<T>();
         }
      });

      public static IResult<T> FirstOrFail<T>(this IEnumerable<T> enumerable, Func<string> failureMessage) => tryTo(() =>
      {
         try
         {
            return enumerable.First().Success();
         }
         catch
         {
            return failureMessage().Failure<T>();
         }
      });

      public static IResult<T> FirstOrFail<T>(this IEnumerable<T> enumerable, Func<Exception, string> failureMessage) => tryTo(() =>
      {
         try
         {
            return enumerable.First().Success();
         }
         catch (Exception exception)
         {
            return failureMessage(exception).Failure<T>();
         }
      });

      public static IResult<T> LastOrFail<T>(this IEnumerable<T> enumerable, string failureMessage = "Default value") => tryTo(() =>
      {
         try
         {
            return enumerable.Last().Success();
         }
         catch
         {
            return failureMessage.Failure<T>();
         }
      });

      public static IResult<T> LastOrFail<T>(this IEnumerable<T> enumerable, Func<string> failureMessage) => tryTo(() =>
      {
         try
         {
            return enumerable.Last().Success();
         }
         catch
         {
            return failureMessage().Failure<T>();
         }
      });

      public static IResult<T> LastOrFail<T>(this IEnumerable<T> enumerable, Func<Exception, string> failureMessage) => tryTo(() =>
      {
         try
         {
            return enumerable.Last().Success();
         }
         catch (Exception exception)
         {
            return failureMessage(exception).Failure<T>();
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
   }
}