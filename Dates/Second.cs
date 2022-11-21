using System;

namespace Core.Dates;

public struct Second
{
   public static implicit operator int(Second second) => second.Value;

   public static Second operator +(Second second, int value) => new(second.date.AddSeconds(value));

   public static Second operator -(Second second, int value) => new(second.date.AddSeconds(-value));

   private DateTime date;

   public Second(DateTime date) : this() => this.date = date;

   public Second(DateTime date, int second) : this()
   {
      this.date = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, second);
   }

   public int Value => date.Second;

   public DateTime Date => date;
}