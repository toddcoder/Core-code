using System;
using System.Threading;
using Core.Arrays;
using Core.Dates.Now;
using Core.Matching;
using Core.Monads;
using Core.Numbers;
using Core.Strings;

namespace Core.Dates
{
   public static class DateTimeExtensions
   {
      private const string REGEX_SIGN = "^ ['+-']; f";

      public class ValidDate
      {
         protected static IResult<DateTime> valid(int year, int month, int day)
         {
            if (year <= 0)
            {
               return "Year must be > 0".Failure<DateTime>();
            }
            else if (!month.Between(1).And(12))
            {
               return "Month must be between 1 and 12".Failure<DateTime>();
            }
            else
            {
               return (day <= month.LastOfMonth(year)).Result(() => new DateTime(year, month, day),
                  $"{month:d.2}/{day:d.2}/{year:d.4} is an invalid date");
            }
         }

         public IResult<int> Year { get; set; }

         public IResult<int> Month { get; set; }

         public IResult<int> Day { get; set; }

         public ValidDate()
         {
            Year = "Year not set".Failure<int>();
            Month = "Month not set".Failure<int>();
            Day = "Day not set".Failure<int>();
         }

         protected IResult<DateTime> validate()
         {
            if (Year.ValueOrCast<DateTime>(out var year, out var original) && Month.ValueOrCast(out var month, out original) &&
               Day.ValueOrCast(out var day, out original))
            {
               return valid(year, month, day);
            }
            else
            {
               return original;
            }
         }

         public IResult<ValidDate> AndYear(int year) => (Month.IsSuccessful || Day.IsSuccessful).Result(() =>
         {
            Year = year.Success();
            return this;
         }, "Month or day must be set");

         public IResult<DateTime> AndYearValid(int year)
         {
            Year = year.Success();
            return validate();
         }

         public IResult<ValidDate> AndMonth(int month) => (Year.IsSuccessful || Day.IsSuccessful).Result(() =>
         {
            Month = month.Success();
            return this;
         }, "Year or day must be set");

         public IResult<DateTime> AndMonthValid(int month)
         {
            Month = month.Success();
            return validate();
         }

         public IResult<ValidDate> AndDay(int day) => (Year.IsSuccessful || Month.IsSuccessful).Result(() =>
         {
            Day = day.Success();
            return this;
         }, "Year or month must be set");

         public IResult<DateTime> AndDayValid(int day)
         {
            Day = day.Success();
            return validate();
         }
      }

      public static ValidDate IsYear(this int year) => new() { Year = year.Success() };

      public static IResult<ValidDate> AndYear(this IResult<ValidDate> validDate, int year)
      {
         return validDate.Map(vd => vd.AndYear(year));
      }

      public static IResult<DateTime> AndYearValid(this IResult<ValidDate> validDate, int year)
      {
         return validDate.Map(vd => vd.AndYearValid(year));
      }

      public static ValidDate IsMonth(this int month) => new() { Month = month.Success() };

      public static IResult<ValidDate> AndMonth(this IResult<ValidDate> validDate, int month)
      {
         return validDate.Map(vd => vd.AndMonth(month));
      }

      public static IResult<DateTime> AndMonthValid(this IResult<ValidDate> validDate, int month)
      {
         return validDate.Map(vd => vd.AndMonthValid(month));
      }

      public static ValidDate IsDay(this int day) => new() { Day = day.Success() };

      public static IResult<ValidDate> AndDay(this IResult<ValidDate> validDate, int day) => validDate.Map(vd => vd.AndDay(day));

      public static IResult<DateTime> AndDayValid(this IResult<ValidDate> validDate, int day)
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

      public static IResult<string> MonthName(this int month)
      {
         switch (month)
         {
            case 1:
               return "January".Success();
            case 2:
               return "February".Success();
            case 3:
               return "March".Success();
            case 4:
               return "April".Success();
            case 5:
               return "May".Success();
            case 6:
               return "June".Success();
            case 7:
               return "July".Success();
            case 8:
               return "August".Success();
            case 9:
               return "September".Success();
            case 10:
               return "October".Success();
            case 11:
               return "November".Success();
            case 12:
               return "December".Success();
            default:
               return "Month must be in a range from 1 to 12".Failure<string>();
         }
      }

      public static IResult<int> MonthNumber(this string name) => name.ToLower() switch
      {
         "january" or "jan" => 1.Success(),
         "february" or "feb" => 2.Success(),
         "march" or "mar" => 3.Success(),
         "april" or "apr" => 4.Success(),
         "may" => 5.Success(),
         "june" or "jun" => 6.Success(),
         "july" or "jul" => 7.Success(),
         "august" or "aug" => 8.Success(),
         "september" or "sep" => 9.Success(),
         "october" or "oct" => 10.Success(),
         "november" or "nov" => 11.Success(),
         "december" or "dec" => 12.Success(),
         _ => $"Didn't understand {name}".Failure<int>()
      };

      public static IResult<DateTime> RelativeTo(this DateTime date, string pattern)
      {
         if (pattern.IsMatch("^ ['//|'] /s* ['//|'] /s* ['//|'] $; f"))
         {
            return date.Success();
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
                     var yearAmount = yearText.ToInt();
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
                     var monthAmount = monthText.ToInt();
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
                     var dayAmount = dayText.ToInt();
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

                  return builder.Date.Success();
               }

               return $"Couldn't extract date elements from {pattern}".Failure<DateTime>();
            }
            else
            {
               return $"Didn't understand {pattern}".Failure<DateTime>();
            }
         }
      }

      public static void Sleep(this TimeSpan interval) => Thread.Sleep(interval);

      public static bool OldEnough(this DateTime date, TimeSpan age) => NowServer.Now - date >= age;
   }
}