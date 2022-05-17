using System;
using Core.Dates;
using Core.Dates.Now;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Services.Scheduling
{
   public class ScheduleIncrement
   {
      protected int year;
      protected int month;
      protected Maybe<int> _day;
      protected DayOfWeek dayOfWeek;
      protected int hour;
      protected int minute;
      protected int second;

      internal ScheduleIncrement(int year, int month, int hour, int minute, int second)
      {
         this.year = year;
         this.month = month;
         this.hour = hour;
         this.minute = minute;
         this.second = second;
      }

      public ScheduleIncrement(int year, int month, int day, int hour, int minute, int second) : this(year, month, hour, minute, second)
      {
         _day = day;
      }

      public ScheduleIncrement(int year, int month, DayOfWeek dayOfWeek, int hour, int minute, int second) : this(year, month, hour, minute, second)
      {
         _day = nil;
         this.dayOfWeek = dayOfWeek;
      }

      protected static int moveToEndOfMonthIf(int day, int year, int month) => day == 0 ? DateTime.DaysInMonth(year, month) : day;

      public Result<DateTime> Next
      {
         get
         {
            DateIncrementer now = NowServer.Now;

            if (_day.Map(out var day))
            {
               if (year > 0)
               {
                  var current = now.Clone();
                  var _target =
                     from setMonth in current.SetMonth(month)
                     let dayToUse = moveToEndOfMonthIf(day, current.Year, current.Month)
                     from setDay in current.SetDay(dayToUse)
                     from setHour in current.SetHour(hour)
                     from setMinute in current.SetMinute(minute)
                     select current;
                  if (_target.Map(out var target, out var exception))
                  {
                     if (target <= now)
                     {
                        target.Year += year;
                        var dayToUse = moveToEndOfMonthIf(day, target.Year, target.Month);
                        target.SetDay(dayToUse);
                     }

                     return target.Date;
                  }
                  else
                  {
                     return exception;
                  }
               }
               else if (month > 0)
               {
                  var current = now.Clone();
                  var dayToUse = moveToEndOfMonthIf(day, current.Year, current.Month);
                  var _target=
                     from setDay in current.SetDay(dayToUse)
                     from setHour in current.SetHour(hour)
                     from setMinute in current.SetMinute(minute)
                     select current;
                  if (_target.Map(out var target, out var exception))
                  {
                     if (target <= now)
                     {
                        target.Month += month;
                        dayToUse = moveToEndOfMonthIf(day, target.Year, target.Month);
                        target.SetDay(dayToUse);
                     }

                     return target.Date;
                  }
                  else
                  {
                     return exception;
                  }
               }
               else if (day > 0)
               {
                  var current = now.Clone();
                  var _target=
                     from setHour in current.SetHour(hour)
                     from setMinute in current.SetMinute(minute)
                     select current;
                  if (_target.Map(out var target, out var exception))
                  {
                     if (target <= now)
                     {
                        target.Day += day;
                     }

                     return target.Date;
                  }
                  else
                  {
                     return exception;
                  }
               }
               else if (hour > 0)
               {
                  var target = now.Clone();
                  if (target.SetMinute(minute).Map(out _, out var exception))
                  {
                     if (target <= now)
                     {
                        target.Hour += hour;
                     }

                     return target.Date;
                  }
                  else
                  {
                     return exception;
                  }
               }
               else if (minute > 0)
               {
                  var target = now.Clone();
                  target.Minute += minute;
                  return target.Date;
               }
               else if (second > 0)
               {
                  var target = now.Clone();
                  target.Second += second;
                  return target.Date;
               }
               else
               {
                  return fail("Internal error: Values in schedule can't be all 0");
               }
            }
            else
            {
               while (now.Date.DayOfWeek != dayOfWeek)
               {
                  now.Day += 1;
               }

               return now.Date;
            }
         }
      }
   }
}