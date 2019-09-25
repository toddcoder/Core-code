using System;
using System.Linq;
using Core.Dates.DateIncrements;
using Core.Exceptions;
using Core.Monads;
using Core.RegularExpressions;
using Core.Strings;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Dates
{
   public static class TimeSpanExtensions
   {
      const string REGEX_TIMER_INTERVAL = "/(/d+) /s+ /(('milli')? 'sec' ('ond')? 's'? | 'min' ('ute')? 's'? | " +
         "'h' ('ou')? 'r' 's'? | 'days'?)";

      public static string ToLongString(this TimeSpan span, bool includeMilliseconds) => Time.ToLongString(span, includeMilliseconds);

      public static string ToShortString(this TimeSpan span, bool includeMilliseconds)
      {
         return Time.ToShortString(span, includeMilliseconds);
      }

      public static TimeSpan ToTimeSpan(this string source) => source.TimeSpan().Recover(e => 1.Second());

      public static TimeSpan ToTimeSpan(this string source, TimeSpan defaultValue) => source.TimeSpan().Recover(e => defaultValue);

      public static IMaybe<TimeSpan> AsTimeSpan(this string source)
      {
         var intervals = source.Split("/s* (',' | 'and') /s*");
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

      public static IResult<TimeSpan> TimeSpan(this string source)
      {
         var intervals = source.Split("/s* (',' | 'and') /s*");
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

      static IResult<TimeSpan> getSpan(string source) =>
         from matcher in source.Matches(REGEX_TIMER_INTERVAL).Result($"Can't match {source}")
         from span in getSpan(matcher)
         select span;

      static IResult<TimeSpan> getSpan(Matcher matcher)
      {
         var value = matcher.FirstGroup;
         var unit = matcher.SecondGroup;

         return
            from intValue in value.Int32()
            from span in tryTo(() =>
            {
               if (unit.IsMatch("'millisec' ('ond')? 's'?"))
               {
                  return new TimeSpan(0, 0, 0, 0, intValue);
               }
               else if (unit.IsMatch("'sec' ('ond') 's'?"))
               {
                  return new TimeSpan(0, 0, 0, intValue, 0);
               }
               else if (unit.IsMatch("'min' ('ute')? 's'?"))
               {
                  return new TimeSpan(0, intValue, 0);
               }
               else if (unit.IsMatch("'h' ('ou')? 'r' 's'?"))
               {
                  return new TimeSpan(0, intValue, 0, 0);
               }
               else if (unit.IsMatch("'days'?"))
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