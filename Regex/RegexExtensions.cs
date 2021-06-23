﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Core.Monads;
using Core.Numbers;
using Core.RegularExpressions;
using Core.Strings;
using static System.Text.RegularExpressions.RegexOptions;
using static Core.Monads.MonadFunctions;

namespace Core.Regex
{
   public static class RegexExtensions
   {
      internal static RegexOptions GetOptions(bool ignoreCase, bool multiline)
      {
         var options = None;

         if (ignoreCase)
         {
            options |= IgnoreCase;
         }

         options |= multiline ? Multiline : Singleline;

         return options;
      }

      public static bool IsMatch(this string input, string pattern, RegexOptions options)
      {
         return System.Text.RegularExpressions.Regex.IsMatch(input, pattern, options);
      }

      public static bool IsMatch(this string input, string pattern, bool ignoreCase = false, bool multiline = false)
      {
         return input.IsMatch(pattern, GetOptions(ignoreCase, multiline));
      }

      public static bool IsMatch(this string input, RegexPattern regexPattern)
      {
         return input.IsMatch(regexPattern.Pattern, regexPattern.Options);
      }

      public static string Substitute(this string input, string pattern, string replacement, RegexOptions options)
      {
         return System.Text.RegularExpressions.Regex.Replace(input, pattern, replacement, options);
      }

      public static string Substitute(this string input, string pattern, string replacement, RegexOptions options, int count)
      {
         var regex = new System.Text.RegularExpressions.Regex(pattern, options);
         return regex.Replace(input, replacement, count);
      }

      public static string Substitute(this string input, string pattern, string replacement, bool ignoreCase = false, bool multiline = false)
      {
         return input.Substitute(pattern, replacement, GetOptions(ignoreCase, multiline));
      }

      public static string Substitute(this string input, string pattern, string replacement, int count, bool ignoreCase = false,
         bool multiline = false)
      {
         return input.Substitute(pattern, replacement, GetOptions(ignoreCase, multiline), count);
      }

      public static string Substitute(this string input, RegexPattern regexPattern, string replacement)
      {
         return input.Substitute(regexPattern.Pattern, replacement, regexPattern.Options);
      }

      public static string Substitute(this string input, RegexPattern regexPattern, string replacement, int count)
      {
         return input.Substitute(regexPattern.Pattern, replacement, regexPattern.Options, count);
      }

      public static string Replace(this string input, string pattern, Action<Matcher> replacer, RegexOptions options)
      {
         var matcher = new Matcher();
         if (matcher.IsMatch(input, pattern, options))
         {
            replacer(matcher);
            return matcher.ToString();
         }
         else
         {
            return input;
         }
      }

      public static string Replace(this string input, string pattern, Action<Matcher> replacer, bool ignoreCase = false, bool multiline = false)
      {
         return input.Replace(pattern, replacer, GetOptions(ignoreCase, multiline));
      }

      public static string Replace(this string input, RegexPattern regexPattern, Action<Matcher> replacer)
      {
         return input.Replace(regexPattern.Pattern, replacer, regexPattern.Options);
      }

      public static string[] Split(this string input, string pattern, RegexOptions options)
      {
         return System.Text.RegularExpressions.Regex.Split(input, pattern, options);
      }

      public static string[] Split(this string input, string pattern, bool ignoreCase = false, bool multiline = false)
      {
         return input.Split(pattern, GetOptions(ignoreCase, multiline));
      }

      public static string[] Split(this string input, RegexPattern regexPattern)
      {
         return input.Split(regexPattern.Pattern, regexPattern.Options);
      }

      private static IEnumerable<Slice> sliceSplit(string input, string pattern, RegexOptions regexOptions)
      {
         var includeSeparator = pattern.StartsWith("(");
         var startIndex = 0;
         var matcher = new Matcher();
         if (matcher.IsMatch(input, pattern, regexOptions))
         {
            for (var i = 0; i < matcher.MatchCount; i++)
            {
               var (matchText, index, length) = matcher.GetMatch(i);
               var text = input.Drop(startIndex).Keep(index - startIndex);
               yield return new Slice { Text = text, Index = startIndex, Length = text.Length };

               if (includeSeparator)
               {
                  yield return new Slice { Text = matchText, Index = index, Length = length };
               }

               startIndex = index + length;
            }

            var rest = input.Drop(startIndex);
            yield return new Slice { Text = rest, Index = startIndex, Length = rest.Length };
         }
         else
         {
            yield return new Slice { Text = input, Index = 0, Length = input.Length };
         }
      }

      public static IEnumerable<Slice> SliceSplit(this string input, string pattern, RegexOptions regexOptions)
      {
         return sliceSplit(input, pattern, regexOptions);
      }

      public static IEnumerable<Slice> SliceSplit(this string input, string pattern, bool ignoreCase = false, bool multiline = false)
      {
         return sliceSplit(input, pattern, GetOptions(ignoreCase, multiline));
      }

      public static IEnumerable<Slice> SliceSplit(this string input, RegexPattern regexPattern)
      {
         return sliceSplit(input, regexPattern.Pattern, regexPattern.Options);
      }

      public static (string, string) Split2(this string input, string pattern, bool ignoreCase = false, bool multiline = false)
      {
         var result = input.Split(pattern, ignoreCase, multiline);
         return result.Length == 1 ? (result[0], "") : (result[0], result[1]);
      }

      public static (string, string) Split2(this string input, string pattern, RegexOptions options)
      {
         var result = input.Split(pattern, options);
         return result.Length == 1 ? (result[0], "") : (result[0], result[1]);
      }

      public static (string, string) Split2(this string input, RegexPattern regexPattern)
      {
         return input.Split2(regexPattern.Pattern, regexPattern.Options);
      }

      public static (string, string, string) Split3(this string input, string pattern, bool ignoreCase = false, bool multiline = false)
      {
         var result = input.Split(pattern, ignoreCase, multiline);
         return result.Length switch
         {
            1 => (result[0], "", ""),
            2 => (result[0], result[1], ""),
            _ => (result[0], result[1], result[2])
         };
      }

      public static (string, string, string) Split3(this string input, string pattern, RegexOptions options)
      {
         var result = input.Split(pattern, options);
         return result.Length switch
         {
            1 => (result[0], "", ""),
            2 => (result[0], result[1], ""),
            _ => (result[0], result[1], result[2])
         };
      }

      public static (string, string, string) Split3(this string input, RegexPattern regexPattern)
      {
         return input.Split3(regexPattern.Pattern, regexPattern.Options);
      }

      public static (string, string, string, string) Split4(this string input, string pattern, bool ignoreCase = false, bool multiline = false)
      {
         var result = input.Split(pattern, ignoreCase, multiline);
         return result.Length switch
         {
            1 => (result[0], "", "", ""),
            2 => (result[0], result[1], "", ""),
            3 => (result[0], result[1], result[2], ""),
            _ => (result[0], result[1], result[2], result[3])
         };
      }

      public static (string, string, string, string) Split4(this string input, string pattern, RegexOptions options)
      {
         var result = input.Split(pattern, options);
         return result.Length switch
         {
            1 => (result[0], "", "", ""),
            2 => (result[0], result[1], "", ""),
            3 => (result[0], result[1], result[2], ""),
            _ => (result[0], result[1], result[2], result[3])
         };
      }

      public static (string, string, string, string) Split4(this string input, RegexPattern regexPattern)
      {
         return input.Split4(regexPattern.Pattern, regexPattern.Options);
      }

      public static Maybe<Matcher> Matcher(this string input, string pattern, bool ignoreCase = false, bool multiline = false)
      {
         var matcher = new Matcher();
         return maybe(matcher.IsMatch(input, pattern, ignoreCase, multiline), () => matcher);
      }

      public static Maybe<Matcher> Matcher(this string input, string pattern, RegexOptions options)
      {
         var matcher = new Matcher();
         return maybe(matcher.IsMatch(input, pattern, options), () => matcher);
      }

      public static Maybe<Matcher> Matcher(this string input, RegexPattern regexPattern)
      {
         return input.Matcher(regexPattern.Pattern, regexPattern.Options);
      }

      public static bool Matcher(this string input, string pattern, out Matcher matcher, bool ignoreCase = false, bool multiline = false)
      {
         return input.Matcher(pattern, ignoreCase, multiline).If(out matcher);
      }

      public static bool Matcher(this string input, string pattern, out Matcher matcher, RegexOptions options)
      {
         return input.Matcher(pattern, options).If(out matcher);
      }

      public static bool Matcher(this string input, RegexPattern regexPattern, out Matcher matcher)
      {
         return input.Matcher(regexPattern).If(out matcher);
      }

      public static void IfMatches(this string input, string pattern, Action<Matcher> ifTrue, bool ignoreCase = false, bool multiline = false)
      {
         var matcher = new Matcher();
         if (matcher.IsMatch(input, pattern, ignoreCase, multiline))
         {
            ifTrue(matcher);
         }
      }

      public static void IfMatches(this string input, string pattern, Action<Matcher> ifTrue, RegexOptions options)
      {
         var matcher = new Matcher();
         if (matcher.IsMatch(input, pattern, options))
         {
            ifTrue(matcher);
         }
      }

      public static void IfMatches(this string input, RegexPattern regexPattern, Action<Matcher> ifTrue)
      {
         input.IfMatches(regexPattern.Pattern, ifTrue, regexPattern.Options);
      }

      public static void IfMatches(this string input, string pattern, Action<Matcher> ifTrue, Action ifFalse, bool ignoreCase = false,
         bool multiline = false)
      {
         var matcher = new Matcher();
         if (matcher.IsMatch(input, pattern, ignoreCase, multiline))
         {
            ifTrue(matcher);
         }
         else
         {
            ifFalse();
         }
      }

      public static void IfMatches(this string input, string pattern, Action<Matcher> ifTrue, Action ifFalse, RegexOptions options)
      {
         var matcher = new Matcher();
         if (matcher.IsMatch(input, pattern, options))
         {
            ifTrue(matcher);
         }
         else
         {
            ifFalse();
         }
      }

      public static void IfMatches(this string input, RegexPattern regexPattern, Action<Matcher> ifTrue, Action ifFalse)
      {
         input.IfMatches(regexPattern.Pattern, ifTrue, ifFalse, regexPattern.Options);
      }

      public static IMatched<RegularExpressions.Matcher.Match[]> MatchAll(this string input, string pattern, RegexOptions options)
      {
         return new Matcher().MatchAll(input, pattern, options);
      }

      public static IMatched<RegularExpressions.Matcher.Match[]> MatchAll(this string input, string pattern, bool ignoreCase = false,
         bool multiline = false)
      {
         return new Matcher().MatchAll(input, pattern, ignoreCase, multiline);
      }

      public static IMatched<RegularExpressions.Matcher.Match[]> MatchAll(this string input, RegexPattern regexPattern)
      {
         return new Matcher().MatchAll(input, regexPattern);
      }

      public static IMatched<RegularExpressions.Matcher.Match> MatchOne(this string input, string pattern, RegexOptions options)
      {
         return new Matcher().MatchOne(input, pattern, options);
      }

      public static IMatched<RegularExpressions.Matcher.Match> MatchOne(this string input, string pattern, bool ignoreCase = false,
         bool multiline = false)
      {
         return new Matcher().MatchOne(input, pattern, ignoreCase, multiline);
      }

      public static IMatched<RegularExpressions.Matcher.Match> MatchOne(this string input, RegexPattern regexPattern)
      {
         return new Matcher().MatchOne(input, regexPattern);
      }

      public static IEnumerable<RegularExpressions.Matcher.Match> Matches(this string input, string pattern, bool ignoreCase = false,
         bool multiline = false)
      {
         return new Matcher().Matched(input, pattern, ignoreCase, multiline);
      }

      public static IEnumerable<RegularExpressions.Matcher.Match> Matches(this string input, string pattern, RegexOptions options)
      {
         return new Matcher().Matched(input, pattern, options);
      }

      public static IEnumerable<RegularExpressions.Matcher.Match> Matches(this string input, RegexPattern regexPattern)
      {
         return new Matcher().Matched(input, regexPattern);
      }

      public static IEnumerable<RegexResult> Matches(this string input, params string[] patterns)
      {
         foreach (var pattern in patterns)
         {
            var regexPattern = (RegularExpressions.RegexPattern)pattern;
            var result = regexPattern.Matches(input);
            if (result.IsMatch)
            {
               yield return result;

               input = input.Drop(result.Length);
            }
            else
            {
               yield break;
            }
         }
      }

      public static string Retain(this string input, string pattern, bool ignoreCase, bool multiline)
      {
         var matcher = new Matcher();
         if (matcher.IsMatch(input, pattern, ignoreCase, multiline))
         {
            var builder = new StringBuilder();
            foreach (var match in matcher.Matches)
            {
               builder.Append(match);
            }

            return builder.ToString();
         }
         else
         {
            return string.Empty;
         }
      }

      public static string Retain(this string input, string pattern, Bits32<RegexOptions> options)
      {
         return input.Retain(pattern, options[IgnoreCase], options[Multiline]);
      }

      public static string Retain(this string input, RegexPattern regexPattern)
      {
         return input.Retain(regexPattern.Pattern, regexPattern.IgnoreCase, regexPattern.Multiline);
      }

      public static string Scrub(this string input, string pattern, bool ignoreCase, bool multiline)
      {
         var matcher = new Matcher();
         if (matcher.IsMatch(input, pattern, ignoreCase, multiline))
         {
            for (var i = 0; i < matcher.MatchCount; i++)
            {
               matcher[i] = string.Empty;
            }

            return matcher.ToString();
         }
         else
         {
            return input;
         }
      }

      public static string Scrub(this string input, string pattern, Bits32<RegexOptions> options)
      {
         return input.Scrub(pattern, options[IgnoreCase], options[Multiline]);
      }

      public static string Scrub(this string input, RegexPattern regexPattern)
      {
         return input.Scrub(regexPattern.Pattern, regexPattern.IgnoreCase, regexPattern.Multiline);
      }
   }
}