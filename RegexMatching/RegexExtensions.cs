using System;
using System.Collections.Generic;
using System.Text;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using RRegex = System.Text.RegularExpressions.Regex;

namespace Core.RegexMatching
{
   public static class RegexExtensions
   {
      public static bool IsMatch(this string input, Matcher pattern)
      {
         return pattern.Matches(input).IsMatched;
      }

      public static string Substitute(this string input, Matcher pattern, string replacement)
      {
         return RRegex.Replace(input, pattern.Pattern, replacement, pattern.Options);
      }

      public static string Substitute(this string input, Matcher pattern, string replacement, int count)
      {
         var regex = new RRegex(pattern.Pattern, pattern.Options);

         return regex.Replace(input, replacement, count);
      }

      public static string Replace(this string input, Matcher pattern, Action<Result> replacer)
      {
         if (pattern.Matches(input).If(out var result, out _))
         {
            replacer(result);
            return result.ToString();
         }
         else
         {
            return input;
         }
      }

      public static string[] Split(this string input, Matcher pattern)
      {
         return RRegex.Split(input, pattern.Pattern, pattern.Options);
      }

      public static IEnumerable<Slice> SplitIntoSlices(this string input, Matcher pattern)
      {
         var split = input.Split(pattern);
         var index = 0;

         foreach (var segment in split)
         {
            yield return new Slice { Index = index, Length = segment.Length, Text = segment };

            index += segment.Length;
         }
      }

      public static (string, string) Split2(this string input, Matcher pattern)
      {
         var result = input.Split(pattern);
         return result.Length == 1 ? (result[0], "") : (result[0], result[1]);
      }

      public static (string, string, string) Split3(this string input, Matcher pattern)
      {
         var result = input.Split(pattern);
         return result.Length switch
         {
            1 => (result[0], "", ""),
            2 => (result[0], result[1], ""),
            _ => (result[0], result[1], result[2])
         };
      }

      public static (string, string, string, string) Split4(this string input, Matcher pattern)
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

      public static (string group1, string group2) Group2(this string input, Matcher pattern)
      {
         Matcher matcher = pattern;
         if (matcher.Matches(input).If(out var result))
         {
            return (result.FirstGroup, result.SecondGroup);
         }
         else
         {
            return ("", "");
         }
      }

      public static (string group1, string group2, string group3) Group3(this string input, Matcher pattern)
      {
         Matcher matcher = pattern;
         if (matcher.Matches(input).If(out var result))
         {
            return (result.FirstGroup, result.SecondGroup, result.ThirdGroup);
         }
         else
         {
            return ("", "", "");
         }
      }

      public static (string group1, string group2, string group3, string group4) Group4(this string input, Matcher pattern)
      {
         Matcher matcher = pattern;
         if (matcher.Matches(input).If(out var result))
         {
            return (result.FirstGroup, result.SecondGroup, result.ThirdGroup, result.FourthGroup);
         }
         else
         {
            return ("", "", "", "");
         }
      }

      public static string Retain(this string input, Matcher pattern)
      {
         if (pattern.Matches(input).If(out var result))
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

      public static string Scrub(this string input, Matcher pattern)
      {
         if (pattern.Matches(input).If(out var result))
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

      public static IMatched<Result> Matched(this string input, Matcher pattern) => pattern.Matches(input);

      public static IMaybe<Result> Matches(this string input, Matcher pattern)
      {
         if (pattern.Matches(input).If(out var result))
         {
            return result.Some();
         }
         else
         {
            return none<Result>();
         }
      }

      public static Matcher Matcher(this string pattern, bool ignoreCase, bool multiline, bool friendly)
      {
         if (!ignoreCase && !multiline && RegexMatching.Matcher.IsFriendly)
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
         return RegexMatching.Matcher.IsFriendly ? input.Replace("/", "//") : RRegex.Escape(input).Replace("]", @"\]");
      }
   }
}