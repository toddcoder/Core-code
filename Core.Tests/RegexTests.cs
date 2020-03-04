using System;
using Core.Assertions;
using Core.Regex;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Assertions.AssertionFunctions;

namespace Core.Tests
{
   [TestClass]
   public class RegexTests
   {
      [TestMethod]
      public void MatcherTest()
      {
         var matcher = new Matcher();
         if (matcher.IsMatch("tsqlcop.sql.format.options.xml", "(sql)"))
         {
            for (var matchIndex = 0; matchIndex < matcher.MatchCount; matchIndex++)
            {
               matcher[matchIndex, 1] = "style";
            }

            Console.WriteLine(matcher);
            assert(() => matcher.ToString()).Must().Equal("tstylecop.style.format.options.xml").OrThrow();
         }
      }

      [TestMethod]
      public void MatchOnlySubstitutions()
      {
         var result = "This is the full sentence with sql1 in it".Substitute(@"\w{3}(\d)", "-$1", matchOnly: true);
         Console.WriteLine(result);
         assert(() => result).Must().Equal("sql-1").OrThrow();
      }
   }
}