using System;
using System.Threading;
using Core.Arrays;
using Core.Dates.Now;
using Core.Matching;
using Core.Monads;
using Core.Numbers;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ConversionFunctions;

namespace Core.Dates
{
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
            if (Year.If(out var year, out var exception) && Month.If(out var month, out exception) &&
               Day.If(out var day, out exception))
            {
               return valid(year, month, day);
            }
            else
            {
               return exception;
            }
         }

         public Result<ValidDate> AndYear(int year) => (Month.IsSuccessful || Day.IsSuccessful).Result(() =>
         {
            Year = year;
            return this;
         }, "Month or day must be set");

         public Result<DateTime> AndYearValid(int year)
         {
            Year = year;
            return validate();
         }

         public Result<ValidDate> AndMonth(int month) => (Year.IsSuccessful || Day.IsSuccessful).Result(() =>
         {
            Month = month;
            return this;
         }, "Year or day must be set");

         public Result<DateTime> AndMonthValid(int month)
         {
            Month = month;
            return validate();
         }

         public Result<ValidDate> AndDay(int day) => (Year.IsSuccessful || Month.IsSuccessful).Result(() =>
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
         if (pattern.IsMatch("^ ['//|'] /s* ['//|'] /s* ['//|'] $; f"))
         {
            return date;
         }
         else
         {
            if (pattern.Matches("/(['+-']? /d*) ['//|'] /(['+-']? /d*) ['//|'] /(['+-']? /d*); f").If(out var result))
            {
               var match = result.GetMatch(0);
               if (match.Groups.Assign(out _, out var year, out var month, out var day).IsSuccessful)
               {
                  DateIncrementer builder = date;
                  if (year.Length > 0)
                  {
                     var yearText = year.Text;
                     var yearAmount = Value.Int32(yearText);
                     if (yearText.IsMatch(REGEX_SIGN))
                     {
                        builder.Year += yearAmount;
                     }
                     else
                     {
                        var setYearResult = builder.SetYear(yearAmount);
                        if (setYearResult.IsFailed)
                        {
                           return setYearResult;
                        }
                     }
                  }

                  if (month.Length > 0)
                  {
                     var monthText = month.Text;
                     var monthAmount = Value.Int32(monthText);
                     if (monthText.IsMatch(REGEX_SIGN))
                     {
                        builder.Month += monthAmount;
                     }
                     else
                     {
                        var setMonthResult = builder.SetMonth(monthAmount);
                        if (setMonthResult.IsFailed)
                        {
                           return setMonthResult;
                        }
                     }
                  }

                  if (day.Length > 0)
                  {
                     var dayText = day.Text;
                     var dayAmount = Value.Int32(dayText);
                     if (dayText.IsMatch(REGEX_SIGN))
                     {
                        builder.Day += dayAmount;
                     }
                     else if (dayAmount == 0)
                     {
                        var setDayResult = builder.SetToLastDay();
                        if (setDayResult.IsFailed)
                        {
                           return setDayResult;
                        }
                     }
                     else
                     {
                        var setDayResult = builder.SetDay(dayAmount);
                        if (setDayResult.IsFailed)
                        {
                           return setDayResult;
                        }
                     }
                  }

                  return builder.Date;
               }

               return fail($"Couldn't extract date elements from {pattern}");
            }
            else
            {
               return fail($"Didn't understand {pattern}");
            }
         }
      }

      public static void Sleep(this TimeSpan interval) => Thread.Sleep(interval);

      public static bool OldEnough(this DateTime date, TimeSpan age) => NowServer.Now - date >= age;

      public static string DescriptionFromNow(this DateTime date)
      {
         var today = NowServer.Today;
         var dateOnly = date.Truncate();
         var difference = (int)today.Subtract(dateOnly).TotalDays;

         static int dayOfWeek(DateTime dateTime) => (int)dateTime.DayOfWeek;

         return difference switch
         {
            0 => "Today",
            1 => "Yesterday",
            <= 7 when dayOfWeek(today) > dayOfWeek(dateOnly) => dateOnly.DayOfWeek.ToString(),
            <= 7 => $"Last {dateOnly.DayOfWeek}",
            _ when dateOnly.Year == today.Year => dateOnly.ToString("MMMM d"),
            _ => dateOnly.ToString("MMMM d, yyyy")
         };
      }
   }
}