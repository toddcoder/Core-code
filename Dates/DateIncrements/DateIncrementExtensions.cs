using System;

namespace Core.Dates.DateIncrements;

public static class DateIncrementExtensions
{
   public static TimeSpan Day(this int amount) => new(amount, 0, 0, 0);

   public static TimeSpan Days(this int amount) => amount.Day();

   public static TimeSpan Hour(this int amount) => new(amount, 0, 0);

   public static TimeSpan Hours(this int amount) => amount.Hour();

   public static TimeSpan Minute(this int amount) => new(0, amount, 0);

   public static TimeSpan Minutes(this int amount) => amount.Minute();

   public static TimeSpan Second(this int amount) => new(0, 0, amount);

   public static TimeSpan Seconds(this int amount) => amount.Second();

   public static TimeSpan Millisecond(this int amount) => new(0, 0, 0, 0, amount);

   public static TimeSpan Milliseconds(this int amount) => amount.Millisecond();
}