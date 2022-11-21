using System;
using Core.Monads;
using Core.Numbers;
using static System.Math;
using static Core.Monads.MonadFunctions;

namespace Core.Dates.Relative;

public class Relation
{
   protected bool isRelative;
   protected int amount;

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

   protected static int safeDay(DateTime date) => Min(date.Day, date.LastOfMonth().Day);

   protected static int safeDay(DateTime date, int month) => Min(date.Day, month.LastOfMonth(date.Year));

   public Result<DateTime> Month(DateTime date)
   {
      if (isRelative)
      {
         return date.AddMonths(amount);
      }
      else if (amount.Between(1).And(12))
      {
         return new DateTime(date.Year, date.Month, safeDay(date, amount));
      }
      else
      {
         return fail($"Month {amount} must be in range 1 to 12");
      }
   }

   public Result<DateTime> Day(DateTime date)
   {
      if (isRelative)
      {
         return date.AddDays(amount);
      }
      else if (amount == 0)
      {
         return date.LastOfMonth();
      }
      else
      {
         var lastOfMonth = date.Month.LastOfMonth(date.Year);
         if (amount <= lastOfMonth)
         {
            return new DateTime(date.Year, date.Month, amount);
         }
         else
         {
            return fail($"Day {amount} must be in range 1-{lastOfMonth} for {date.Month}/{date.Year}");
         }
      }
   }
}