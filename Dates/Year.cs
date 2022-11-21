using System;

namespace Core.Dates;

public struct Year
{
   public static implicit operator int(Year year) => year.Value;

   public static Year operator +(Year year, int value) => new(year.date.AddYears(value));

   public static Year operator -(Year year, int value) => new(year.date.AddYears(-value));

   private DateTime date;

   public Year(DateTime date) : this() => this.date = date;

   public Year(DateTime date, int year) : this()
   {
      this.date = new DateTime(year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
   }

   public int Value => date.Year;

   public DateTime Date => date;
}