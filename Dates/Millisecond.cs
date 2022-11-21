using System;

namespace Core.Dates;

public struct Millisecond
{
   public static implicit operator int(Millisecond millisecond) => millisecond.Value;

   public static Millisecond operator +(Millisecond millisecond, int value)
   {
      return new Millisecond(millisecond.date.AddMilliseconds(value));
   }

   public static Millisecond operator -(Millisecond millisecond, int value)
   {
      return new Millisecond(millisecond.date.AddMilliseconds(-value));
   }

   private DateTime date;

   public Millisecond(DateTime date) : this() => this.date = date;

   public Millisecond(DateTime date, int millisecond) : this()
   {
      var dateTime = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
      this.date = dateTime.AddMilliseconds(millisecond);
   }

   public int Value => date.Millisecond;

   public DateTime Date => date;
}