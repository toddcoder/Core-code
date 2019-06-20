using System;
using Core.Monads;
using Core.Numbers;
using static System.Math;

namespace Core.Dates.Relative
{
   public class Relation
   {
      bool isRelative;
      int amount;

      public Relation(string source, int amount)
      {
         this.amount = amount;
         switch (source)
         {
            case "+":
               isRelative = true;
               break;
            case "-":
               isRelative = true;
               this.amount = -Abs(this.amount);
               break;
            default:
               isRelative = false;
               break;
         }
      }

      public DateTime Year(DateTime date) => isRelative ? date.AddYears(amount) : new DateTime(amount, date.Month, safeDay(date));

      static int safeDay(DateTime date) => Min(date.Day, date.LastOfMonth().Day);

      static int safeDay(DateTime date, int month) => Min(date.Day, month.LastOfMonth(date.Year));

      public IResult<DateTime> Month(DateTime date)
      {
         if (isRelative)
         {
            return date.AddMonths(amount).Success();
         }
         else if (amount.Between(1).And(12))
         {
            return new DateTime(date.Year, date.Month, safeDay(date, amount)).Success();
         }
         else
         {
            return $"Month {amount} must be in range 1 to 12".Failure<DateTime>();
         }
      }

      public IResult<DateTime> Day(DateTime date)
      {
         if (isRelative)
         {
            return date.AddDays(amount).Success();
         }
         else if (amount == 0)
         {
            return date.LastOfMonth().Success();
         }
         else
         {
            var lastOfMonth = date.Month.LastOfMonth(date.Year);
            if (amount <= lastOfMonth)
            {
               return new DateTime(date.Year, date.Month, amount).Success();
            }
            else
            {
               return $"Day {amount} must be in range 1-{lastOfMonth} for {date.Month}/{date.Year}".Failure<DateTime>();
            }
         }
      }
   }
}