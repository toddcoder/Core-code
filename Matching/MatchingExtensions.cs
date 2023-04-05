using System;
using System.Collections.Generic;
using System.Text;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using RRegex = System.Text.RegularExpressions.Regex;

namespace Core.Matching;

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
      var _response = pattern.MatchedBy(input);
      if (_response is (true, var response))
      {
         replacer(response);
         return response.ToString();
      }
      else
      {
         return input;
      }
   }

   public static string[] Unjoin(this string input, Pattern pattern)
   {
      return RRegex.Split(input, pattern.Regex, pattern.Options);
   }

   public static IEnumerable<Slice> UnjoinIntoSlices(this string input, Pattern pattern)
   {
      var _matches = input.Matches(pattern);
      if (_matches is (true, var matches))
      {
         var index = 0;
         int length;
         string text;
         foreach (var (_, matchIndex, matchLength) in matches)
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

   public static (string, string) Unjoin2(this string input, Pattern pattern)
   {
      var result = input.Unjoin(pattern);
      return result.Length == 1 ? (result[0], "") : (result[0], result[1]);
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
      var _result = pattern.MatchedBy(input);
      if (_result is (true, var (group1, group2)))
      {
         return (group1, group2);
      }
      else
      {
         return ("", "");
      }
   }

   public static (string group1, string group2, string group3) Group3(this string input, Pattern pattern)
   {
      var _result = pattern.MatchedBy(input);
      if (_result is (true, var (group1, group2, group3)))
      {
         return (group1, group2, group3);
      }
      else
      {
         return ("", "", "");
      }
   }

   public static (string group1, string group2, string group3, string group4) Group4(this string input, Pattern pattern)
   {
      var _result = pattern.MatchedBy(input);
      if (_result is (true, var (group1, group2, group3, group4)))
      {
         return (group1, group2, group3, group4);
      }
      else
      {
         return ("", "", "", "");
      }
   }

   public static string Retain(this string input, Pattern pattern)
   {
      var _matches = pattern.MatchedBy(input);
      if (_matches is (true, var matches))
      {
         var builder = new StringBuilder();
         foreach (var match in matches)
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
      var _matches = pattern.MatchedBy(input);
      if (_matches is (true, var matches))
      {
         foreach (var match in matches)
         {
            match.Text = string.Empty;
         }

         return matches.ToString();
      }
      else
      {
         return input;
      }
   }

   public static Optional<MatchResult> MatchedBy(this string pattern, string input) => ((Pattern)pattern).MatchedBy(input);

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
      var _matches = input.Matches(pattern);
      if (_matches is (true, var matches))
      {
         foreach (var match in matches)
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
         if (_result is (true, var result))
         {
            return result;
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