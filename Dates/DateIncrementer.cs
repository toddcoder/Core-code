using System;
using Core.Assertions;
using Core.Dates.Now;
using Core.Monads;
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
         var newDate = month.MustAs(nameof(month)).BeBetween(1).And(12).Try().Map(m => new Month(date, m).Date);
         return newDate.OnSuccess(d => date = d);
      }

      public Day Day
      {
         get => new Day(date);
         set => date = value.Date;
      }

      public IResult<DateTime> SetDay(int day)
      {
         var newDate = day.MustAs(nameof(day)).BeBetween(1).And(date.LastOfMonth().Day).Try().Map(d => new Day(date, d).Date);
         return newDate.OnSuccess(d => date = d);
      }

      public IResult<DateTime> SetToLastDay() => SetDay(date.LastOfMonth().Day);

      public Hour Hour
      {
         get => new Hour(date);
         set => date = value.Date;
      }

      public IResult<DateTime> SetHour(int hour)
      {
         var newDate = hour.MustAs(nameof(hour)).BeBetween(0).Until(24).Try().Map(h => new Hour(date, h).Date);
         return newDate.OnSuccess(d => date = d);
      }

      public Minute Minute
      {
         get => new Minute(date);
         set => date = value.Date;
      }

      public IResult<DateTime> SetMinute(int minute)
      {
         var newDate = minute.MustAs(nameof(minute)).BeBetween(0).Until(60).Try().Map(m => new Minute(date, m).Date);
         return newDate.OnSuccess(d => date = d);
      }

      public Second Second
      {
         get => new Second(date);
         set => date = value.Date;
      }

      public IResult<DateTime> SetSecond(int second)
      {
         var newDate = second.MustAs(nameof(second)).BeBetween(0).Until(60).Try().Map(s => new Second(date, s).Date);
         return newDate.OnSuccess(d => date = d);
      }

      public Millisecond Millisecond
      {
         get => new Millisecond(date);
         set => date = value.Date;
      }

      public IResult<DateTime> SetMillisecond(int millisecond)
      {
         var newDate = millisecond.MustAs(nameof(millisecond)).BeBetween(0).Until(1000).Try().Map(m => new Millisecond(date, m).Date);
         return newDate.OnSuccess(d => date = d);
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