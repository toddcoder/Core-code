using System;
using System.Collections.Generic;
using System.Linq;
using Core.Arrays;
using Core.Dates.DateIncrements;
using Core.Exceptions;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Dates
{
   public static class TimeSpanExtensions
   {
      private const string REGEX_TIMER_INTERVAL = "/(/d+) /s+ /(('milli')? 'sec' ('ond')? 's'? | 'min' ('ute')? 's'? | " +
         "'h' ('ou')? 'r' 's'? | 'days'?); f";

      public static string ToLongString(this TimeSpan span, bool includeMilliseconds) => Time.ToLongString(span, includeMilliseconds);

      public static string ToString(this TimeSpan span, bool includeMilliseconds)
      {
         var list = new List<string>();

         if (span.Days > 0)
         {
            list.Add(span.Days == 1 ? "1 day" : $"{span.Days} days");
         }

         if (span.Hours > 0)
         {
            list.Add(span.Hours == 1 ? "1 hour" : $"{span.Hours} hours");
         }

         if (span.Minutes > 0)
         {
            list.Add(span.Minutes == 1 ? "1 minute" : $"{span.Minutes} minutes");
         }

         if (span.Seconds > 0)
         {
            if (includeMilliseconds && span.Milliseconds > 0)
            {
               list.Add($"{span.Seconds}.{span.Milliseconds:D3} seconds");
            }
            else
            {
               list.Add(span.Seconds == 1 ? "1 second" : $"{span.Seconds} seconds");
            }
         }
         else if (includeMilliseconds && span.Milliseconds > 0)
         {
            list.Add($"0.{span.Milliseconds:D3} seconds");
         }
         else
         {
            list.Add("0 seconds");
         }

         return list.ToArray().Andify();
      }

      public static string ToShortString(this TimeSpan span, bool includeMilliseconds)
      {
         return Time.ToShortString(span, includeMilliseconds);
      }

      public static TimeSpan ToTimeSpan(this string source) => source.TimeSpan().Recover(_ => 1.Second());

      public static TimeSpan ToTimeSpan(this string source, TimeSpan defaultValue) => source.TimeSpan().Recover(_ => defaultValue);

      public static Maybe<TimeSpan> AsTimeSpan(this string source)
      {
         var intervals = source.Split("/s* (',' | 'and') /s*; f");
         var spans = intervals.Where(i => i.IsNotEmpty()).Select(getSpan);
         var newSpan = new TimeSpan(0, 0, 0, 0);

         foreach (var span in spans)
         {
            if (span.If(out var timeSpan))
            {
               newSpan = newSpan.Add(timeSpan);
            }
            else
            {
               return none<TimeSpan>();
            }
         }

         return newSpan.Some();
      }

      public static Result<TimeSpan> TimeSpan(this string source)
      {
         var intervals = source.Split("/s* (',' | 'and') /s*; f");
         var spans = intervals.Where(i => i.IsNotEmpty()).Select(getSpan);
         var newSpan = new TimeSpan(0, 0, 0, 0);

         foreach (var span in spans)
         {
            if (span.ValueOrOriginal(out var timeSpan, out var original))
            {
               newSpan = newSpan.Add(timeSpan);
            }
            else
            {
               return original;
            }
         }

         return newSpan.Success();
      }

      private static Result<TimeSpan> getSpan(string source)
      {
         return
            from result in source.Matches(REGEX_TIMER_INTERVAL).Result($"Can't match {source}")
            from span in getSpan(result)
            select span;
      }

      private static Result<TimeSpan> getSpan(MatchResult result)
      {
         var value = result.FirstGroup;
         var unit = result.SecondGroup;

         return
            from intValue in value.Int32()
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
                  throw $"Couldn't determine unit from {unit}".Throws();
               }
            })
            select span;
      }
   }
}