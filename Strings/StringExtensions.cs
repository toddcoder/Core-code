﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Core.Collections;
using Core.Dates.Now;
using Core.Enumerables;
using Core.Monads;
using Core.Numbers;
using Core.RegularExpressions;
using static Core.Booleans.Assertions;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;
using static Core.Strings.StringFunctions;
using static Core.Strings.StringStreamFunctions;

namespace Core.Strings
{
	public static class StringExtensions
	{
		enum StageType
		{
			LeftNotFound,
			LeftFound
		}

		const string PAIRS = "()[]{}<>";

		public static string Repeat(this string source, int count)
		{
			if (source.IsEmpty())
				return "";

			var result = new StringBuilder();
			for (var i = 0; i < count; i++)
				result.Append(source);

			return result.ToString();
		}

		public static string Repeat(this string source, int count, int maxLength)
		{
			if (source.IsEmpty())
				return "";

			var result = source.Repeat(count);
			return result.Length <= maxLength ? result : result.Substring(0, maxLength);
		}

		public static string Repeat(this string source, int count, string connector)
		{
			if (source.IsEmpty())
				return "";

			var result = new List<string>();
			for (var i = 0; i < count; i++)
				result.Add(source);

			return result.Stringify(connector.ToNonNullString());
		}

		public static string Repeat(this string source, int count, int maxLength, string connector)
		{
			var result = source.Repeat(count, connector);
			return result.Length <= maxLength ? result : result.Substring(0, maxLength);
		}

		public static string[] Lines(this string source, SplitType split)
		{
			if (source.IsEmpty())
				return new string[0];
			else
				return source.Split(splitPattern(split));
		}

		public static string Slice(this string source, int startIndex, int stopIndex)
		{
			if (source.IsEmpty())
				return "";

			if (startIndex < 0)
				startIndex = 0;
			if (stopIndex > source.Length - 1)
				stopIndex = source.Length - 1;

			var length = stopIndex - startIndex + 1;
			if (length > 0)
				return source.Substring(startIndex, length);
			else
				return "";
		}

		public static string Slice(this string source, IMaybe<int> startIndex, IMaybe<int> stopIndex)
		{
			return source.Slice(startIndex.DefaultTo(() => 0), stopIndex.DefaultTo(() => source.Length - 1));
		}

		public static string Truncate(this string source, int limit, bool addEllipses = true)
		{
			if (source.IsEmpty())
				return "";
			else if (source.Length <= limit)
				return source;
			else if (addEllipses)
				return source.Substring(0, limit - 1) + "…";
			else
				return source.Substring(0, limit);
		}

		public static string Elliptical(this string source, int limit, char upTo)
		{
			if (source.IsEmpty())
				return "";
			else if (source.Length > limit)
			{
				var index = source.LastIndexOf(upTo);
				if (index == -1)
					return source.Truncate(limit);
				else
				{
					var suffix = source.Keep(-(source.Length - index));
					var prefix = source.Keep(limit - (1 + suffix.Length));
					if (!source.StartsWith(prefix))
						prefix = $"…{prefix.Drop(1)}";
					var result = prefix + "…" + suffix;
					if (result.Length > limit)
						result = $"{result.Keep(limit - 1)}…";

					return result;
				}
			}
			else
				return source;
		}

		static string replaceWhitespace(string source, string replacement)
		{
			if (source.IsEmpty())
				return "";
			else
				return string.Join(replacement, source.Split("/s+"));
		}

		public static string NormalizeWhitespace(this string source) => replaceWhitespace(source, " ");

		public static string RemoveWhitespace(this string source) => replaceWhitespace(source, "");

		public static string ToTitleCase(this string source)
		{
			if (source.IsEmpty())
				return "";
			else
				return new CultureInfo("en-US").TextInfo.ToTitleCase(source.ToLower());
		}

		static string upCaseFirst(string source)
		{
			if (source.IsEmpty())
				return "";
			else
				return char.IsUpper(source[0]) ? source : char.ToUpper(source[0]) + source.Substring(1);
		}

		static bool nextIsLower(string source, int index)
		{
			if (source.IsEmpty())
				return false;
			else
				return index < source.Length - 1 && char.IsLower(source[index + 1]);
		}

		static bool nextIsUpper(string source, int index)
		{
			if (source.IsEmpty())
				return false;
			else
				return index < source.Length - 1 && char.IsUpper(source[index + 1]);
		}

		public static string CamelToLowerWithSeparator(this string source, string separator, string quantitative = "+")
		{
			if (source.IsEmpty())
				return "";
			else if (source.IsMatch("^ {A-Z} $"))
				return source.ToLower();
			else
			{
				var matches = source.Matches($"-/b /(['A-Z']{quantitative})");
				if (matches.If(out var matcher))
				{
					for (var i = 0; i < matcher.MatchCount; i++)
						matcher[i, 1] = separator + matcher[i, 1];

					return matcher.ToString().ToLower();
				}
				else
					return source.ToLower();
			}
		}

		public static string CamelToSnakeCase(this string source, string quantitative = "+")
		{
			if (source.IsEmpty())
				return "";
			else
				return source.CamelToLowerWithSeparator("_", quantitative);
		}

		public static string CamelToObjectGraphCase(this string source, string quantitative = "+")
		{
			if (source.IsEmpty())
				return "";
			else
				return source.CamelToLowerWithSeparator("-", quantitative);
		}

		public static string LowerToCamelCase(this string source, string separator, bool upperCase)
		{
			if (source.IsEmpty())
				return "";
			else
			{
				var pattern = separator.Escape().SingleQuotify() + "/(['a-z'])";
				var matches = source.Matches(pattern);
				if (matches.If(out var matcher))
				{
					for (var i = 0; i < matcher.MatchCount; i++)
						matcher[i] = matcher[i, 1].ToUpper();

					return upperCase ? matcher.ToString().ToUpper1() : matcher.ToString().ToLower1();
				}
				else
					return upperCase ? source.ToUpper1() : source.ToLower1();
			}
		}

		public static string SnakeToCamelCase(this string source, bool upperCase)
		{
			return source.LowerToCamelCase("_", upperCase);
		}

		public static string ObjectGraphToCamelCase(this string source, bool upperCase)
		{
			return source.LowerToCamelCase("-", upperCase);
		}

		public static string Abbreviate(this string source)
		{
			if (source.IsEmpty())
				return "";

			var text = source;
			if (text.Has("_"))
				text = text.SnakeToCamelCase(true);

			var matcher = new Matcher();
			if (matcher.IsMatch(text, "['A-Z'] ['a-z']*"))
			{
				var numericMatcher = new Matcher();
				var numeric = "";
				if (numericMatcher.IsMatch(text, "/d+ $"))
					numeric = numericMatcher[0];

				var result = new StringBuilder();
				var subMatcher = new Matcher();
				for (var i = 0; i < matcher.MatchCount; i++)
				{
					if (subMatcher.IsMatch(matcher[i, 0], "^ /uc? /lc* [/lv 'y' /uv 'Y']+ /(/lc?) /1? ['h']? ('e' $)?"))
						for (var j = 0; j < subMatcher.MatchCount; j++)
							result.Append(fixMatch(matcher[i, 0], subMatcher[j, 0]));
					else
						result.Append(matcher[0, 0]);

					if (matcher[0, 0].EndsWith("s"))
						result.Append("s");
				}

				return result + numeric;
			}
			else
				return text;
		}

		static string fixMatch(string match, string subMatch)
		{
			if (match.IsMatch("^ 'assign' /w+"))
				return "Assign";
			else if (match.IsMatch("^ 'institut' /w+"))
				return "Inst";
			else if (match.IsMatch("^ 'cust'"))
				return "Cust";
			else if (match.IsMatch("'id' $"))
				return "ID";
			else if (match.IsMatch("/b 'image' /b"))
				return "Img";
			else if (match.IsMatch("'company'"))
				return "Cpny";
			else if (match.IsMatch("'user'"))
				return "Usr";
			else if (match.IsMatch("'order'"))
				return "Ord";
			else if (match.IsMatch("'history'"))
				return "Hist";
			else if (match.IsMatch("'account'"))
				return "Acct";
			else if (match.IsMatch("^ 'stag' ('e' | 'ing')"))
				return "Stg";
			else if (match.IsMatch("'import'"))
				return "Imp";
			else if (match.IsMatch("'message' 's'?"))
				return "Msg";
			else
				return subMatch;
		}

		public static string Append(this string target, string text)
		{
			return string.IsNullOrEmpty(target) ? text : target + text;
		}

		public static string Passwordify(this string source)
		{
			if (source.IsEmpty())
				return "";

			source = source.ToLower().Replace("a", "@");
			source = source.Replace("b", "6");
			source = source.Replace("c", "(");
			source = source.Replace("e", "3");
			source = source.Replace("g", "8");
			source = source.Replace("h", "4");
			source = source.Replace("i", "1");
			source = source.Replace("l", "7");
			source = source.Replace("o", "0");
			source = source.Replace("q", "9");
			source = source.Replace("s", "$");
			source = source.Replace("t", "+");
			return source.Replace("z", "2");
		}

		public static string Depasswordify(this string source)
		{
			if (source.IsEmpty())
				return "";

			source = source.Replace("@", "a");
			source = source.Replace("6", "b");
			source = source.Replace("(", "c");
			source = source.Replace("3", "e");
			source = source.Replace("8", "g");
			source = source.Replace("4", "h");
			source = source.Replace("1", "i");
			source = source.Replace("7", "l");
			source = source.Replace("0", "o");
			source = source.Replace("9", "q");
			source = source.Replace("$", "s");
			source = source.Replace("+", "t");
			return source.Replace("2", "z");
		}

		public static string Copy(this string source) => string.Copy(source.IsEmpty() ? "" : source);

		public static string RemoveCComments(this string source)
		{
			if (source.IsEmpty())
				return "";
			else
				return source.Substitute("'/*' .*? '*/'", "");
		}

		public static string RemoveCOneLineComments(this string source)
		{
			if (source.IsEmpty())
				return "";
			else
				return source.Substitute("'//' .*? /crlf", "", false, true);
		}

		public static string RemoveSQLOneLineComments(this string source)
		{
			if (source.IsEmpty())
				return "";
			else
				return source.Substitute("'--' .*? /crlf", "", false, true);
		}

		public static string AllowOnly(this string source, string allowed)
		{
			if (source.IsEmpty())
				return "";

			if (!allowed.IsMatch("'[' -[']']+ ']'"))
				allowed = $"['{allowed}']";
			var matcher = new Matcher();
			if (matcher.IsMatch(source, allowed))
			{
				var result = new StringBuilder();
				for (var i = 0; i < matcher.MatchCount; i++)
					result.Append(matcher[i, 0]);

				return result.ToString();
			}
			else
				return "";
		}

		public static string AllowOnly(this string source, params char[] allowed)
		{
			if (source.IsEmpty())
				return "";

			var characters = new StringBuilder();
			foreach (var character in allowed)
				characters.Append(character);

			return source.AllowOnly(characters.ToString());
		}

		public static string Quotify(this string source, string quote)
		{
			if (source.IsEmpty())
				return "";
			else
				return $"\"{source.Replace("\"", quote)}\"";
		}

		public static string SingleQuotify(this string source, string quote)
		{
			if (source.IsEmpty())
				return "";
			else
				return $"'{source.Replace("'", quote)}'";
		}

		public static string Quotify(this string source) => source.Quotify(@"\""");

		public static string SingleQuotify(this string source) => source.SingleQuotify(@"\'");

		public static string QuotifyIf(this string source, string escapedQuote)
		{
			return source.IsNumeric() || source.Same("true") || source.Same("false") ? source : source.Quotify(escapedQuote);
		}

		public static string SingleQuotifyIf(this string source, string escapedQuote)
		{
			return source.IsNumeric() || source.Same("true") || source.Same("false") ? source : source.SingleQuotify(escapedQuote);
		}

		public static string QuotifyIf(this string source) => source.QuotifyIf(@"\""");

		public static string SingleQuotifyIf(this string source) => source.SingleQuotifyIf("\'");

		public static string Unquotify(this string source)
		{
			if (source.IsEmpty())
				return "";

			var matcher = new Matcher();
			if (matcher.IsMatch(source, "^ /dq /(.*) /dq $"))
				return matcher[0, 1];
			else
				return source;
		}

		public static string SingleUnquotify(this string source)
		{
			if (source.IsEmpty())
				return "";

			var matcher = new Matcher();
			if (matcher.IsMatch(source, "^ /sq /(.*) /sq $"))
				return matcher[0, 1];
			else
				return source;
		}

		public static string Guillemetify(this string source)
		{
			if (source.IsEmpty())
				return "";
			else
				return '«' + source + '»';
		}

		public static string VisibleWhitespace(this string source, bool spaceVisible)
		{
			if (source.IsEmpty())
				return "";

			if (spaceVisible)
				source = source.Substitute("' '", "•");
			source = source.Substitute("/t", "¬");
			source = source.Substitute("/n", "¶");
			source = source.Substitute("/r", "¤");

			return source;
		}

		public static string VisibleTabs(this string source)
		{
			if (source.IsEmpty())
				return "";
			else
				return source.Substitute("/t", "¬");
		}

		public static string PadCenter(this string source, int length, char paddingCharacter = ' ')
		{
			if (source.IsEmpty())
				return paddingCharacter.ToString().Repeat(length);
			else
			{
				var sourceLength = source.Length;
				if (sourceLength < length)
				{
					var difference = length - sourceLength;
					var half = difference / 2;
					var padLeft = half;
					var padRight = difference.IsEven() ? half : half + 1;
					var paddingString = paddingCharacter.ToString();

					return paddingString.Repeat(padLeft) + source + paddingString.Repeat(padRight);
				}
				else
					return source;
			}
		}

		public static string Pad(this string source, PadType padType, int length, char paddingCharacter = ' ')
		{
			switch (padType)
			{
				case PadType.Center:
					return source.PadCenter(length, paddingCharacter);
				case PadType.Left:
					return source.PadLeft(length, paddingCharacter);
				case PadType.Right:
					return source.PadRight(length, paddingCharacter);
				default:
					return source;
			}
		}

		public static string Pad(this string source, int width, char paddingCharacter = ' ')
		{
			if (source.IsEmpty())
				return "";
			else
				return width > 0 ? source.PadLeft(width, paddingCharacter) : source.PadRight(-width, paddingCharacter);
		}

		public static string Justify(this string source, Justification justification, int width, char paddingCharacter = ' ')
		{
			if (source.IsEmpty())
				return "";

			PadType padType;
			switch (justification)
			{
				case Justification.Left:
					padType = PadType.Right;
					break;
				case Justification.Right:
					padType = PadType.Left;
					break;
				case Justification.Center:
					padType = PadType.Center;
					break;
				default:
					return source;
			}

			return source.Pad(padType, width, paddingCharacter);
		}

		public static string LeftJustify(this string source, int width, char paddingChar = ' ')
		{
			return source.Justify(Justification.Left, width, paddingChar);
		}

		public static string RightJustify(this string source, int width, char paddingChar = ' ')
		{
			return source.Justify(Justification.Right, width, paddingChar);
		}

		public static string Center(this string source, int width, char paddingChar = ' ')
		{
			return source.Justify(Justification.Center, width, paddingChar);
		}

		public static string Debyteify(this byte[] source)
		{
			var result = new StringBuilder();
			var crLFEmitted = false;

			foreach (var character in source)
				if (character == 0 || character.Between((byte)32).And(126))
				{
					result.Append(Convert.ToChar(character));
					crLFEmitted = false;
				}
				else if (!crLFEmitted)
				{
					result.Append("\r\n");
					crLFEmitted = true;
				}

			return result.ToString();
		}

		public static string Debyteify(this string source, Encoding encoding)
		{
			if (source.IsEmpty())
				return "";
			else
				return encoding.GetBytes(source).Debyteify();
		}

		public static string Debyteify(this string source) => source.Debyteify(Encoding.ASCII);

		public static string DefaultTo(this string source, string defaultValue)
		{
			return source.IsEmpty() ? defaultValue : source;
		}

		public static string TrimLeft(this string source) => source.IsNotEmpty() ? source.TrimStart() : "";

		public static string TrimRight(this string source) => source.IsNotEmpty() ? source.TrimEnd() : "";

		public static string TrimAll(this string source) => source.IsNotEmpty() ? source.Trim() : "";

		public static string ToUpper1(this string source)
		{
			return source.IsNotEmpty() ? source.Keep(1).ToUpper() + source.Drop(1) : "";
		}

		public static string ToLower1(this string source)
		{
			return source.IsNotEmpty() ? source.Keep(1).ToLower() + source.Drop(1) : "";
		}

		public static string Plural(this string source, int number)
		{
			if (source.IsEmpty())
				return "";

			var matcher = new Matcher();
			if (matcher.IsMatch(source, "'(' /(/w+) (',' /(/w+))? ')'", true))
				if (number == 1)
				{
					if (matcher[0, 2].IsNotEmpty())
						matcher[0] = matcher[0, 1];
					else
						matcher[0] = "";
					return $"{number} {matcher}";
				}
				else
				{
					if (matcher[0, 2].IsNotEmpty())
						matcher[0] = matcher[0, 2];
					else
						matcher[0] = matcher[0, 1];
					return $"{number} {matcher}";
				}
			else
				return source;
		}

		public static string Reverse(this string source)
		{
			if (source.IsEmpty())
				return "";
			else
				return new string(source.Select(c => c).Reverse().ToArray());
		}

		public static string Succ(this string source)
		{
			if (source.IsNotEmpty())
			{
				var builder = new StringBuilder(source);
				return succ(builder, builder.Length - 1).ToString();
			}
			else
				return source;
		}

		static StringBuilder succ(StringBuilder builder, int index)
		{
			while (true)
				if (builder.Length != 0)
					if (index >= 0)
					{
						var ch = builder[index];
						switch (ch)
						{
							case '9':
								builder[index] = '0';
								if (index == 0)
								{
									builder.Insert(0, '0');
									return builder;
								}

								index--;
								continue;
							case 'Z':
								builder[index] = 'A';
								if (index == 0)
								{
									builder.Insert(0, 'A');
									return builder;
								}

								index--;
								continue;
							case 'z':
								builder[index] = 'a';
								if (index == 0)
								{
									builder.Insert(0, 'a');
									return builder;
								}

								index--;
								continue;
						}

						if (ch.ToString().IsMatch("[alnum]"))
						{
							builder[index] = (char)(builder[index] + 1);
							return builder;
						}

						index--;
					}
					else
						return new StringBuilder();
				else
					return builder;
		}

		public static string Pred(this string source)
		{
			if (source.IsNotEmpty())
			{
				var builder = new StringBuilder(source);
				return pred(builder, builder.Length - 1).ToString();
			}
			else
				return source;
		}

		static StringBuilder pred(StringBuilder builder, int index)
		{
			while (true)
				if (builder.Length != 0)
					if (index >= 0)
					{
						var ch = builder[index];
						switch (ch)
						{
							case '0':
								builder[index] = '9';
								index--;
								continue;
							case 'A':
								builder[index] = 'Z';
								index--;
								continue;
							case 'a':
								builder[index] = 'z';
								index--;
								continue;
						}

						if (ch.ToString().IsMatch("[alnum]"))
						{
							builder[index] = (char)(builder[index] - 1);
							return builder;
						}

						index--;
					}
					else
					{
						builder.Remove(0, 1);
						return builder;
					}
				else
					return builder;
		}

		public static bool IsNotEmpty(this string source) => !string.IsNullOrEmpty(source);

		public static bool IsEmpty(this string source) => string.IsNullOrEmpty(source);

		public static bool Has(this string source, string substring, bool ignoreCase = false)
		{
			if (source.IsNotEmpty() && substring.IsNotEmpty())
				return ignoreCase ? source.ToLower().Contains(substring.ToLower()) : source.Contains(substring);
			else
				return false;
		}

		public static bool IsWhitespace(this string source) => source.IsEmpty() || source.IsMatch("^ /s* $");

		public static bool IsQuoted(this string source)
		{
			if (source.IsEmpty())
				return false;
			else
				return source.IsMatch("^ [quote] .*? [quote] $");
		}

		public static bool Same(this string source, string comparison)
		{
			if (source.IsEmpty())
				source = source.ToNonNullString();
			if (comparison.IsEmpty())
				comparison = comparison.ToNonNullString();

			return string.Compare(source, comparison, StringComparison.OrdinalIgnoreCase) == 0;
		}

		public static bool IsGUID(this string source)
		{
			if (source.IsEmpty())
				return false;
			else
				return source.IsMatch("^ '{'? [/l /d]8 '-' [/l /d]4 '-' [/l /d]4 '-' [/l /d]4 '-' [/l /d]12 '}'? $");
		}

		public static bool In(this string source, params string[] comparisons)
		{
			if (source.IsEmpty())
				return false;
			else
				return comparisons.Any(source.Same);
		}

		public static bool IsConvertibleTo<T>(this string source)
		{
			try
			{
				if (source.IsNotEmpty())
				{
					var result = Convert.ChangeType(source, typeof(T));
					return result != null;
				}
				else
					return false;
			}
			catch
			{
				return false;
			}
		}

		public static bool IsConvertibleTo(this string source, Type type)
		{
			try
			{
				if (source.IsNotEmpty())
				{
					var result = Convert.ChangeType(source, type);
					return result != null;
				}
				else
					return false;
			}
			catch
			{
				return false;
			}
		}

		public static bool ToBool(this string source, bool defaultValue = false)
		{
			if (source.IsNotEmpty())
				switch (source)
				{
					case "1":
						return true;
					case "0":
						return false;
					default:
						return bool.TryParse(source, out var result) ? result : defaultValue;
				}
			else
				return defaultValue;
		}

		public static byte ToByte(this string source, byte defaultValue = 0)
		{
			if (source.IsNotEmpty())
				return byte.TryParse(source, out var result) ? result : defaultValue;
			else
				return defaultValue;
		}

		public static int ToInt(this string source, int defaultValue = 0)
		{
			if (source.IsNotEmpty())
				return int.TryParse(source, out var result) ? result : defaultValue;
			else
				return defaultValue;
		}

		public static long ToLong(this string source, long defaultValue = 0)
		{
			if (source.IsNotEmpty())
				return long.TryParse(source, out var result) ? result : defaultValue;
			else
				return defaultValue;
		}

		public static float ToFloat(this string source, float defaultValue = 0)
		{
			if (source.IsNotEmpty())
				return float.TryParse(source, out var result) ? result : defaultValue;
			else
				return defaultValue;
		}

		public static double ToDouble(this string source, double defaultValue = 0)
		{
			if (source.IsNotEmpty())
				return double.TryParse(source, out var result) ? result : defaultValue;
			else
				return defaultValue;
		}

		public static decimal ToDecimal(this string source, decimal defaultValue = 0)
		{
			if (source.IsNotEmpty())
				return decimal.TryParse(source, out var result) ? result : defaultValue;
			else
				return defaultValue;
		}

		public static DateTime ToDateTime(this string source, DateTime defaultValue)
		{
			if (source.IsNotEmpty())
				return System.DateTime.TryParse(source, out var result) ? result : defaultValue;
			else
				return defaultValue;
		}

		public static DateTime ToDateTime(this string source) => source.ToDateTime(System.DateTime.MinValue);

		public static Guid ToGuid(this string source)
		{
			if (source.IsNotEmpty())
				return System.Guid.TryParse(source, out var guid) ? guid : System.Guid.Empty;
			else
				return System.Guid.Empty;
		}

		public static IMaybe<bool> AsBool(this string source)
		{
			if (source.IsNotEmpty())
				return when(bool.TryParse(source, out var result), () => result);
			else
				return none<bool>();
		}

		public static IResult<bool> Boolean(this string source) => tryTo(() => bool.Parse(source));

		public static IMaybe<byte> AsByte(this string source)
		{
			if (source.IsNotEmpty())
				return when(byte.TryParse(source, out var result), () => result);
			else
				return none<byte>();
		}

		public static IResult<byte> Byte(this string source) => tryTo(() => byte.Parse(source));

		public static IMaybe<int> AsInt(this string source)
		{
			if (source.IsNotEmpty())
				return when(int.TryParse(source, out var result), () => result);
			else
				return none<int>();
		}

		public static IResult<int> Int32(this string source) => tryTo(() => int.Parse(source));

		public static IMaybe<long> AsLong(this string source)
		{
			if (source.IsNotEmpty())
				return when(long.TryParse(source, out var result), () => result);
			else
				return none<long>();
		}

		public static IResult<long> Int64(this string source) => tryTo(() => long.Parse(source));

		public static IMaybe<float> AsFloat(this string source)
		{
			if (source.IsNotEmpty())
				return when(float.TryParse(source, out var result), () => result);
			else
				return none<float>();
		}

		public static IResult<float> Single(this string source) => tryTo(() => float.Parse(source));

		public static IMaybe<double> AsDouble(this string source)
		{
			if (source.IsNotEmpty())
				return when(double.TryParse(source, out var result), () => result);
			else
				return none<double>();
		}

		public static IResult<double> Double(this string source) => tryTo(() => double.Parse(source));

		public static IMaybe<decimal> AsDecimal(this string source)
		{
			if (source.IsNotEmpty())
				return when(decimal.TryParse(source, out var result), () => result);
			else
				return none<decimal>();
		}

		public static IResult<decimal> Decimal(this string source) => tryTo(() => decimal.Parse(source));

		public static IMaybe<DateTime> AsDateTime(this string source)
		{
			if (source.IsNotEmpty())
				return when(System.DateTime.TryParse(source, out var result), () => result);
			else
				return none<DateTime>();
		}

		public static IResult<DateTime> DateTime(this string source) => tryTo(() => System.DateTime.Parse(source));

		public static IMaybe<Guid> AsGuid(this string source)
		{
			if (source.IsNotEmpty())
				return when(System.Guid.TryParse(source, out var guid), () => guid);
			else
				return none<Guid>();
		}

		public static IResult<Guid> Guid(this string source) => tryTo(() => System.Guid.Parse(source));

		public static string ToBase64(this string source, Encoding encoding)
		{
			return Convert.ToBase64String(encoding.GetBytes(source));
		}

		public static string ToBase64(this string source) => source.ToBase64(Encoding.ASCII);

		public static T ToEnumeration<T>(this string value, bool ignoreCase = true)
			where T : struct => (T)Enum.Parse(typeof(T), value, ignoreCase);

		public static T ToEnumeration<T>(this string value, bool ignoreCase, T defaultValue)
			where T : struct
		{
			try
			{
				return value.ToEnumeration<T>(ignoreCase);
			}
			catch
			{
				return defaultValue;
			}
		}

		public static Enum ToBaseEnumeration(this string value, Type enumerationType, bool ignoreCase = true)
		{
			return (Enum)Enum.Parse(enumerationType, value, ignoreCase);
		}

		public static Enum ToBaseEnumeration(this string value, Type enumerationType, bool ignoreCase, Enum defaultValue)
		{
			try
			{
				return (Enum)Enum.Parse(enumerationType, value, ignoreCase);
			}
			catch
			{
				return defaultValue;
			}
		}

		public static T ToEnumeration<T>(this string value, T defaultValue)
			where T : struct => value.ToEnumeration(true, defaultValue);

		public static IMaybe<T> AsEnumeration<T>(this string value, bool ignoreCase = true)
			where T : struct
		{
			try
			{
				return ToEnumeration<T>(value, ignoreCase).Some();
			}
			catch
			{
				return none<T>();
			}
		}

		public static string ExtractFromQuotes(this string source)
		{
			if (source.IsEmpty())
				return "";
			else
				return source.Matches("^ [quote] /(.*?) [quote] $").FlatMap(m => m[0, 1], () => source);
		}

		public static IMaybe<object> ToObject(this string value)
		{
			if (value == null)
				return none<object>();
			else if (value.IsQuoted())
				return value.ExtractFromQuotes().Some<object>();
			else if (value.IsIntegral())
			{
				var newValue = value.ToLong();
				return (newValue >= int.MinValue && newValue <= int.MaxValue ? (object)value.ToInt() : value).Some();
			}

			if (value.IsSingle())
			{
				var matcher = new Matcher();
				if (matcher.IsMatch(value, "^ /(.+) ['fF'] $"))
					return some<float, object>(matcher.FirstGroup.ToFloat());
			}

			if (value.IsDouble())
			{
				var matcher = new Matcher();
				if (matcher.IsMatch(value, "^ /(.+) ['dD'] $"))
					return some<double, object>(matcher[0, 1].ToDouble());
			}

			if (value.IsDecimal())
			{
				var matcher = new Matcher();
				if (matcher.IsMatch(value, "^ /(.+) ['mM'] $"))
					return some<decimal, object>(matcher[0, 1].ToDecimal());
			}

			if (value.IsGUID())
				return some<Guid, object>(new Guid(value));
			else if (value.IsDate())
				return some<DateTime, object>(value.ToDateTime());
			else if (value.Same("false") || value.Same("true"))
				return some<bool, object>(value.ToBool());
			else
				return none<object>();
		}

		public static string ToNonNullString(this object value) => value?.ToString() ?? "";

		public static IMaybe<string> ToIMaybeString(this object value) => when(value != null, value.ToString);

		public static string ToLiteral(this object value)
		{
			if (value == null)
				return "";
			else if (value.IsFloat())
			{
				var result = value.ToString();
				if (!result.Has("."))
					result += ".0";
				if (value.IsDouble())
					return result + "d";
				else if (value.IsSingle())
					return result + "f";
				else if (value.IsDecimal())
					return result + "m";
				else
					return result;
			}
			else
				return value.ToString();
		}

		public static IMaybe<int> ExtractInt(this string source)
		{
			if (source.IsEmpty())
				return none<int>();
			else
				return source.Matches("/(['+-']? /d+)").Map(m => m[0, 1].ToInt());
		}

		public static IMaybe<double> ExtractDouble(this string source)
		{
			if (source.IsEmpty())
				return none<double>();
			else
				return source.Matches("/(['+-']? /d* '.' /d* (['eE'] ['-+']? /d+)?)").Map(m => m[0, 1].ToDouble());
		}

		public static IMaybe<char> First(this string source) => when(source.IsNotEmpty(), () => source[0]);

		public static IMaybe<char> Last(this string source) => when(source.IsNotEmpty(), () => source[source.Length - 1]);

		public static IMaybe<string> Left(this string source, int length)
		{
			if (source.IsNotEmpty())
			{
				var minLength = Math.Min(length, source.Length);
				if (minLength > 0)
					return source.Substring(0, minLength).Some();
				else
					return none<string>();
			}
			else
				return none<string>();
		}

		public static IMaybe<string> Right(this string source, int length)
		{
			if (source.IsNotEmpty())
			{
				var minLength = Math.Min(length, source.Length);
				if (minLength > 0)
					return source.Substring(source.Length - minLength, minLength).Some();
				else
					return none<string>();
			}
			else
				return none<string>();
		}

		public static IMaybe<string> Sub(this string source, int index, int length)
		{
			if (source.IsNotEmpty() && length > 0 && index >= 0)
				return when(index + length - 1 < source.Length, () => source.Substring(index, length));
			else
				return none<string>();
		}

		public static IMaybe<string> Sub(this string source, int index)
		{
			return when(source.IsNotEmpty() && index >= 0 && index < source.Length, () => source.Substring(index));
		}

		public static bool IsDate(this string date) => System.DateTime.TryParse(date, out _);

		public static string FromBase64(this string source, Encoding encoding)
		{
			return encoding.GetString(Convert.FromBase64String(source));
		}

		public static byte[] FromBase64(this string source) => Convert.FromBase64String(source);

		public static string Head(this string source) => source.IsNotEmpty() ? source.Substring(0, 1) : "";

		public static string Tail(this string source) => source.IsNotEmpty() ? source.Substring(1) : "";

		public static string Foot(this string source)
		{
			return source.IsNotEmpty() ? source.Substring(source.Length - 1, 1) : "";
		}

		public static string Front(this string source)
		{
			return source.IsNotEmpty() ? source.Substring(0, source.Length - 1) : "";
		}

		public static IEnumerable<string> Enumerable(this string source) => new StringEnumerable(source);

		public static StringSegment Balanced(this string source, char left, int startIndex = 0)
		{
			if (source.IsNotEmpty() && startIndex < source.Length)
			{
				var destringifier = new Destringifier(source.Substring(startIndex));

				var leftOfPairIndex = PAIRS.IndexOf(left);
				if (leftOfPairIndex != -1 && leftOfPairIndex.IsEven())
				{
					var right = PAIRS[leftOfPairIndex + 1];
					var parsed = destringifier.Parse();
					var count = 0;
					var escaped = false;
					var type = StageType.LeftNotFound;
					var index = -1;

					for (var i = 0; i < parsed.Length; i++)
					{
						var ch = parsed[i];
						switch (type)
						{
							case StageType.LeftNotFound:
								if (ch == left)
								{
									count++;
									if (!escaped)
									{
										type = StageType.LeftFound;
										index = i;
									}
								}
								else if (ch == '/')
								{
									escaped = true;
									continue;
								}

								break;
							case StageType.LeftFound:
								if (ch == left)
									count++;
								else if (ch == right)
								{
									count--;
									if (count == 0)
									{
										var segment = parsed.Slice(index, i);
										segment = destringifier.Restring(segment, true);
										return new StringSegment(segment, index, i);
									}
								}

								break;
						}

						escaped = false;
					}

					return new StringSegment();
				}
				else
					return new StringSegment();
			}
			else
				return new StringSegment();
		}

		public static string[] Words(this string source)
		{
			if (source.IsEmpty())
				return new string[0];
			else
				return source.Split("/s+");
		}

		static string formatWith(string format, Hash<string, string> pairs)
		{
			if (format.IsEmpty())
				return "";

			var matcher = new Matcher();
			foreach (var (key, replacement) in pairs)
			{
				Assert(key.IsNotEmpty(), "Key must have a value");

				var pattern = "-(< '//') '(" + key.Escape() + ")'";
				if (matcher.IsMatch(format, pattern))
				{
					for (var j = 0; j < matcher.MatchCount; j++)
						matcher[j] = replacement;

					format = matcher.ToString();
				}
			}

			return replaceEscaped(format, matcher);
		}

		static string replaceEscaped(string format, Matcher matcher)
		{
			if (format.IsEmpty())
				return "";

			if (matcher.IsMatch(format, "'//('"))
			{
				for (var i = 0; i < matcher.MatchCount; i++)
					matcher[i] = "(";

				format = matcher.ToString();
			}

			return format;
		}

		public static string Drop(this string source, int count)
		{
			if (source.IsEmpty())
				return "";

			if (count > 0)
			{
				count = count.MinOf(source.Length);
				return source.Substring(count);
			}
			else if (count == 0)
				return source;
			else
			{
				count = (-count).MinOf(source.Length);
				return source.Substring(0, source.Length - count);
			}
		}

		public static string DropWhile(this string source, Predicate<string> predicate)
		{
			if (source.IsEmpty())
				return "";

			for (var i = 0; i < source.Length; i++)
			{
				var ch = source.Substring(i, 1);
				if (!predicate(ch))
					return source.Slice(i, source.Length - 1);
			}

			return source;
		}

		public static string DropWhile(this string source, string searchString,
			StringComparison comparisonType = StringComparison.CurrentCulture)
		{
			if (source.IsEmpty())
				return "";

			var index = source.LastIndexOf(searchString, comparisonType);
			return Drop(source, index + searchString.Length);
		}

		public static string DropWhile(this string source, params char[] chars)
		{
			if (source.IsEmpty())
				return "";

			for (var i = 0; i < source.Length; i++)
				if (!chars.Contains(source[i]))
					return source.Drop(i);

			return source;
		}

		public static string DropUntil(this string source, Predicate<string> predicate)
		{
			if (source.IsEmpty())
				return "";

			for (var i = 0; i < source.Length; i++)
			{
				var ch = source.Substring(i, 1);
				if (predicate(ch))
					return source.Slice(i, source.Length - 1);
			}

			return source;
		}

		public static string DropUntil(this string source, string searchString,
			StringComparison comparisonType = StringComparison.CurrentCulture)
		{
			if (source.IsEmpty())
				return "";

			var index = source.IndexOf(searchString, comparisonType);
			return Drop(source, index);
		}

		public static string DropUntil(this string source, params char[] chars)
		{
			if (source.IsEmpty())
				return "";

			for (var i = 0; i < source.Length; i++)
				if (chars.Contains(source[i]))
					return source.Drop(i);

			return source;
		}

		public static string Keep(this string source, int count)
		{
			if (source.IsEmpty())
				return "";
			else if (count > 0)
			{
				count = count.MinOf(source.Length);
				return source.Substring(0, count);
			}
			else if (count == 0)
				return "";
			else
			{
				count = (-count).MinOf(source.Length);
				return source.Substring(source.Length - count);
			}
		}

		public static string KeepWhile(this string source, Predicate<string> predicate)
		{
			if (source.IsEmpty())
				return "";

			for (var i = 0; i < source.Length; i++)
			{
				var ch = source.Substring(i, 1);
				if (!predicate(ch))
					return source.Slice(0, i - 1);
			}

			return source;
		}

		public static string KeepWhile(this string source, string searchString,
			StringComparison comparisonType = StringComparison.CurrentCulture)
		{
			if (source.IsEmpty())
				return "";

			var index = source.LastIndexOf(searchString, comparisonType);
			return Keep(source, index + searchString.Length);
		}

		public static string KeepWhile(this string source, params char[] chars)
		{
			if (source.IsEmpty())
				return "";

			for (var i = 0; i < source.Length; i++)
				if (!chars.Contains(source[i]))
					return source.Keep(i);

			return source;
		}

		public static string KeepUntil(this string source, Predicate<string> predicate)
		{
			if (source.IsEmpty())
				return "";

			for (var i = 0; i < source.Length; i++)
			{
				var ch = source.Substring(i, 1);
				if (predicate(ch))
					return source.Slice(0, i - 1);
			}

			return source;
		}

		public static string KeepUntil(this string source, string searchString,
			StringComparison comparisonType = StringComparison.CurrentCulture)
		{
			if (source.IsEmpty())
				return "";

			var index = source.IndexOf(searchString, comparisonType);
			return Keep(source, index);
		}

		public static string KeepUntil(this string source, params char[] chars)
		{
			if (source.IsEmpty())
				return "";

			for (var i = 0; i < source.Length; i++)
				if (chars.Contains(source[i]))
					return source.Keep(i);

			return source;
		}

		public static string Map(this string source, Func<string, string> func)
		{
			if (source.IsEmpty())
				return "";

			var builder = new StringBuilder();
			for (var i = 0; i < source.Length; i++)
				builder.Append(func(source.Substring(i)));

			return builder.ToString();
		}

		public static string If(this string source, Predicate<string> predicate)
		{
			var builder = new StringBuilder();
			for (var i = 0; i < source.Length; i++)
			{
				var ch = source.Substring(i);
				if (predicate(ch))
					builder.Append(ch);
			}

			return builder.ToString();
		}

		public static string Unless(this string source, Predicate<string> predicate)
		{
			if (source.IsEmpty())
				return "";

			var builder = new StringBuilder();
			for (var i = 0; i < source.Length; i++)
			{
				var ch = source.Substring(i);
				if (!predicate(ch))
					builder.Append(ch);
			}

			return builder.ToString();
		}

		public static IMaybe<int> FromHex(this string source)
		{
			if (source.IsEmpty())
				return none<int>();
			else
				return source.Matches("^ '0x' /(['0-9a-fA-F']+) $").Map(matcher => int.Parse(matcher.FirstGroup, NumberStyles.HexNumber));
		}

		public static IMaybe<string> GetSignature(this string parameterName)
		{
			if (parameterName.IsEmpty())
				return none<string>();
			else
				return parameterName.Matches("^ '@' /(.*) $").Map(m => m.FirstGroup.SnakeToCamelCase(true));
		}

		public static IEnumerable<Slice> SlicesOf(this string source, string value,
			StringComparison comparison = StringComparison.CurrentCulture)
		{
			if (source.IsEmpty())
				yield break;

			var index = 0;
			var list = new List<int> { 0 };

			while (index > -1)
			{
				index = source.IndexOf(value, index, comparison);
				if (index > -1)
				{
					list.Add(index);
					index += value.Length + 1;
				}
			}

			for (var i = 0; i < list.Count; i++)
			{
				var start = list[i];
				var length = i + 1 < list.Count ? list[i + 1] - start : source.Length - start;
				var text = source.Drop(start).Keep(length);
				yield return new Slice(start, length, text);
			}
		}

		public static string Obscure(this string source, char character) => character.Repeat(source?.Length ?? 0);

		public static string Obscure(this string source, string characters, bool random = false)
		{
			if (source.IsNotEmpty())
			{
				var length = characters.Length;
				var str = stream();

				if (random)
				{
					var rand = new Random(NowServer.Now.Millisecond);
					for (var i = 0; i < source.Length; i++)
						str /= characters[rand.Next(length)];
				}
				else
				{
					var index = 0;
					for (var i = 0; i < source.Length; i++)
					{
						str /= characters[index++];
						index %= length;
					}
				}

				return str;
			}
			else
				return "";
		}

		public static string Replace(this string source, params (string, string)[] replacements)
		{
			if (source.IsEmpty())
				return "";

			var result = source;
			foreach (var replacement in replacements)
			{
				var (oldString, newString) = replacement;
				result = result.Replace(oldString, newString);
			}

			return result;
		}

		public static string Extend(this string source, string before = "", string after = "")
		{
			if (source.IsNotEmpty())
				return $"{before}{source}{after}";
			else
				return "";
		}

		public static string ReplaceAll(this string source, params (string, string)[] replacements)
		{
			if (source.IsEmpty())
				return "";

			var builder = new StringBuilder(source);
			foreach (var (find, replace) in replacements)
				builder.Replace(find, replace);

			return builder.ToString();
		}

		public static IResult<long> ByteSize(this string source)
		{
			if (source.IsEmpty())
				return "Source is empty".Failure<long>();
			else if (source.Matches("^ /(/d+) /['kmg']? $", out var matcher))
			{
				var valueSource = matcher.FirstGroup;
				var suffix = matcher.SecondGroup;

				if (valueSource.AsLong().If(out var value))
				{
					if (suffix.IsEmpty())
						return value.Success();

					value *= 1028;
					if (suffix == "k")
						return value.Success();

					value *= 1028;
					if (suffix == "m")
						return value.Success();
					else
						return (value * 1028).Success();
				}
				else
					return $"{valueSource} can't be converted to a long".Failure<long>();
			}
			else
				return "Badly formatted source".Failure<long>();
		}

		public static IMaybe<long> AsByteSize(this string source) => source.ByteSize().FlatMap(l => l.Some(), e => none<long>());

		public static long ToByteSize(this string source, long defaultValue = 0)
		{
			return source.AsByteSize().FlatMap(l => l, () => defaultValue);
		}

		public static string Partition(this string source, int allowedLength, string splitPattern = @"-(< '\')','", int padding = 1)
		{
			if (source.IsEmpty())
				return "";

			var array = source.Split(splitPattern);
			var result = array.Select(e => $"{e}{" ".Repeat(padding)}").Stringify("").Trim();
			return result.Center(allowedLength).Elliptical(allowedLength, ' ');
		}

		public static IMaybe<int> Find(this string source, string substring, int startIndex = 0, bool ignoreCase = false)
		{
			if (substring.IsNotEmpty())
			{
				var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
				var index = source.IndexOf(substring, startIndex, comparison);

				return when(index != -1, () => index);
			}
			else
				return none<int>();
		}

		public static IEnumerable<int> FindAll(this string source, string substring, bool ignoreCase = false)
		{
			var result = source.Find(substring, 0, ignoreCase);
			while (true)
			{
				if (result.If(out var i))
					yield return i;
				else
					break;

				result = source.Find(substring, i + substring.Length, ignoreCase);
			}
		}
	}
}