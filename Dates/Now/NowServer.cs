using System;

namespace Core.Dates.Now
{
   public static class NowServer
   {
      private static NowBase now;

      static NowServer() => SetToStandard();

      public static void SetToStandard() => now = new StandardNow();

      public static void SetToTest(DateIncrementer incrementer) => now = new TestNow(incrementer);

      public static DateTime Now => now.Now;

      public static DateTime Today => now.Today;
   }
}