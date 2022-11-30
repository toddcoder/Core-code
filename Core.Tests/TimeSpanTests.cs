using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Dates.TimeSpanExtensions;

namespace Core.Tests;

[TestClass]
public class TimeSpanTests
{
   [TestMethod]
   public void ToStringTest()
   {
      var timeSpan = new TimeSpan(1, 2, 3, 4, 53);
      Console.WriteLine(timeSpan.ToString(true));
   }
}