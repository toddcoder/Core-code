using System;
using System.Linq;
using Core.Assertions;

namespace Core.Dates
{
   public static class DateExtensions
   {
      public static DateTime Average(this DateTime[] dates)
      {
         dates.Must().Not.BeEmpty().Assert("You must have at least one date");
         return new DateTime((long)dates.Average(d => (double)d.Ticks));
      }
   }
}