using System;

namespace Core.Dates;

public struct Day
{
   public static implicit operator int(Day day) => day.Value;

   public static Day operator +(Day day, int value) => new(day.date.AddDays(value));

   public static Day operator -(Day day, int value) => new(day.date.AddDays(-value));

   private DateTime date;

   public Day(DateTime date) : this() => this.date = date;

   public Day(DateTime date, int day) : this()
   {
      this.date = new DateTime(date.Year, date.Month, day, date.Hour, date.Minute, date.Second);
   }

   public int Value => date.Day;

   public DateTime Date => date;
}