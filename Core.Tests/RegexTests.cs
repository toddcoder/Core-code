using System;
using System.Collections.Generic;
using Core.Assertions;
using Core.Enumerables;
using Core.RegularExpressions;
using Core.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
         var result = "This is the full sentence with sql1 in it".Substitute("'sql' /(/d)", "sql-$1");
         Console.WriteLine(result);
         result.Must().Equal("This is the full sentence with sql-1 in it").OrThrow();
      }

      [TestMethod]
      public void MatchPatternsTest()
      {
         foreach (var (text, index, _, _, itemIndex, isFound) in "foobar(foo,baz)".Matches("/w+ '('", "/w+ ','", "/w+ ')'"))
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

         foreach (var result in "foobar(foo, baz, box)".Matches("/(/w+) '('", "(/s* ',')? /s* /(/w+)"))
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

         while (outerResult.IsMatch)
         {
            Console.Write(outerResult.Text);
            outerResult = outerResult.MatchNext();
         }

         Console.WriteLine(")");
      }

      [TestMethod]
      public void MatchFirstTest()
      {
         var matcher = new Matcher();
         var input = "foobar(foo, baz, boq) -> foobaz";
         var pattern = (RegexPattern)@"^ /(/w+) '('";

         if (matcher.IsMatch(input, pattern))
         {
            Console.Write($"{matcher.FirstGroup}(");
            var result = matcher.MatchFirst((RegexPattern)@"(/s* ',')? /s* /(/w+)");
            var list = new List<string>();
            while (result.IsMatch)
            {
               list.Add(result.FirstGroup);
               result = result.MatchNext();
            }

            result = result.MatchFirst((RegexPattern)@"^ ')' /s* '->' /s* /(/w+)");
            if (result.IsMatch)
            {
               Console.WriteLine($"{list.ToString(", ")}) -> {result.FirstGroup}");
            }
         }
      }

      [TestMethod]
      public void QuoteTest()
      {
         var pattern = (RegexPattern)"`quote /(-[`quote]+) `quote";
         if ("\"Fee fi fo fum\" said the giant.".Matcher(pattern).If(out var matcher))
         {
            Console.WriteLine(matcher.FirstGroup.Guillemetify());
         }
      }
   }
}