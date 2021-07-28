using System;
using System.Linq;
using Core.Assertions;
using Core.Collections;
using Core.Enumerables;
using Core.Matching;
using Core.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
   [TestClass]
   public class MatchingTests
   {
      [TestMethod]
      public void MatcherTest()
      {
         if ("tsqlcop.sql.format.options.xml".Matches("(sql); u").If(out var result))
         {
            for (var matchIndex = 0; matchIndex < result.MatchCount; matchIndex++)
            {
               result[matchIndex, 1] = "style";
            }

            Console.WriteLine(result);
            result.ToString().Must().Equal("tstylecop.style.format.options.xml").OrThrow();
         }
      }

      [TestMethod]
      public void MatchOnlySubstitutions()
      {
         var result = "This is the full sentence with sql1 in it".Substitute("'sql' /(/d); f", "sql-$1");
         Console.WriteLine(result);
         result.Must().Equal("This is the full sentence with sql-1 in it").OrThrow();
      }

      [TestMethod]
      public void MatchPatternsTest()
      {
         if ("foobar(foo,baz)".Matches("^ /w+ '('; f").If(out var result))
         {
            Console.Write(result);
            var lastResult = result;
            while (result.MatchedBy("/w+ ','; f").If(out result))
            {
               Console.Write(result);
               lastResult = result;
            }

            if (lastResult.MatchedBy("/w+ ')'; f").If(out result))
            {
               Console.WriteLine(result);
            }
         }
      }

      [TestMethod]
      public void QuoteTest()
      {
         var pattern = "`quote /(-[`quote]+) `quote; f";
         if ("\"Fee fi fo fum\" said the giant.".Matches(pattern).If(out var result))
         {
            Console.WriteLine(result.FirstGroup.Guillemetify());
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

         var scraper = new Scraper("foo(a, b, c)\r\nbar(x,y , z); f");
         var index1 = 0;
         var index2 = 0;
         var _result =
            from name1 in scraper.Match("^ /(/w+) '('; f", "name1")
            from pushed1 in name1.Push("^ -[')']+; f")
            from split1 in pushed1.Split("/s* ',' /s*; f", s => $"var0_{s}:{index1++}")
            from popped1 in split1.Pop()
            from skipped1 in popped1.Skip(1)
            from skippedCrLf in skipped1.Skip("^ (/r /n)+; f")
            from name2 in scraper.Match("^ /(/w+) '('; f", "name2")
            from pushed2 in name2.Push("^ -[')']+; f")
            from split2 in pushed2.Split("/s* ',' /s*; f", s => $"var1_{s}:{index2++}")
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

      [TestMethod]
      public void RetainTest()
      {
         var source = "~foobar-foo?baz-boo!boo-yogi";
         var retained = source.Retain("[/w '-']; f");
         Console.WriteLine(retained);
      }

      [TestMethod]
      public void ScrubTest()
      {
         var source = "~foobar-foo?baz-boo!boo-yogi";
         var retained = source.Scrub("[/w '-']; f");
         Console.WriteLine(retained);
      }
   }
}