using System;

namespace Core.Dates;

public struct Hour
{
   public static implicit operator int(Hour hour) => hour.Value;

   public static Hour operator +(Hour hour, int value) => new(hour.date.AddHours(value));

   public static Hour operator -(Hour hour, int value) => new(hour.date.AddHours(-value));

   private DateTime date;

   public Hour(DateTime date) : this() => this.date = date;

   public Hour(DateTime date, int hour) : this()
   {
      this.date = new DateTime(date.Year, date.Month, date.Day, hour, date.Minute, date.Second);
   }

   public int Value => date.Hour;

   public DateTime Date => date;
}