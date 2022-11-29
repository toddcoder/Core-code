using System;
using System.Linq;
using Core.Assertions;

namespace Core.Dates;

public static class DateExtensions
{
   public static DateTime Average(this DateTime[] dates)
   {
      dates.Must().Not.BeEmpty().OrThrow("You must have at least one date");
      return new DateTime((long)dates.Average(d => (double)d.Ticks));
   }

   public static string Zulu(this DateTime dateTime) => dateTime.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
}