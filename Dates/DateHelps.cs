using System;
using System.Linq;
using static Core.Booleans.Assertions;

namespace Core.Dates
{
   public static class DateHelps
   {
      public static DateTime Average(this DateTime[] dates)
      {
         Assert(dates.Length > 0, "You must have at least one date");
         return new DateTime((long)dates.Average(d => (double)d.Ticks));
      }
   }
}