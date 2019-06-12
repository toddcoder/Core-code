using System;
using Core.Dates.Now;
using Core.Monads;
using Core.Numbers;
using static Core.Monads.AttemptFunctions;

namespace Core.Dates
{
   public class DateIncrementer : IComparable<DateTime>
   {
      public static implicit operator DateIncrementer(DateTime date) => new DateIncrementer(date);

      public static implicit operator DateTime(DateIncrementer incrementer) => incrementer.date;

      public static bool operator ==(DateIncrementer left, DateTime right) => left.CompareTo(right) == 0;

      public static bool operator !=(DateIncrementer left, DateTime right) => left.CompareTo(right) != 0;

      public static bool operator <(DateIncrementer left, DateTime right) => left.CompareTo(right) < 0;

      public static bool operator <=(DateIncrementer left, DateTime right) => left.CompareTo(right) <= 0;

      public static bool operator >(DateIncrementer left, DateTime right) => left.CompareTo(right) > 0;

      public static bool operator >=(DateIncrementer left, DateTime right) => left.CompareTo(right) >= 0;

      public static DateIncrementer operator +(DateIncrementer incrementer, TimeSpan increment)
      {
         return incrementer.Date + increment;
      }

      public static DateIncrementer operator -(DateIncrementer incrementer, TimeSpan increment)
      {
         return incrementer.Date - increment;
      }

      DateTime date;

      public DateIncrementer(DateTime date) => this.date = date;

      public DateTime Date => date;

      public void Clear() => date = DateTime.MinValue;

      public void Now() => date = NowServer.Now;

      public void Today() => date = NowServer.Today;

      public Year Year
      {
         get => new Year(date);
         set => date = value.Date;
      }

      public IResult<DateTime> SetYear(int year) => tryTo(() => date = new Year(date, year).Date);

      public Month Month
      {
         get => new Month(date);
         set => date = value.Date;
      }

      public IResult<DateTime> SetMonth(int month)
      {
         return assert(month.Between(1).And(12), () => date = new Month(date, month).Date, () => $"Month {month} out of range");
      }

      public Day Day
      {
         get => new Day(date);
         set => date = value.Date;
      }

      public IResult<DateTime> SetDay(int day)
      {
         return assert(day.Between(1).And(date.LastOfMonth().Day), () => date = new Day(date, day).Date, () => $"Day {day} out of range");
      }

      public IResult<DateTime> SetToLastDay() => SetDay(date.LastOfMonth().Day);

      public Hour Hour
      {
         get => new Hour(date);
         set => date = value.Date;
      }

      public IResult<DateTime> SetHour(int hour)
      {
         return assert(hour.Between(0).And(24), () => date = new Hour(date, hour).Date, () => $"Hour {hour} out of range");
      }

      public Minute Minute
      {
         get => new Minute(date);
         set => date = value.Date;
      }

      public IResult<DateTime> SetMinute(int minute)
      {
         return assert(minute.Between(0).And(59), () => date = new Minute(date, minute).Date, () => $"Minute {minute} out of range");
      }

      public Second Second
      {
         get => new Second(date);
         set => date = value.Date;
      }

      public IResult<DateTime> SetSecond(int second)
      {
         return assert(second.Between(0).And(59), () => date = new Second(date, second).Date, () => $"Second {second} out of range");
      }

      public Millisecond Millisecond
      {
         get => new Millisecond(date);
         set => date = value.Date;
      }

      public IResult<DateTime> SetMillisecond(int millisecond)
      {
         return assert(millisecond.Between(0).And(999),
            () => date = new Millisecond(date, millisecond).Date,
            () => $"Millisecond {millisecond} out of range");
      }

      public int CompareTo(DateTime other) => date.CompareTo(other);

      public override string ToString() => date.ToString();

      public string ToString(string format) => date.ToString(format);

      protected bool Equals(DateIncrementer other) => date.Equals(other.date);

      public override bool Equals(object obj) => obj is DateIncrementer di && date == di.Date;

      public override int GetHashCode() => date.GetHashCode();

      public DateIncrementer Clone() => new DateIncrementer(date);
   }
}