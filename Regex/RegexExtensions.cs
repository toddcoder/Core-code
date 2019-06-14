using System;
using System.Text.RegularExpressions;
using Core.Monads;
using static Core.Booleans.Assertions;
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
				options |= IgnoreCase;
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

		public static void RequiredMatch(this string input, string pattern, string message, RegexOptions options)
		{
			Assert(input.IsMatch(pattern, options), message);
		}

		public static void RequiredMatch(this string input, string pattern, string message, bool ignoreCase = false,
			bool multiline = false)
		{
			Assert(input.IsMatch(pattern, ignoreCase, multiline), message);
		}

		public static string Substitute(this string input, string pattern, string replacement, RegexOptions options)
		{
			return System.Text.RegularExpressions.Regex.Replace(input, pattern, replacement, options);
		}

		public static string Substitute(this string input, string pattern, string replacement, RegexOptions options,
			int count)
		{
			var regex = new System.Text.RegularExpressions.Regex(pattern, options);
			return regex.Replace(input, replacement, count);
		}

		public static string Substitute(this string input, string pattern, string replacement, bool ignoreCase = false,
			bool multiline = false)
		{
			return input.Substitute(pattern, replacement, GetOptions(ignoreCase, multiline));
		}

		public static string Substitute(this string input, string pattern, string replacement, int count,
			bool ignoreCase = false, bool multiline = false)
		{
			return input.Substitute(pattern, replacement, GetOptions(ignoreCase, multiline), count);
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
				return input;
		}

		public static string Replace(this string input, string pattern, Action<Matcher> replacer, bool ignoreCase = false,
			bool multiline = false)
		{
			return input.Replace(pattern, replacer, GetOptions(ignoreCase, multiline));
		}

		public static string[] Split(this string input, string pattern, RegexOptions options)
		{
			return System.Text.RegularExpressions.Regex.Split(input, pattern, options);
		}

		public static string[] Split(this string input, string pattern, bool ignoreCase = false, bool multiline = false)
		{
			return input.Split(pattern, GetOptions(ignoreCase, multiline));
		}

		public static IMaybe<Matcher> Matches(this string input, string pattern, bool ignoreCase = false, bool multiline = false)
		{
			var matcher = new Matcher();

			return maybe(matcher.IsMatch(input, pattern, ignoreCase, multiline), () => matcher);
		}

		public static IMaybe<Matcher> Matches(this string input, string pattern, RegexOptions options)
		{
			var matcher = new Matcher();
			return maybe(matcher.IsMatch(input, pattern, options), () => matcher);
		}

		public static bool Matches(this string input, string pattern, out Matcher matcher, bool ignoreCase = false,
			bool multiline = false)
		{
			return input.Matches(pattern, ignoreCase, multiline).If(out matcher);
		}

		public static bool Matches(this string input, string pattern, out Matcher matcher, RegexOptions options)
		{
			return input.Matches(pattern, options).If(out matcher);
		}

		public static void IfMatches(this string input, string pattern, Action<Matcher> ifTrue, bool ignoreCase = false,
			bool multiline = false)
		{
			var matcher = new Matcher();
			if (matcher.IsMatch(input, pattern, ignoreCase, multiline))
				ifTrue(matcher);
		}

		public static void IfMatches(this string input, string pattern, Action<Matcher> ifTrue, RegexOptions options)
		{
			var matcher = new Matcher();
			if (matcher.IsMatch(input, pattern, options))
				ifTrue(matcher);
		}

		public static void IfMatches(this string input, string pattern, Action<Matcher> ifTrue, Action ifFalse,
			bool ignoreCase = false, bool multiline = false)
		{
			var matcher = new Matcher();
			if (matcher.IsMatch(input, pattern, ignoreCase, multiline))
				ifTrue(matcher);
			else
				ifFalse();
		}

		public static void IfMatches(this string input, string pattern, Action<Matcher> ifTrue, Action ifFalse, RegexOptions options)
		{
			var matcher = new Matcher();
			if (matcher.IsMatch(input, pattern, options))
				ifTrue(matcher);
			else
				ifFalse();
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

		public static IMatched<RegularExpressions.Matcher.Match> MatchOne(this string input, string pattern, RegexOptions options)
		{
			return new Matcher().MatchOne(input, pattern, options);
		}

		public static IMatched<RegularExpressions.Matcher.Match> MatchOne(this string input, string pattern, bool ignoreCase = false,
			bool multiline = false)
		{
			return new Matcher().MatchOne(input, pattern, ignoreCase, multiline);
		}
	}
}