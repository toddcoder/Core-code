using System;
using System.Collections.Generic;
using System.Text;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using RRegex = System.Text.RegularExpressions.Regex;

namespace Core.Matching
{
   public static class MatchingExtensions
   {
      public static bool IsMatch(this string input, Pattern pattern)
      {
         return pattern.MatchedBy(input).IsMatched;
      }

      public static string Substitute(this string input, Pattern pattern, string replacement)
      {
         return RRegex.Replace(input, pattern.Regex, replacement, pattern.Options);
      }

      public static string Substitute(this string input, Pattern pattern, string replacement, int count)
      {
         var regex = new RRegex(pattern.Regex, pattern.Options);

         return regex.Replace(input, replacement, count);
      }

      public static string Replace(this string input, Pattern pattern, Action<MatchResult> replacer)
      {
         if (pattern.MatchedBy(input).If(out var result, out _))
         {
            replacer(result);
            return result.ToString();
         }
         else
         {
            return input;
         }
      }

      public static string[] Split(this string input, Pattern pattern)
      {
         return RRegex.Split(input, pattern.Regex, pattern.Options);
      }

      public static IEnumerable<Slice> SplitIntoSlices(this string input, Pattern pattern)
      {
         if (input.Matches(pattern).If(out var result))
         {
            var index = 0;
            int length;
            string text;
            foreach (var (_, matchIndex, matchLength) in result)
            {
               length = matchIndex - index;
               text = input.Drop(index).Keep(length);
               yield return new Slice(text, index, length);

               index = matchIndex + matchLength;
            }

            length = input.Length - index;
            text = input.Drop(index);
            yield return new Slice(text, index, length);
         }
      }

      public static (string, string) Split2(this string input, Pattern pattern)
      {
         var result = input.Split(pattern);
         return result.Length == 1 ? (result[0], "") : (result[0], result[1]);
      }

      public static (string, string, string) Split3(this string input, Pattern pattern)
      {
         var result = input.Split(pattern);
         return result.Length switch
         {
            1 => (result[0], "", ""),
            2 => (result[0], result[1], ""),
            _ => (result[0], result[1], result[2])
         };
      }

      public static (string, string, string, string) Split4(this string input, Pattern pattern)
      {
         var result = input.Split(pattern);
         return result.Length switch
         {
            1 => (result[0], "", "", ""),
            2 => (result[0], result[1], "", ""),
            3 => (result[0], result[1], result[2], ""),
            _ => (result[0], result[1], result[2], result[3])
         };
      }

      public static (string group1, string group2) Group2(this string input, Pattern pattern)
      {
         if (pattern.MatchedBy(input).If(out var result))
         {
            return (result.FirstGroup, result.SecondGroup);
         }
         else
         {
            return ("", "");
         }
      }

      public static (string group1, string group2, string group3) Group3(this string input, Pattern pattern)
      {
         if (pattern.MatchedBy(input).If(out var result))
         {
            return (result.FirstGroup, result.SecondGroup, result.ThirdGroup);
         }
         else
         {
            return ("", "", "");
         }
      }

      public static (string group1, string group2, string group3, string group4) Group4(this string input, Pattern pattern)
      {
         if (pattern.MatchedBy(input).If(out var result))
         {
            return (result.FirstGroup, result.SecondGroup, result.ThirdGroup, result.FourthGroup);
         }
         else
         {
            return ("", "", "", "");
         }
      }

      public static string Retain(this string input, Pattern pattern)
      {
         if (pattern.MatchedBy(input).If(out var result))
         {
            var builder = new StringBuilder();
            foreach (var match in result)
            {
               builder.Append(match.Text);
            }

            return builder.ToString();
         }
         else
         {
            return string.Empty;
         }
      }

      public static string Scrub(this string input, Pattern pattern)
      {
         if (pattern.MatchedBy(input).If(out var result))
         {
            for (var i = 0; i < result.MatchCount; i++)
            {
               result[i] = string.Empty;
            }

            return result.ToString();
         }
         else
         {
            return input;
         }
      }

      public static Matched<MatchResult> Matched(this string input, Pattern pattern) => pattern.MatchedBy(input);

      public static Maybe<MatchResult> Matches(this string input, Pattern pattern)
      {
         if (pattern.MatchedBy(input).If(out var result))
         {
            return result.Some();
         }
         else
         {
            return none<MatchResult>();
         }
      }

      public static Pattern Pattern(this string pattern, bool ignoreCase, bool multiline, bool friendly)
      {
         if (!ignoreCase && !multiline && Matching.Pattern.IsFriendly)
         {
            return pattern;
         }

         var strIgnoreCase = ignoreCase ? "i" : "c";
         var strMultiline = multiline ? "m" : "s";
         var strFriendly = friendly ? "f" : "u";

         return $"{pattern}; {strIgnoreCase}{strMultiline}{strFriendly}";
      }

      public static string Escape(this string input)
      {
         return Matching.Pattern.IsFriendly ? input.Replace("/", "//") : RRegex.Escape(input).Replace("]", @"\]");
      }

      public static IEnumerable<Match> AllMatches(this string input, Pattern pattern)
      {
         if (input.Matches(pattern).If(out var result))
         {
            foreach (var match in result)
            {
               yield return match;
            }
         }
      }
   }
}