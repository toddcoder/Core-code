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
         return pattern.MatchedBy(input);
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
         if (pattern.MatchedBy(input).Map(out var result, out _))
         {
            replacer(result);
            return result.ToString();
         }
         else
         {
            return input;
         }
      }

      [Obsolete("Use Unjoin")]
      public static string[] Split(this string input, Pattern pattern)
      {
         return RRegex.Split(input, pattern.Regex, pattern.Options);
      }

      public static string[] Unjoin(this string input, Pattern pattern)
      {
         return RRegex.Split(input, pattern.Regex, pattern.Options);
      }

      [Obsolete("Use UnjoinIntoSlices")]
      public static IEnumerable<Slice> SplitIntoSlices(this string input, Pattern pattern)
      {
         if (input.Matches(pattern).Map(out var result))
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

      public static IEnumerable<Slice> UnjoinIntoSlices(this string input, Pattern pattern)
      {
         if (input.Matches(pattern).Map(out var result))
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

      [Obsolete("Use Unjoin2")]
      public static (string, string) Split2(this string input, Pattern pattern)
      {
         var result = input.Split(pattern);
         return result.Length == 1 ? (result[0], "") : (result[0], result[1]);
      }

      public static (string, string) Unjoin2(this string input, Pattern pattern)
      {
         var result = input.Unjoin(pattern);
         return result.Length == 1 ? (result[0], "") : (result[0], result[1]);
      }

      [Obsolete("Use Unjoin3")]
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

      public static (string, string, string) Unjoin3(this string input, Pattern pattern)
      {
         var result = input.Unjoin(pattern);
         return result.Length switch
         {
            1 => (result[0], "", ""),
            2 => (result[0], result[1], ""),
            _ => (result[0], result[1], result[2])
         };
      }

      [Obsolete("Use Unjoin4")]
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

      public static (string, string, string, string) Unjoin4(this string input, Pattern pattern)
      {
         var result = input.Unjoin(pattern);
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
         if (pattern.MatchedBy(input).Map(out var result))
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
         if (pattern.MatchedBy(input).Map(out var result))
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
         if (pattern.MatchedBy(input).Map(out var result))
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
         if (pattern.MatchedBy(input).Map(out var result))
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
         if (pattern.MatchedBy(input).Map(out var result))
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

      public static Responding<MatchResult> Matched(this string input, Pattern pattern) => pattern.MatchedBy(input);

      public static Maybe<MatchResult> Matches(this string input, Pattern pattern) => pattern.MatchedBy(input).Maybe();

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
         if (input.Matches(pattern).Map(out var result))
         {
            foreach (var match in result)
            {
               yield return match;
            }
         }
      }

      public static Maybe<MatchResult> FirstMatch(this IEnumerable<Pattern> patterns, string input)
      {
         foreach (var pattern in patterns)
         {
            var _result = pattern.MatchedBy(input);
            if (_result)
            {
               return _result.Maybe();
            }
         }

         return nil;
      }

      public static IEnumerable<Either<Slice, Match>> LeadingMatches(this IEnumerable<Match> matches, string input, bool includeRemainder = false)
      {
         var index = 0;
         foreach (var match in matches)
         {
            var text = input.Drop(index).Keep(match.Index - index);
            var length = text.Length;
            var slice = new Slice(text, index, length);
            yield return slice;

            yield return match;

            index = match.Index + match.Length;
         }

         if (includeRemainder)
         {
            var remainder = input.Drop(index);
            if (remainder.IsNotEmpty())
            {
               yield return new Slice(remainder, index, remainder.Length);
            }
         }
      }
   }
}