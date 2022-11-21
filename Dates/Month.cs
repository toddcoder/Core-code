using System;

namespace Core.Dates;

public struct Month
{
   public static implicit operator int(Month month) => month.Value;

   public static Month operator +(Month month, int value) => new(month.date.AddMonths(value));

   public static Month operator -(Month month, int value) => new(month.date.AddMonths(-value));

   private DateTime date;

   public Month(DateTime date) : this() => this.date = date;

   public Month(DateTime date, int month) : this()
   {
      this.date = new DateTime(date.Year, month, date.Day, date.Hour, date.Minute, date.Second);
   }

   public int Value => date.Month;

   public DateTime Date => date;
}