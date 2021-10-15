using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Core.Dates;
using Core.Dates.DateIncrements;
using Core.Dates.Now;
using static Core.Objects.ConversionFunctions;

namespace Core.Tests
{
   [TestClass]
   public class DateTimeTests
   {
      [TestMethod]
      public void DescriptionFromNowTest()
      {
         var beginningDate = Value.DateTime("06/01/2021");
         DateIncrementer incrementer = beginningDate;
         NowServer.SetToTest(incrementer);

         checkToday();
         checkYesterday();
         checkDayBeforeYesterday();
         checkThreeDaysAgo();
         check5DaysAgo();
         check7DaysAgo();
         check8DaysAgo();
         check30DaysAgo();
         checkAYearAgo();
      }

      protected static void checkDate(string message, DateTime dateTime)
      {
         Console.WriteLine($"{message}: [{dateTime}, {dateTime.DayOfWeek}] = {dateTime.DescriptionFromNow()}");
      }

      protected static void checkToday() => checkDate("Today", NowServer.Today);

      protected static void checkYesterday() => checkDate("Yesterday", NowServer.Today - 1.Day());

      protected static void checkDayBeforeYesterday() => checkDate("Day before yesterday", NowServer.Now - 2.Days());

      protected static void checkThreeDaysAgo() => checkDate("3 days ago", NowServer.Now - 3.Days());

      protected static void check5DaysAgo() => checkDate("5 days ago", NowServer.Today - 5.Days());

      protected static void check7DaysAgo() => checkDate("7 days ago", NowServer.Today - 7.Days());

      protected static void check8DaysAgo() => checkDate("8 days ago", NowServer.Today - 8.Days());

      protected static void check30DaysAgo() => checkDate("30 days ago", NowServer.Today - 30.Days());

      protected static void checkAYearAgo() => checkDate("1 year ago", NowServer.Today - 365.Days());
   }
}