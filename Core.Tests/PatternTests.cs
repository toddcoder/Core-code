using System;
using Core.Assertions;
using Core.Matching;
using Core.Monads;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.Strings;

namespace Core.Tests
{
   [TestClass]
   public class PatternTests
   {
      protected static void matcherTest(Pattern pattern)
      {
         if ("tsqlcop.sql.format.options.xml".Matches(pattern).If(out var result))
         {
            foreach (var match in result)
            {
               match.FirstGroup = "style";
            }

            Console.WriteLine(result);
            result.ToString().Must().Equal("tstylecop.style.format.options.xml").OrThrow();
         }
      }

      [TestMethod]
      public void UMatcherTest()
      {
         matcherTest("(sql); u");
      }

      [TestMethod]
      public void FMatcherTest()
      {
         matcherTest("/('sql'); f");
      }

      protected static void matchOnlySubstitutions(Pattern pattern)
      {
         var result = "This is the full sentence with sql1 in it".Substitute(pattern, "sql-$1");
         Console.WriteLine(result);
         result.Must().Equal("This is the full sentence with sql-1 in it").OrThrow();
      }

      [TestMethod]
      public void UMatchOnlySubstitutionsTest()
      {
         Pattern.IsFriendly = false;
         matchOnlySubstitutions(@"sql(\d+); u");
      }

      [TestMethod]
      public void FMatchOnlySubstitutionsTest()
      {
         matchOnlySubstitutions("'sql' /(/d+); f");
      }

      protected static void matchPatternsTest(Pattern pattern1, Pattern pattern2, Pattern pattern3)
      {
         if ("foobar(foo,baz)".Matches(pattern1).If(out var result))
         {
            Console.Write(result.FirstMatch);
            Maybe<Exception> _exception;
            var lastResult = result;
            while (result.MatchedBy(pattern2).If(out result, out _exception))
            {
               Console.Write(result.FirstMatch);
               lastResult = result;
            }

            if (_exception.If(out var exception))
            {
               Console.WriteLine($"Exception: {exception.Message}");
            }

            if (lastResult.MatchedBy(pattern3).If(out result))
            {
               Console.WriteLine(result.FirstMatch);
            }
         }
      }

      [TestMethod]
      public void UMatchPatternsTest()
      {
         matchPatternsTest(@"^\w+\(; u", @"\w+,; u",@"\w+\)l; u");
      }

      [TestMethod]
      public void FMatchPatternsTest()
      {
         matchPatternsTest("^ /w+ '('; f", "/w+ ','; f", "/w+ ')'; f");
      }

      [TestMethod]
      public void FQuoteTest()
      {
         Pattern pattern = "`quote /(-[`quote]+) `quote; f";
         if ("\"Fee fi fo fum\" said the giant.".Matches(pattern).If(out var result))
         {
            Console.WriteLine(result.FirstGroup.Guillemetify());
         }
      }

      [TestMethod]
      public void RetainTest()
      {
         Pattern.IsFriendly = true;

         var source = "~foobar-foo?baz-boo!boo-yogi";
         var retained = source.Retain("[/w '-']; f");
         Console.WriteLine(retained);
      }

      [TestMethod]
      public void ScrubTest()
      {
         var source = "~foobar-foo?baz-boo!boo-yogi";
         var scrubbed = source.Scrub("[/w '-']; f");
         Console.WriteLine(scrubbed);
      }

      [TestMethod]
      public void BugTest()
      {
         var pattern = @"^ '\)'; f";
         var input = @"\)";
         if (input.Matches(pattern).IsSome)
         {
            Console.WriteLine("Matched");
         }
         else
         {
            Console.WriteLine("Not matched");
         }
      }
   }
}
