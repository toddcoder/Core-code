using System;

namespace Core.Dates;

public struct Minute
{
   public static implicit operator int(Minute minute) => minute.Value;

   public static Minute operator +(Minute minute, int value) => new(minute.date.AddMinutes(value));

   public static Minute operator -(Minute minute, int value) => new(minute.date.AddMinutes(-value));

   private DateTime date;

   public Minute(DateTime date) : this() => this.date = date;

   public Minute(DateTime date, int minute) : this()
   {
      this.date = new DateTime(date.Year, date.Month, date.Day, date.Hour, minute, date.Second);
   }

   public int Value => date.Hour;

   public DateTime Date => date;
}