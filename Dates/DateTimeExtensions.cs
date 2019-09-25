using System;
using System.Threading;
using Core.Arrays;
using Core.Dates.Now;
using Core.Monads;
using Core.Numbers;
using Core.RegularExpressions;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Dates
{
   public static class DateTimeExtensions
   {
      const string REGEX_SIGN = "^ ['+-']";

      public class ValidDate
      {
         static IResult<DateTime> valid(int year, int month, int day)
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

         IResult<DateTime> validate()
         {
            if (Year.ValueOrResult<DateTime>(out var year, out var original) && Month.ValueOrResult(out var month, out original) &&
               Day.ValueOrResult(out var day, out original))
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

      public static ValidDate IsYear(this int year) => new ValidDate { Year = year.Success() };

      public static IResult<ValidDate> AndYear(this IResult<ValidDate> validDate, int year)
      {
         return validDate.Map(vd => vd.AndYear(year));
      }

      public static IResult<DateTime> AndYearValid(this IResult<ValidDate> validDate, int year)
      {
         return validDate.Map(vd => vd.AndYearValid(year));
      }

      public static ValidDate IsMonth(this int month) => new ValidDate { Month = month.Success() };

      public static IResult<ValidDate> AndMonth(this IResult<ValidDate> validDate, int month)
      {
         return validDate.Map(vd => vd.AndMonth(month));
      }

      public static IResult<DateTime> AndMonthValid(this IResult<ValidDate> validDate, int month)
      {
         return validDate.Map(vd => vd.AndMonthValid(month));
      }

      public static ValidDate IsDay(this int day) => new ValidDate { Day = day.Success() };

      public static IResult<ValidDate> AndDay(this IResult<ValidDate> validDate, int day) => validDate.Map(vd => vd.AndDay(day));

      public static IResult<DateTime> AndDayValid(this IResult<ValidDate> validDate, int day)
      {
         return validDate.Map(vd => vd.AndDayValid(day));
      }

      public static DateTime FirstOfMonth(this DateTime date) => new DateTime(date.Year, date.Month, 1);

      public static DateTime LastOfMonth(this DateTime date) => date.FirstOfMonth().AddMonths(1).AddDays(-1);

      public static int LastOfMonth(this int month, int year) => new DateTime(year, month, 1).AddMonths(1).AddDays(-1).Day;

      public static DateTime NearestQuarterHour(this DateTime time)
      {
         var minutes = time.Minute;
         var quarter = minutes % 15;

         if (quarter >= 0 && quarter <= 7)
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

      public static DateTime Truncate(this DateTime date) => new DateTime(date.Year, date.Month, date.Day);

      public static Numbers.Comparison<DateTime> ComparedTo(this DateTime left, DateTime right)
      {
         return new Numbers.Comparison<DateTime>(left, right);
      }

      public static DateEnumerator To(this DateTime beginDate, DateTime endDate)
      {
         return new DateEnumerator(beginDate, endDate);
      }

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

      public static IResult<int> MonthNumber(this string name)
      {
         switch (name.ToLower())
         {
            case "january":
            case "jan":
               return 1.Success();
            case "february":
            case "feb":
               return 2.Success();
            case "march":
            case "mar":
               return 3.Success();
            case "april":
            case "apr":
               return 4.Success();
            case "may":
               return 5.Success();
            case "june":
            case "jun":
               return 6.Success();
            case "july":
            case "jul":
               return 7.Success();
            case "august":
            case "aug":
               return 8.Success();
            case "september":
            case "sep":
               return 9.Success();
            case "october":
            case "oct":
               return 10.Success();
            case "november":
            case "nov":
               return 11.Success();
            case "december":
            case "dec":
               return 12.Success();
            default:
               return $"Didn't understand {name}".Failure<int>();
         }
      }

      public static IResult<DateTime> RelativeTo(this DateTime date, string pattern)
      {
         if (pattern.IsMatch("^ ['//|'] /s* ['//|'] /s* ['//|'] $"))
         {
            return date.Success();
         }
         else
         {
            return pattern.MatchOne("/(['+-']? /d*) ['//|'] /(['+-']? /d*) ['//|'] /(['+-']? /d*)").FlatMap(match =>
            {
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
                        var result = builder.SetYear(yearAmount);
                        if (result.IsFailed)
                        {
                           return result;
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
                        var result = builder.SetMonth(monthAmount);
                        if (result.IsFailed)
                        {
                           return result;
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
                        var result = builder.SetToLastDay();
                        if (result.IsFailed)
                        {
                           return result;
                        }
                     }
                     else
                     {
                        var result = builder.SetDay(dayAmount);
                        if (result.IsFailed)
                        {
                           return result;
                        }
                     }
                  }

                  return builder.Date.Success();
               }

               return $"Couldn't extract date elements from {pattern}".Failure<DateTime>();
            }, () => $"Didn't understand {pattern}".Failure<DateTime>(), failure<DateTime>);
         }
      }

      public static void Sleep(this TimeSpan interval) => Thread.Sleep(interval);

      public static bool OldEnough(this DateTime date, TimeSpan age) => NowServer.Now - date >= age;
   }
}