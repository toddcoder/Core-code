using System;
using System.Text.RegularExpressions;
using Core.Assertions;
using Core.Monads;
using static System.Text.RegularExpressions.RegexOptions;
using static Core.Monads.MonadFunctions;

namespace Core.RegularExpressions
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

      public static bool IsMatch(this string input, string pattern, RegexOptions options, bool friendly = true)
      {
         if (friendly)
         {
            pattern = Matcher.getPattern(pattern);
         }

         return System.Text.RegularExpressions.Regex.IsMatch(input, pattern, options);
      }

      public static bool IsMatch(this string input, string pattern, bool ignoreCase = false, bool multiline = false,
         bool friendly = true)
      {
         return input.IsMatch(pattern, GetOptions(ignoreCase, multiline), friendly);
      }

      public static void RequiredMatch(this string input, string pattern, string message, RegexOptions options,
         bool friendly = true)
      {
         input.IsMatch(pattern, options, friendly).MustAs(nameof(input)).Be().Assert(message);

      }

      public static void RequiredMatch(this string input, string pattern, string message, bool ignoreCase = false,
         bool multiline = false, bool friendly = true)
      {
         input.IsMatch(pattern, ignoreCase, multiline, friendly).MustAs(input).Be().Assert(message);
      }

      public static string Substitute(this string input, string pattern, string replacement, RegexOptions options,
         bool friendly = true)
      {
         if (friendly)
         {
            pattern = Matcher.getPattern(pattern);
         }

         return System.Text.RegularExpressions.Regex.Replace(input, pattern, replacement, options);
      }

      public static string Substitute(this string input, string pattern, string replacement, RegexOptions options,
         int count, bool friendly = true)
      {
         if (friendly)
         {
            pattern = Matcher.getPattern(pattern);
         }

         var regex = new System.Text.RegularExpressions.Regex(pattern, options);

         return regex.Replace(input, replacement, count);
      }

      public static string Substitute(this string input, string pattern, string replacement, bool ignoreCase = false,
         bool multiline = false, bool friendly = true)
      {
         return input.Substitute(pattern, replacement, GetOptions(ignoreCase, multiline), friendly);
      }

      public static string Substitute(this string input, string pattern, string replacement, int count,
         bool ignoreCase = false, bool multiline = false, bool friendly = true)
      {
         return input.Substitute(pattern, replacement, GetOptions(ignoreCase, multiline), count, friendly);
      }

      public static string Replace(this string input, string pattern, Action<Matcher> replacer,
         RegexOptions options, bool friendly = true)
      {
         var matcher = new Matcher(friendly);
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

      public static string Replace(this string input, string pattern, Action<Matcher> replacer,
         bool ignoreCase = false, bool multiline = false, bool friendly = true)
      {
         return input.Replace(pattern, replacer, GetOptions(ignoreCase, multiline), friendly);
      }

      public static string[] Split(this string input, string pattern, RegexOptions options, bool friendly = true)
      {
         if (friendly)
         {
            pattern = Matcher.getPattern(pattern);
         }

         return System.Text.RegularExpressions.Regex.Split(input, pattern, options);
      }

      public static string[] Split(this string input, string pattern, bool ignoreCase = false, bool multiline = false,
         bool friendly = true)
      {
         return input.Split(pattern, GetOptions(ignoreCase, multiline), friendly);
      }

      public static string Escape(this string input, bool friendly = true)
      {
         return friendly ? input.Replace("/", "//") : System.Text.RegularExpressions.Regex.Escape(input).Replace("]", @"\]");
      }

      public static string FriendlyString(this string input) => $"'{input.Replace("'", "\\'")}'";

      public static IMaybe<Matcher> Matches(this string input, string pattern, bool ignoreCase = false,
         bool multiline = false, bool friendly = true)
      {
         var matcher = new Matcher(friendly);

         return maybe(matcher.IsMatch(input, pattern, ignoreCase, multiline), () => matcher);
      }

      public static IMaybe<Matcher> Matches(this string input, string pattern, RegexOptions options,
         bool friendly = true)
      {
         var matcher = new Matcher(friendly);

         return maybe(matcher.IsMatch(input, pattern, options), () => matcher);
      }

      public static bool Matches(this string input, string pattern, out Matcher matcher, bool ignoreCase = false,
         bool multiline = false, bool friendly = true)
      {
         return input.Matches(pattern, ignoreCase, multiline, friendly).If(out matcher);
      }

      public static bool Matches(this string input, string pattern, out Matcher matcher, RegexOptions options,
         bool friendly = true)
      {
         return input.Matches(pattern, options, friendly).If(out matcher);
      }

      public static void IfMatches(this string input, string pattern, Action<Matcher> ifTrue, bool ignoreCase = false,
         bool multiline = false, bool friendly = true)
      {
         var matcher = new Matcher(friendly);
         if (matcher.IsMatch(input, pattern, ignoreCase, multiline))
         {
            ifTrue(matcher);
         }
      }

      public static void IfMatches(this string input, string pattern, Action<Matcher> ifTrue, RegexOptions options,
         bool friendly = true)
      {
         var matcher = new Matcher(friendly);
         if (matcher.IsMatch(input, pattern, options))
         {
            ifTrue(matcher);
         }
      }

      public static void IfMatches(this string input, string pattern, Action<Matcher> ifTrue, Action ifFalse,
         bool ignoreCase = false, bool multiline = false, bool friendly = true)
      {
         var matcher = new Matcher(friendly);
         if (matcher.IsMatch(input, pattern, ignoreCase, multiline))
         {
            ifTrue(matcher);
         }
         else
         {
            ifFalse();
         }
      }

      public static void IfMatches(this string input, string pattern, Action<Matcher> ifTrue, Action ifFalse,
         RegexOptions options, bool friendly = true)
      {
         var matcher = new Matcher(friendly);
         if (matcher.IsMatch(input, pattern, options))
         {
            ifTrue(matcher);
         }
         else
         {
            ifFalse();
         }
      }

      public static IMatched<Matcher.Match[]> MatchAll(this string input, string pattern, RegexOptions options,
         bool friendly = true)
      {
         return new Matcher(friendly).MatchAll(input, pattern, options);
      }

      public static IMatched<Matcher.Match[]> MatchAll(this string input, string pattern, bool ignoreCase = false,
         bool multiline = false, bool friendly = true)
      {
         return new Matcher(friendly).MatchAll(input, pattern, ignoreCase, multiline);
      }

      public static IMatched<Matcher.Match> MatchOne(this string input, string pattern, RegexOptions options,
         bool friendly = true) => new Matcher(friendly).MatchOne(input, pattern, options);

      public static IMatched<Matcher.Match> MatchOne(this string input, string pattern, bool ignoreCase = false,
         bool multiline = false, bool friendly = true)
      {
         return new Matcher(friendly).MatchOne(input, pattern, ignoreCase, multiline);
      }
   }
}