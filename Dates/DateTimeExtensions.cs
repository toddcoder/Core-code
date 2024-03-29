﻿using System;
using System.Threading;
using Core.Dates.Now;
using Core.Matching;
using Core.Monads;
using Core.Numbers;
using Core.Strings;
using static Core.Monads.Lazy.LazyMonads;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ConversionFunctions;

namespace Core.Dates;

public static class DateTimeExtensions
{
   private const string REGEX_SIGN = "^ ['+-']; f";

   public class ValidDate
   {
      protected static Result<DateTime> valid(int year, int month, int day)
      {
         if (year <= 0)
         {
            return fail("Year must be > 0");
         }
         else if (!month.Between(1).And(12))
         {
            return fail("Month must be between 1 and 12");
         }
         else
         {
            return (day <= month.LastOfMonth(year)).Result(() => new DateTime(year, month, day),
               $"{month:d.2}/{day:d.2}/{year:d.4} is an invalid date");
         }
      }

      public Result<int> Year { get; set; }

      public Result<int> Month { get; set; }

      public Result<int> Day { get; set; }

      public ValidDate()
      {
         Year = fail("Year not set");
         Month = fail("Month not set");
         Day = fail("Day not set");
      }

      protected Result<DateTime> validate()
      {
         return
            from year in Year
            from month in Month
            from day in Day
            select valid(year, month, day);
      }

      public Result<ValidDate> AndYear(int year) => ((bool)Month || (bool)Day).Result(() =>
      {
         Year = year;
         return this;
      }, "Month or day must be set");

      public Result<DateTime> AndYearValid(int year)
      {
         Year = year;
         return validate();
      }

      public Result<ValidDate> AndMonth(int month) => ((bool)Year || (bool)Day).Result(() =>
      {
         Month = month;
         return this;
      }, "Year or day must be set");

      public Result<DateTime> AndMonthValid(int month)
      {
         Month = month;
         return validate();
      }

      public Result<ValidDate> AndDay(int day) => ((bool)Year || (bool)Month).Result(() =>
      {
         Day = day;
         return this;
      }, "Year or month must be set");

      public Result<DateTime> AndDayValid(int day)
      {
         Day = day;
         return validate();
      }
   }

   public static ValidDate IsYear(this int year) => new() { Year = year };

   public static Result<ValidDate> AndYear(this Result<ValidDate> validDate, int year)
   {
      return validDate.Map(vd => vd.AndYear(year));
   }

   public static Result<DateTime> AndYearValid(this Result<ValidDate> validDate, int year)
   {
      return validDate.Map(vd => vd.AndYearValid(year));
   }

   public static ValidDate IsMonth(this int month) => new() { Month = month };

   public static Result<ValidDate> AndMonth(this Result<ValidDate> validDate, int month)
   {
      return validDate.Map(vd => vd.AndMonth(month));
   }

   public static Result<DateTime> AndMonthValid(this Result<ValidDate> validDate, int month)
   {
      return validDate.Map(vd => vd.AndMonthValid(month));
   }

   public static ValidDate IsDay(this int day) => new() { Day = day };

   public static Result<ValidDate> AndDay(this Result<ValidDate> validDate, int day) => validDate.Map(vd => vd.AndDay(day));

   public static Result<DateTime> AndDayValid(this Result<ValidDate> validDate, int day)
   {
      return validDate.Map(vd => vd.AndDayValid(day));
   }

   public static DateTime FirstOfMonth(this DateTime date) => new(date.Year, date.Month, 1);

   public static DateTime LastOfMonth(this DateTime date) => date.FirstOfMonth().AddMonths(1).AddDays(-1);

   public static int LastOfMonth(this int month, int year) => new DateTime(year, month, 1).AddMonths(1).AddDays(-1).Day;

   public static DateTime NearestQuarterHour(this DateTime time)
   {
      var minutes = time.Minute;
      var quarter = minutes % 15;

      if (quarter is >= 0 and <= 7)
      {
         minutes -= quarter;
      }
      else
      {
         minutes += 15 - quarter;
      }

      DateTime result;
      if (minutes == 60)
      {
         minutes = 0;
         result = new DateTime(time.Year, time.Month, time.Day, time.Hour, minutes, 0, 0);
         result = result.AddHours(1);
      }
      else
      {
         result = new DateTime(time.Year, time.Month, time.Day, time.Hour, 0, 0);
      }

      return result;
   }

   public static DateTime Truncate(this DateTime date) => new(date.Year, date.Month, date.Day);

   public static Numbers.Comparison<DateTime> ComparedTo(this DateTime left, DateTime right) => new(left, right);

   public static DateEnumerator To(this DateTime beginDate, DateTime endDate) => new(beginDate, endDate);

   public static Result<string> MonthName(this int month) => month switch
   {
      1 => "January",
      2 => "February",
      3 => "March",
      4 => "April",
      5 => "May",
      6 => "June",
      7 => "July",
      8 => "August",
      9 => "September",
      10 => "October",
      11 => "November",
      12 => "December",
      _ => fail("Month must be in a range from 1 to 12")
   };

   public static Result<int> MonthNumber(this string name) => name.ToLower() switch
   {
      "january" or "jan" => 1,
      "february" or "feb" => 2,
      "march" or "mar" => 3,
      "april" or "apr" => 4,
      "may" => 5,
      "june" or "jun" => 6,
      "july" or "jul" => 7,
      "august" or "aug" => 8,
      "september" or "sep" => 9,
      "october" or "oct" => 10,
      "november" or "nov" => 11,
      "december" or "dec" => 12,
      _ => fail($"Didn't understand {name}")
   };

   public static Result<DateTime> RelativeTo(this DateTime date, string pattern)
   {
      var _result = lazy.maybe<MatchResult>();
      if (pattern.IsMatch("^ ['//|'] /s* ['//|'] /s* ['//|'] $; f"))
      {
         return date;
      }
      else if (_result.ValueOf(pattern.Matches("/(['+-']? /d*) ['//|'] /(['+-']? /d*) ['//|'] /(['+-']? /d*); f")) is (true, var (year, month, day)))
      {
         DateIncrementer builder = date;
         var _yearResult = lazy.result<DateTime>();
         var _monthResult = lazy.result<DateTime>();
         var _dayResult = lazy.result<DateTime>();
         if (year.IsNotEmpty())
         {
            var yearAmount = Value.Int32(year);
            if (year.IsMatch(REGEX_SIGN))
            {
               builder.Year += yearAmount;
            }
            else if (!_yearResult.ValueOf(builder.SetYear(yearAmount)))
            {
               return _yearResult;
            }
         }

         if (month.Length > 0)
         {
            var monthAmount = Value.Int32(month);
            if (month.IsMatch(REGEX_SIGN))
            {
               builder.Month += monthAmount;
            }
            else if (!_monthResult.ValueOf(builder.SetMonth(monthAmount)))
            {
               return _monthResult;
            }
         }

         if (day.Length > 0)
         {
            var dayAmount = Value.Int32(day);
            if (day.IsMatch(REGEX_SIGN))
            {
               builder.Day += dayAmount;
            }
            else if (dayAmount == 0)
            {
               if (!_dayResult.ValueOf(builder.SetToLastDay))
               {
                  return _dayResult;
               }
            }
            else if (!_dayResult.ValueOf(builder.SetDay(dayAmount)))
            {
               return _dayResult;
            }
         }

         return builder.Date;
      }
      else
      {
         return fail($"Couldn't extract date elements from {pattern}");
      }
   }

   public static void Sleep(this TimeSpan interval) => Thread.Sleep(interval);

   public static bool OldEnough(this DateTime date, TimeSpan age) => NowServer.Now - date >= age;

   private static Maybe<string> differenceInMinutes(int minutes) => minutes switch
   {
      0 => "Just now",
      < 60 => minutes.Plural("minute(s) ago"),
      _ => nil
   };

   private static Maybe<string> differenceInHours(int hours) => hours switch
   {
      < 24 => hours.Plural("hour(s) ago"),
      _ => nil
   };

   private static int dayOfWeek(DateTime dateTime) => (int)dateTime.DayOfWeek;

   private static string differenceInDays(int days, DateTime today, DateTime dateOnly) => days switch
   {
      1 => "Yesterday",
      <= 7 when dayOfWeek(today) > dayOfWeek(dateOnly) => dateOnly.DayOfWeek.ToString(),
      <= 7 => $"Last {dateOnly.DayOfWeek}",
      _ when dateOnly.Year == today.Year => dateOnly.ToString("MMMM d"),
      _ => dateOnly.ToString("MMMM d, yyyy")
   };

   public static string DescriptionBetweenDates(this DateTime date1, DateTime date2)
   {
      DateTime date;
      DateTime now;
      if (date1 < date2)
      {
         date = date1;
         now = date2;
      }
      else
      {
         date = date2;
         now = date1;
      }

      var dateOnly = date.Truncate();
      var minuteDifference = new Lazy<int>(() => (int)now.Subtract(date).TotalMinutes);
      var _minutes = lazy.maybe<string>();
      var hourDifference = new Lazy<int>(() => (int)now.Subtract(date).TotalHours);
      var _hours = lazy.maybe<string>();
      var dayDifference = new Lazy<int>(() => (int)now.Subtract(dateOnly).TotalDays);

      if (_minutes.ValueOf(differenceInMinutes(minuteDifference.Value)) is (true, var minutes))
      {
         return minutes;
      }
      else if (_hours.ValueOf(differenceInHours(hourDifference.Value)) is (true, var hours))
      {
         return hours;
      }
      else
      {
         return differenceInDays(dayDifference.Value, now, dateOnly);
      }
   }

   public static string DescriptionFromNow(this DateTime date)
   {
      var now = NowServer.Now;
      return date.DescriptionBetweenDates(now);
   }
}