using System;
using System.Collections.Generic;
using System.Linq;
using Core.Assertions;
using Core.Collections;
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
         var result = "^ /w+ '('".Matches("foobar(foo,baz)");
         if (result.IsMatch)
         {
            Console.Write(result.Text);
            while (result.IsMatch)
            {
               result = result.Matches("/w+ ','");
               if (result.IsMatch)
               {
                  Console.Write(result.Text);
               }
            }

            result = result.Matches("/w+ ')'");
            if (result.IsMatch)
            {
               Console.WriteLine(result.Text);
            }
         }
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
            var result = matcher.MatchOn((RegexPattern)@"(/s* ',')? /s* /(/w+)");
            var list = new List<string>();
            while (result.IsMatch)
            {
               list.Add(result.FirstGroup);
               result = result.MatchNext();
            }

            result = result.Matches((RegexPattern)@"^ ')' /s* '->' /s* /(/w+)");
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

      [TestMethod]
      public void ScraperTest()
      {
         static string getVariables(Hash<string, string> hash, string prefix)
         {
            var keys = hash.Keys
               .Where(k => k.StartsWith(prefix))
               .Select(k => (key: k, index: k.DropUntil(":") + 1))
               .OrderBy(t => t.index)
               .Select(t => t.key);
            return hash.ValuesFromKeys(keys).ToString(", ");
         }

         var scraper = new Scraper("foo(a, b, c)\r\nbar(x,y , z)");
         var index1 = 0;
         var index2 = 0;
         var _result =
            from name1 in scraper.Match("^ /(/w+) '('", "name1")
            from pushed1 in name1.Push("^ -[')']+")
            from split1 in pushed1.Split("/s* ',' /s*", s => $"var0_{s}:{index1++}")
            from popped1 in split1.Pop()
            from skipped1 in popped1.Skip(1)
            from skippedCrLf in skipped1.Skip("^ (/r /n)+")
            from name2 in scraper.Match("^ /(/w+) '('", "name2")
            from pushed2 in name2.Push("^ -[')']+")
            from split2 in pushed2.Split("/s* ',' /s*", s => $"var1_{s}:{index2++}")
            from popped2 in split2.Pop()
            select popped2;
         if (_result.If(out scraper, out var _exception))
         {
            var hash = scraper.AnyHash().ForceValue();
            var func1 = $"{hash["name1"]}({getVariables(hash, "var0_")})";
            var func2 = $"{hash["name2"]}({getVariables(hash, "var1_")})";
            Console.WriteLine(func1);
            Console.WriteLine(func2);
         }
         else if (_exception.If(out var exception))
         {
            Console.WriteLine($"Exception: {exception.Message}");
         }
         else
         {
            Console.WriteLine("Not matched");
         }
      }
   }
}