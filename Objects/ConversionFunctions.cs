using System;
using System.Linq;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Objects
{
   public static class ConversionFunctions
   {
      public static class Value
      {
         public static bool Boolean(string source, bool defaultValue = false)
         {
            if (source.IsNotEmpty())
            {
               return source switch
               {
                  "1" => true,
                  "0" => false,
                  _ => bool.TryParse(source, out var result) ? result : defaultValue
               };
            }
            else
            {
               return defaultValue;
            }
         }

         public static byte Byte(string source, byte defaultValue = 0)
         {
            if (source.IsNotEmpty())
            {
               return byte.TryParse(source, out var result) ? result : defaultValue;
            }
            else
            {
               return defaultValue;
            }
         }

         public static int Int32(string source, int defaultValue = 0)
         {
            if (source.IsNotEmpty())
            {
               return int.TryParse(source, out var result) ? result : defaultValue;
            }
            else
            {
               return defaultValue;
            }
         }

         public static long Int64(string source, long defaultValue = 0)
         {
            if (source.IsNotEmpty())
            {
               return long.TryParse(source, out var result) ? result : defaultValue;
            }
            else
            {
               return defaultValue;
            }
         }

         public static float Single(string source, float defaultValue = 0)
         {
            if (source.IsNotEmpty())
            {
               return float.TryParse(source, out var result) ? result : defaultValue;
            }
            else
            {
               return defaultValue;
            }
         }

         public static double Double(string source, double defaultValue = 0)
         {
            if (source.IsNotEmpty())
            {
               return double.TryParse(source, out var result) ? result : defaultValue;
            }
            else
            {
               return defaultValue;
            }
         }

         public static decimal Decimal(string source, decimal defaultValue = 0)
         {
            if (source.IsNotEmpty())
            {
               return decimal.TryParse(source, out var result) ? result : defaultValue;
            }
            else
            {
               return defaultValue;
            }
         }

         public static DateTime DateTime(string source, DateTime defaultValue)
         {
            if (source.IsNotEmpty())
            {
               return System.DateTime.TryParse(source, out var result) ? result : defaultValue;
            }
            else
            {
               return defaultValue;
            }
         }

         public static DateTime DateTime(string source) => DateTime(source, System.DateTime.MaxValue);

         public static Guid Guid(string source)
         {
            if (source.IsNotEmpty())
            {
               return System.Guid.TryParse(source, out var guid) ? guid : System.Guid.Empty;
            }
            else
            {
               return System.Guid.Empty;
            }
         }

         public static T Enumeration<T>(string source, bool ignoreCase = true, T defaultValue = default) where T : Enum
         {
            try
            {
               return (T)Enum.Parse(typeof(T), source, ignoreCase);
            }
            catch
            {
               return defaultValue;
            }
         }

         public static object Enumeration(Type type, string source, bool ignoreCase = true) => Enum.Parse(type, source, ignoreCase);

         public static TimeSpan TimeSpan(string source, TimeSpan defaultValue) => getSpans(source).Recover(_ => defaultValue);

         public static TimeSpan TimeSpan(string source) => getSpans(source).ForceValue();

         public static T Cast<T>(object obj, Func<string> message)
         {
            try
            {
               return (T)obj;
            }
            catch (Exception exception)
            {
               var formatter = new Formatter { ["object"] = obj?.ToString() ?? "", ["e"] = exception.Message };
               throw new ApplicationException(formatter.Format(message()));
            }
         }

         public static T Cast<T>(object obj) => (T)obj;
      }

      public static class Maybe
      {
         public static Maybe<bool> Boolean(string source)
         {
            if (source.IsNotEmpty())
            {
               return source switch
               {
                  "1" => true,
                  "0" => false,
                  _ => bool.TryParse(source, out var result) ? result : nil
               };
            }
            else
            {
               return nil;
            }
         }

         public static Maybe<byte> Byte(string source)
         {
            if (source.IsNotEmpty())
            {
               return byte.TryParse(source, out var result) ? result : nil;
            }
            else
            {
               return nil;
            }
         }

         public static Maybe<int> Int32(string source)
         {
            if (source.IsNotEmpty())
            {
               return int.TryParse(source, out var result) ? result : nil;
            }
            else
            {
               return nil;
            }
         }

         public static Maybe<long> Int64(string source)
         {
            if (source.IsNotEmpty())
            {
               return long.TryParse(source, out var result) ? result : nil;
            }
            else
            {
               return nil;
            }
         }

         public static Maybe<float> Single(string source)
         {
            if (source.IsNotEmpty())
            {
               return float.TryParse(source, out var result) ? result : nil;
            }
            else
            {
               return nil;
            }
         }

         public static Maybe<double> Double(string source)
         {
            if (source.IsNotEmpty())
            {
               return double.TryParse(source, out var result) ? result : nil;
            }
            else
            {
               return nil;
            }
         }

         public static Maybe<decimal> Decimal(string source)
         {
            if (source.IsNotEmpty())
            {
               return decimal.TryParse(source, out var result) ? result : nil;
            }
            else
            {
               return nil;
            }
         }

         public static Maybe<DateTime> DateTime(string source)
         {
            if (source.IsNotEmpty())
            {
               return System.DateTime.TryParse(source, out var result) ? result : nil;
            }
            else
            {
               return nil;
            }
         }

         public static Maybe<Guid> Guid(string source)
         {
            if (source.IsNotEmpty())
            {
               return System.Guid.TryParse(source, out var guid) ? guid : nil;
            }
            else
            {
               return nil;
            }
         }

         public static Maybe<T> Enumeration<T>(string source, bool ignoreCase = true) where T : struct, Enum
         {
            try
            {
               return Enum.TryParse<T>(source, ignoreCase, out var result) ? result : nil;
            }
            catch
            {
               return nil;
            }
         }

         public static Maybe<object> Enumeration(Type type, string source, bool ignoreCase = true)
         {
            try
            {
               return Enum.Parse(type, source, ignoreCase);
            }
            catch
            {
               return nil;
            }
         }

         public static Maybe<TimeSpan> TimeSpan(string source) => getSpans(source).Maybe();

         public static Maybe<T> Cast<T>(object obj)
         {
            if (obj is T cast)
            {
               return cast;
            }
            else
            {
               return nil;
            }
         }
      }

      public static class Result
      {
         public static Result<bool> Boolean(string source) => tryTo(() => bool.Parse(source));

         public static Result<byte> Byte(string source) => tryTo(() => byte.Parse(source));

         public static Result<int> Int32(string source) => tryTo(() => int.Parse(source));

         public static Result<long> Int64(string source) => tryTo(() => long.Parse(source));

         public static Result<float> Single(string source) => tryTo(() => float.Parse(source));

         public static Result<double> Double(string source) => tryTo(() => double.Parse(source));

         public static Result<decimal> Decimal(string source) => tryTo(() => decimal.Parse(source));

         public static Result<DateTime> DateTime(string source) => tryTo(() => System.DateTime.Parse(source));

         public static Result<Guid> Guid(string source) => tryTo(() => System.Guid.Parse(source));

         public static Result<T> Enumeration<T>(string source, bool ignoreCase = true) where T : struct, Enum
         {
            return tryTo(() => (T)Enum.Parse(typeof(T), source, ignoreCase));
         }

         public static Result<object> Enumeration(Type type, string source, bool ignoreCase = true)
         {
            return tryTo(() => Enum.Parse(type, source, ignoreCase));
         }

         public static Result<TimeSpan> TimeSpan(string source) => getSpans(source);

         public static Result<T> Cast<T>(object obj)
         {
            try
            {
               return obj switch
               {
                  null => fail($"Argument is null; can't be cast to {typeof(T).FullName}"),
                  T o => o,
                  _ => fail($"{obj} can't be cast to {typeof(T).FullName}")
               };
            }
            catch (Exception exception)
            {
               return exception;
            }
         }
      }

      private const string REGEX_TIMER_INTERVAL = "/(/d+) /s+ /(('milli')? 'sec' ('ond')? 's'? | 'min' ('ute')? 's'? | " +
         "'h' ('ou')? 'r' 's'? | 'days'?); f";

      private static Result<TimeSpan> getSpan(string source)
      {
         return
            from result in source.Matches(REGEX_TIMER_INTERVAL).Result($"Can't match {source}")
            from span in getSpan(result)
            select span;
      }

      private static Result<TimeSpan> getSpans(string source)
      {
         var intervals = source.Unjoin("/s* (',' | 'and') /s*; f");
         var spans = intervals.Where(i => i.IsNotEmpty()).Select(getSpan);
         var newSpan = new TimeSpan(0, 0, 0, 0);

         foreach (var span in spans)
         {
            if (span.Map(out var timeSpan, out var exception))
            {
               newSpan = newSpan.Add(timeSpan);
            }
            else
            {
               return exception;
            }
         }

         return newSpan;
      }

      private static Result<TimeSpan> getSpan(MatchResult result)
      {
         var value = result.FirstGroup;
         var unit = result.SecondGroup;

         return
            from intValue in Result.Int32(value)
            from span in tryTo(() =>
            {
               if (unit.IsMatch("'millisec' ('ond')? 's'?; f"))
               {
                  return new TimeSpan(0, 0, 0, 0, intValue);
               }
               else if (unit.IsMatch("'sec' ('ond') 's'?; f"))
               {
                  return new TimeSpan(0, 0, 0, intValue, 0);
               }
               else if (unit.IsMatch("'min' ('ute')? 's'?; f"))
               {
                  return new TimeSpan(0, intValue, 0);
               }
               else if (unit.IsMatch("'h' ('ou')? 'r' 's'?; f"))
               {
                  return new TimeSpan(0, intValue, 0, 0);
               }
               else if (unit.IsMatch("'days'?; f"))
               {
                  return new TimeSpan(intValue, 0, 0, 0);
               }
               else
               {
                  throw fail($"Couldn't determine unit from {unit}");
               }
            })
            select span;
      }
   }
}