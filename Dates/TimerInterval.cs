using System;
using System.Threading;
using Core.Exceptions;
using Core.RegularExpressions;
using Core.Strings;

namespace Core.Dates
{
   [Obsolete("Use TimeSpan")]
   public class TimerInterval
   {
      const string PATTERN_TIMER_INTERVAL = "/(/d+) /s+ /(('milli')? 'sec' ('ond')? 's'? | 'min' ('ute')? 's'? | " +
         "'h' ('ou')? 'r' 's'?)";

      public static bool IsTimerIntervalString(string expression) => expression.IsMatch(PATTERN_TIMER_INTERVAL);

      public static implicit operator TimerInterval(string expression)
      {
         var matcher = new Matcher();

         matcher.RequiredMatch(expression, PATTERN_TIMER_INTERVAL, $"Couldn't determine value from \"{expression}\"");

         var value = matcher[0, 1];
         var unit = matcher[0, 2];

         var intValue = value.ToInt();
         var plural = intValue != 1;

         if (unit.IsMatch("'millisec' ('ond')? 's'?"))
            return getInterval(intValue, plural, IntervalUnit.Millisecond, IntervalUnit.Milliseconds);
         else if (unit.IsMatch("'sec' ('ond') 's'?"))
            return getInterval(intValue, plural, IntervalUnit.Second, IntervalUnit.Seconds);
         else if (unit.IsMatch("'min' ('ute')? 's'?"))
            return getInterval(intValue, plural, IntervalUnit.Minute, IntervalUnit.Minutes);
         else if (unit.IsMatch("'h' ('ou')? 'r' 's'?"))
            return getInterval(intValue, plural, IntervalUnit.Hour, IntervalUnit.Hours);
         else
            throw "Couldn't determine value or unit from \"{expression}\"".Throws();
      }

      static TimerInterval getInterval(int value, bool plural, IntervalUnit single, IntervalUnit moreThanOne)
      {
         return new TimerInterval(value, plural ? moreThanOne : single);
      }

      public static explicit operator TimerInterval(TimeSpan value)
      {
         return new TimerInterval((int)value.TotalMilliseconds,
            value.TotalMilliseconds == 1 ? IntervalUnit.Millisecond : IntervalUnit.Milliseconds);
      }

      public static implicit operator TimeSpan(TimerInterval value) => new TimeSpan(0, 0, (int)value.IntervalAsSeconds);

      public static implicit operator int(TimerInterval interval) => interval.Interval;

      public TimerInterval(int value, IntervalUnit unit)
      {
         Value = value;
         Unit = unit;
      }

      public TimerInterval() : this(0, IntervalUnit.Milliseconds) { }

      public int Value { get; }

      public IntervalUnit Unit { get; }

      public int Interval => getInterval();

      public double IntervalAsSeconds => getIntervalAsSeconds();

      static int getInterval(int value, IntervalUnit unit)
      {
         while (true)
            switch (unit)
            {
               case IntervalUnit.Millisecond:
               case IntervalUnit.Milliseconds:
                  return value;
               case IntervalUnit.Second:
               case IntervalUnit.Seconds:
                  value = value * 1000;
                  unit = IntervalUnit.Milliseconds;
                  continue;
               case IntervalUnit.Minute:
               case IntervalUnit.Minutes:
                  value = value * 60;
                  unit = IntervalUnit.Seconds;
                  continue;
               case IntervalUnit.Hour:
               case IntervalUnit.Hours:
                  value = value * 60;
                  unit = IntervalUnit.Minutes;
                  continue;
               default:
                  throw $"Didn't understand unit {unit}".Throws();
            }
      }

      int getInterval() => getInterval(Value, Unit);

      static double getIntervalAsSeconds(double value, IntervalUnit unit)
      {
         while (true)
            switch (unit)
            {
               case IntervalUnit.Millisecond:
               case IntervalUnit.Milliseconds:
                  value = value / 1000;
                  unit = IntervalUnit.Second;
                  continue;
               case IntervalUnit.Second:
               case IntervalUnit.Seconds:
                  return value;
               case IntervalUnit.Minute:
               case IntervalUnit.Minutes:
                  value = value * 60;
                  unit = IntervalUnit.Seconds;
                  continue;
               case IntervalUnit.Hour:
               case IntervalUnit.Hours:
                  value = value * 60;
                  unit = IntervalUnit.Minutes;
                  continue;
               default:
                  throw $"Didn't understand unit {unit}".Throws();
            }
      }

      double getIntervalAsSeconds() => getIntervalAsSeconds(Value, Unit);

      public override string ToString()
      {
         switch (Unit)
         {
            case IntervalUnit.Millisecond:
            case IntervalUnit.Milliseconds:
               return Value == 1 ? "1 millisecond" : $"{Value} milliseconds";
            case IntervalUnit.Second:
            case IntervalUnit.Seconds:
               return Value == 1 ? "1 second" : $"{Value} seconds";
            case IntervalUnit.Minute:
            case IntervalUnit.Minutes:
               return Value == 1 ? "1 minute" : $"{Value} minutes";
            case IntervalUnit.Hour:
            case IntervalUnit.Hours:
               return Value == 1 ? "1 hour" : $"{Value} hours";
            default:
               return "";
         }
      }

      public void Sleep() => Thread.Sleep(getInterval());
   }
}