using System;
using Core.Assertions;
using Core.Monads;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.RegexMatching;
using Core.Strings;

namespace Core.Tests
{
   [TestClass]
   public class RegexMatchingTests
   {
      protected static void matcherTest(string pattern)
      {
         Matcher matcher = pattern;
         if (matcher.Matches("tsqlcop.sql.format.options.xml").If(out var result))
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
      public void UMatcherTest()
      {
         Matcher.IsFriendly = false;
         matcherTest("(sql)");
      }

      [TestMethod]
      public void FMatcherTest()
      {
         Matcher.IsFriendly = true;
         matcherTest("/('sql')");
      }

      protected static void matchOnlySubstitutions(string pattern)
      {
         var result = "This is the full sentence with sql1 in it".Substitute(pattern, "sql-$1");
         Console.WriteLine(result);
         result.Must().Equal("This is the full sentence with sql-1 in it").OrThrow();
      }

      [TestMethod]
      public void UMatchOnlySubstitutionsTest()
      {
         Matcher.IsFriendly = false;
         matchOnlySubstitutions(@"sql(\d+)");
      }

      [TestMethod]
      public void FMatchOnlySubstitutionsTest()
      {
         Matcher.IsFriendly = true;
         matchOnlySubstitutions("'sql' /(/d+)");
      }

      protected static void matchPatternsTest(string pattern1, string pattern2, string pattern3)
      {
         if (((Matcher)pattern1).Matches("foobar(foo,baz)").If(out var result))
         {
            Console.Write(result.FirstMatch);
            IMaybe<Exception> _exception;
            var lastResult = result;
            while (result.Matches(pattern2).If(out result, out _exception))
            {
               Console.Write(result.FirstMatch);
               lastResult = result;
            }

            if (_exception.If(out var exception))
            {
               Console.WriteLine($"Exception: {exception.Message}");
            }

            if (lastResult.Matches(pattern3).If(out result))
            {
               Console.WriteLine(result.FirstMatch);
            }
         }
      }

      [TestMethod]
      public void UMatchPatternsTest()
      {
         Matcher.IsFriendly = false;
         matchPatternsTest(@"^\w+\(", @"\w+,",@"\w+\)");
      }

      [TestMethod]
      public void FMatchPatternsTest()
      {
         Matcher.IsFriendly = true;
         matchPatternsTest("^ /w+ '('", "/w+ ','", "/w+ ')'");
      }

      [TestMethod]
      public void FQuoteTest()
      {
         Matcher matcher = "`quote /(-[`quote]+) `quote";
         if (matcher.Matches("\"Fee fi fo fum\" said the giant.").If(out var result))
         {
            Console.WriteLine(result.FirstGroup.Guillemetify());
         }
      }

      [TestMethod]
      public void RetainTest()
      {
         Matcher.IsFriendly = true;

         var source = "~foobar-foo?baz-boo!boo-yogi";
         var retained = source.Retain("[/w '-']");
         Console.WriteLine(retained);
      }

      [TestMethod]
      public void ScrubTest()
      {
         var source = "~foobar-foo?baz-boo!boo-yogi";
         var scrubbed = source.Scrub("[/w '-']");
         Console.WriteLine(scrubbed);
      }
   }
}
