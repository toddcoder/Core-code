using System;
using Core.Assertions;
using Core.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Matcher = Core.Regex.Matcher;
using RegexExtensions = Core.Regex.RegexExtensions;

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
            matcher.ToString().Must().Equal("tstylecop.style.format.options.xml").OrThrow();
         }
      }

      [TestMethod]
      public void MatchOnlySubstitutions()
      {
         var result = RegexExtensions.Substitute("This is the full sentence with sql1 in it", @"sql(\d)", "sql-$1");
         Console.WriteLine(result);
         result.Must().Equal("This is the full sentence with sql-1 in it").OrThrow();
      }

      [TestMethod]
      public void MatchPatternsTest()
      {
         foreach (var (text, index, _, _, itemIndex, isFound) in RegexExtensions.Matches("foobar(foo,baz)", "/w+ '('", "/w+ ','", "/w+ ')'"))
         {
            if (isFound)
            {
               Console.WriteLine($"{itemIndex}: Found \"{text}\" at {index}");
            }
            else
            {
               Console.WriteLine($"{itemIndex}: Not found");
            }
         }

         var outerResult = RegexResult.Empty;

         foreach (var result in RegexExtensions.Matches("foobar(foo, baz, box)", "/(/w+) '('", "(/s* ',')? /s* /(/w+)"))
         {
            outerResult = result;
            var exit = false;
            switch (result.ItemIndex)
            {
               case 0:
                  Console.Write($"{result.FirstGroup}(");
                  break;
               case 1:
                  exit = true;
                  break;
            }

            if (exit)
            {
               break;
            }
         }

         while (outerResult.IsFound)
         {
            Console.Write(outerResult.Text);
            outerResult = outerResult.Next();
         }

         Console.WriteLine(")");
      }
   }
}