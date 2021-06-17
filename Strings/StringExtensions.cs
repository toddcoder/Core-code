﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Core.Assertions;
using Core.Dates.Now;
using Core.Enumerables;
using Core.Monads;
using Core.Numbers;
using Core.RegexMatching;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;
using static Core.Strings.StringFunctions;
using static Core.Strings.StringStreamFunctions;

namespace Core.Strings
{
   public static class StringExtensions
   {
      private enum StageType
      {
         LeftNotFound,
         LeftFound
      }

      private const string PAIRS = "()[]{}<>";

      public static string Repeat(this string source, int count)
      {
         if (source.IsEmpty())
         {
            return string.Empty;
         }

         var result = new StringBuilder();
         for (var i = 0; i < count; i++)
         {
            result.Append(source);
         }

         return result.ToString();
      }

      public static string Repeat(this string source, int count, int maxLength)
      {
         if (source.IsEmpty())
         {
            return string.Empty;
         }

         var result = source.Repeat(count);
         return result.Length <= maxLength ? result : result.Substring(0, maxLength);
      }

      public static string Repeat(this string source, int count, string connector)
      {
         if (source.IsEmpty())
         {
            return string.Empty;
         }

         var result = new List<string>();
         for (var i = 0; i < count; i++)
         {
            result.Add(source);
         }

         return result.ToString(connector.ToNonNullString());
      }

      public static string Repeat(this string source, int count, int maxLength, string connector)
      {
         var result = source.Repeat(count, connector);
         return result.Length <= maxLength ? result : result.Substring(0, maxLength);
      }

      public static string[] Lines(this string source, SplitType split)
      {
         return source.IsEmpty() ? new string[0] : source.Split(splitPattern(split));
      }

      public static string[] Lines(this string source) => source.Split("/r/n | /r | /n");

      public static string Slice(this string source, int startIndex, int stopIndex)
      {
         if (source.IsEmpty())
         {
            return string.Empty;
         }

         if (startIndex < 0)
         {
            startIndex = 0;
         }

         if (stopIndex > source.Length - 1)
         {
            stopIndex = source.Length - 1;
         }

         var length = stopIndex - startIndex + 1;
         return source.Drop(startIndex).Keep(length);
      }

      public static string Slice(this string source, IMaybe<int> startIndex, IMaybe<int> stopIndex)
      {
         return source.Slice(startIndex.DefaultTo(() => 0), stopIndex.DefaultTo(() => source.Length - 1));
      }

      public static string Truncate(this string source, int limit, bool addEllipses = true)
      {
         if (source.IsEmpty() || limit <= 0)
         {
            return string.Empty;
         }
         else if (source.Length <= limit)
         {
            return source;
         }
         else if (addEllipses)
         {
            return source.Keep(limit - 1) + "…";
         }
         else
         {
            return source.Keep(limit);
         }
      }

      public static string Elliptical(this string source, int limit, char upTo, bool pad = false, string ellipses = "…")
      {
         if (source.IsEmpty() || limit <= 0)
         {
            return string.Empty;
         }
         else if (source.Length > limit)
         {
            var index = source.LastIndexOf(upTo);
            if (index == -1)
            {
               return source.Truncate(limit);
            }
            else
            {
               var ellipsesLength = ellipses.Length;
               var suffix = source.Keep(-(source.Length - index));
               var prefix = source.Keep(limit - (1 + suffix.Length));
               if (!source.StartsWith(prefix))
               {
                  prefix = $"{ellipses}{prefix.Drop(ellipsesLength)}";
               }

               var result = $"{prefix}{ellipses}{suffix}";
               if (result.Length > limit)
               {
                  result = $"{result.Keep(limit - ellipsesLength)}{ellipses}";
               }

               return result;
            }
         }
         else if (source.Length < limit && pad)
         {
            return source.LeftJustify(limit);
         }
         else
         {
            return source;
         }
      }

      public static string Elliptical(this string source, int limit, string upTo, bool pad = false, string ellipses = "…")
      {
         var upToChars = upTo.ToArray();
         if (source.IsEmpty() || limit <= 0)
         {
            return string.Empty;
         }
         else if (source.Length > limit)
         {
            var index = source.LastIndexOfAny(upToChars);
            if (index == -1)
            {
               return source.Truncate(limit);
            }
            else
            {
               var ellipsesLength = ellipses.Length;
               var suffix = source.Keep(-(source.Length - index));
               var prefix = source.Keep(limit - (1 + suffix.Length));
               if (!source.StartsWith(prefix))
               {
                  prefix = $"{ellipses}{prefix.Drop(ellipsesLength)}";
               }

               var result = $"{prefix}{ellipses}{suffix}";
               if (result.Length > limit)
               {
                  result = $"{result.Keep(limit - ellipsesLength)}{ellipses}";
               }

               return result;
            }
         }
         else if (source.Length < limit && pad)
         {
            return source.LeftJustify(limit);
         }
         else
         {
            return source;
         }
      }

      private static string replaceWhitespace(string source, string replacement)
      {
         return source.Map(s => string.Join(replacement, s.Split("/s+")));
      }

      public static string NormalizeWhitespace(this string source) => replaceWhitespace(source, " ");

      public static string RemoveWhitespace(this string source) => replaceWhitespace(source, string.Empty);

      public static string ToTitleCase(this string source)
      {
         return source.Map(s => new CultureInfo("en-US").TextInfo.ToTitleCase(s.ToLower()));
      }

      public static string CamelToLowerWithSeparator(this string source, string separator, string quantitative = "+")
      {
         if (source.IsEmpty())
         {
            return string.Empty;
         }

         if (source.IsMatch("^ {A-Z} $"))
         {
            return source.ToLower();
         }
         else
         {
            var matches = source.Matched($"-/b /(['A-Z']{quantitative})");
            if (matches.If(out var result))
            {
               for (var i = 0; i < result.MatchCount; i++)
               {
                  result[i, 1] = separator + result[i, 1];
               }

               return result.ToString().ToLower();
            }
            else
            {
               return source.ToLower();
            }
         }
      }

      public static string CamelToSnakeCase(this string source, string quantitative = "+")
      {
         return source.Map(s => s.CamelToLowerWithSeparator("_", quantitative));
      }

      public static string CamelToObjectGraphCase(this string source, string quantitative = "+")
      {
         return source.Map(s => s.CamelToLowerWithSeparator("-", quantitative));
      }

      public static string LowerToCamelCase(this string source, string separator, bool upperCase)
      {
         if (source.IsEmpty())
         {
            return string.Empty;
         }
         else
         {
            var pattern = separator.Escape().SingleQuotify() + "/(['a-z'])";
            var matches = source.Matched(pattern);
            if (matches.If(out var result))
            {
               for (var i = 0; i < result.MatchCount; i++)
               {
                  result[i] = result[i, 1].ToUpper();
               }

               return upperCase ? result.ToString().ToUpper1() : result.ToString().ToLower1();
            }
            else
            {
               return upperCase ? source.ToUpper1() : source.ToLower1();
            }
         }
      }

      public static string SnakeToCamelCase(this string source, bool upperCase) => source.LowerToCamelCase("_", upperCase);

      public static string Abbreviate(this string source)
      {
         if (source.IsEmpty())
         {
            return string.Empty;
         }

         var text = source;
         if (text.Has("_"))
         {
            text = text.SnakeToCamelCase(true);
         }

         Pattern pattern = "['A-Z'] ['a-z']*";
         if (pattern.MatchedBy(text).If(out var result))
         {
            Pattern numericPattern = "/d+ $";
            var numeric = string.Empty;
            if (numericPattern.MatchedBy(text).If(out var numericResult))
            {
               numeric = numericResult[0];
            }

            var builder = new StringBuilder();
            Pattern subPattern = "^ /uc? /lc* [/lv 'y' /uv 'Y']+ /(/lc?) /1? ['h']? ('e' $)?";
            for (var i = 0; i < result.MatchCount; i++)
            {
               if (subPattern.MatchedBy(result[i, 0]).If(out var subResult))
               {
                  for (var j = 0; j < subResult.MatchCount; j++)
                  {
                     builder.Append(fixMatch(result[i, 0], subResult[j, 0]));
                  }
               }
               else
               {
                  builder.Append(result[0, 0]);
               }

               if (result[0, 0].EndsWith("s"))
               {
                  builder.Append("s");
               }
            }

            return builder + numeric;
         }
         else
         {
            return text;
         }
      }

      private static string fixMatch(string match, string subMatch)
      {
         if (match.IsMatch("^ 'assign' /w+"))
         {
            return "Assign";
         }
         else if (match.IsMatch("^ 'institut' /w+"))
         {
            return "Inst";
         }
         else if (match.IsMatch("^ 'cust'"))
         {
            return "Cust";
         }
         else if (match.IsMatch("'id' $"))
         {
            return "ID";
         }
         else if (match.IsMatch("/b 'image' /b"))
         {
            return "Img";
         }
         else if (match.IsMatch("'company'"))
         {
            return "Cpny";
         }
         else if (match.IsMatch("'user'"))
         {
            return "Usr";
         }
         else if (match.IsMatch("'order'"))
         {
            return "Ord";
         }
         else if (match.IsMatch("'history'"))
         {
            return "Hist";
         }
         else if (match.IsMatch("'account'"))
         {
            return "Acct";
         }
         else if (match.IsMatch("^ 'stag' ('e' | 'ing')"))
         {
            return "Stg";
         }
         else if (match.IsMatch("'import'"))
         {
            return "Imp";
         }
         else if (match.IsMatch("'message' 's'?"))
         {
            return "Msg";
         }
         else
         {
            return subMatch;
         }
      }

      public static string Append(this string target, string text) => string.IsNullOrEmpty(target) ? text : target + text;

      public static string Passwordify(this string source)
      {
         if (source.IsEmpty())
         {
            return string.Empty;
         }

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
         {
            return string.Empty;
         }

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

      public static string Copy(this string source) => string.Copy(source.IsEmpty() ? string.Empty : source);

      public static string RemoveCComments(this string source)
      {
         return source.Map(s => s.Substitute("'/*' .*? '*/'", string.Empty));
      }

      public static string RemoveCOneLineComments(this string source)
      {
         return source.Map(s => s.Substitute("'//' .*? /crlf; m", string.Empty));
      }

      public static string RemoveSQLOneLineComments(this string source)
      {
         return source.Map(s => s.Substitute("'--' .*? /crlf; m", string.Empty));
      }

      public static string AllowOnly(this string source, string allowed)
      {
         if (source.IsEmpty())
         {
            return string.Empty;
         }

         if (!allowed.IsMatch("'[' -[']']+ ']'"))
         {
            allowed = $"['{allowed}']";
         }

         var matcher = allowed;
         if (matcher.Matched(source).If(out var result))
         {
            var builder = new StringBuilder();
            for (var i = 0; i < result.MatchCount; i++)
            {
               builder.Append(result[i, 0]);
            }

            return builder.ToString();
         }
         else
         {
            return string.Empty;
         }
      }

      public static string AllowOnly(this string source, params char[] allowed)
      {
         if (source.IsEmpty())
         {
            return string.Empty;
         }

         var characters = new StringBuilder();
         foreach (var character in allowed)
         {
            characters.Append(character);
         }

         return source.AllowOnly(characters.ToString());
      }

      public static string Quotify(this string source, string quote) => source.Map(s => $"\"{s.Replace("\"", quote)}\"");

      public static string SingleQuotify(this string source, string quote) => source.Map(s => $"'{s.Replace("'", quote)}'");

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
         return source.Map(s => s.Matches("^ /dq /(.*) /dq $").Map(result => result.FirstGroup).DefaultTo(() => s));
      }

      public static string SingleUnquotify(this string source)
      {
         return source.Map(s => source.Matches("^ /sq /(.*) /sq $").Map(result => result.FirstGroup).DefaultTo(() => s));
      }

      public static string Guillemetify(this string source) => source.Map(s => $"«{s}»");

      public static string VisibleWhitespace(this string source, bool spaceVisible)
      {
         if (source.IsEmpty())
         {
            return string.Empty;
         }

         if (spaceVisible)
         {
            source = source.Substitute("' '", "•");
         }

         source = source.Substitute("/t", "¬");
         source = source.Substitute("/n", "¶");
         source = source.Substitute("/r", "¤");

         return source;
      }

      public static string VisibleTabs(this string source) => source.Map(s => s.Substitute("/t", "¬"));

      public static string PadCenter(this string source, int length, char paddingCharacter = ' ')
      {
         if (source.IsEmpty())
         {
            return paddingCharacter.ToString().Repeat(length);
         }
         else
         {
            var sourceLength = source.Length;
            if (sourceLength < length)
            {
               var difference = length - sourceLength;
               var half = difference / 2;
               var padRight = difference.IsEven() ? half : half + 1;
               var paddingString = paddingCharacter.ToString();

               return paddingString.Repeat(half) + source + paddingString.Repeat(padRight);
            }
            else
            {
               return source;
            }
         }
      }

      public static string Pad(this string source, PadType padType, int length, char paddingCharacter = ' ') => padType switch
      {
         PadType.Center => source.PadCenter(length, paddingCharacter),
         PadType.Left => source.PadLeft(length, paddingCharacter),
         PadType.Right => source.PadRight(length, paddingCharacter),
         _ => source
      };

      public static string Pad(this string source, int width, char paddingCharacter = ' ')
      {
         return source.Map(s => width > 0 ? s.PadLeft(width, paddingCharacter) : s.PadRight(-width, paddingCharacter));
      }

      public static string Justify(this object source, Justification justification, int width, char paddingCharacter = ' ')
      {
         var asString = source.ToNonNullString();

         if (asString.IsEmpty())
         {
            return string.Empty;
         }

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
               return asString;
         }

         return asString.Pad(padType, width, paddingCharacter);
      }

      public static string LeftJustify(this object source, int width, char paddingChar = ' ')
      {
         return source.Justify(Justification.Left, width, paddingChar);
      }

      public static string RightJustify(this object source, int width, char paddingChar = ' ')
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
         {
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
         }

         return result.ToString();
      }

      public static string Debyteify(this string source, Encoding encoding) => source.Map(s => encoding.GetBytes(s).Debyteify());

      public static string Debyteify(this string source) => source.Debyteify(Encoding.ASCII);

      public static string DefaultTo(this string source, string defaultValue) => source.IsEmpty() ? defaultValue : source;

      public static string TrimLeft(this string source) => source.Map(s => s.TrimStart());

      public static string TrimRight(this string source) => source.Map(s => s.TrimEnd());

      public static string TrimAll(this string source) => source.Map(s => s.Trim());

      public static string ToUpper1(this string source) => source.Map(s => s.Keep(1).ToUpper() + s.Drop(1));

      public static string ToLower1(this string source) => source.Map(s => s.Keep(1).ToLower() + s.Drop(1));

      public static string Plural(this string source, int number)
      {
         if (source.IsEmpty())
         {
            return string.Empty;
         }

         Pattern pattern = "'(' /(/w+) (',' /(/w+))? ')'; i";
         if (pattern.MatchedBy(source).If(out var result))
         {
            if (number == 1)
            {
               for (var matchIndex = 0; matchIndex < result.MatchCount; matchIndex++)
               {
                  if (result[matchIndex, 2].IsNotEmpty())
                  {
                     result[matchIndex] = result[matchIndex, 1];
                  }
                  else
                  {
                     result[matchIndex] = string.Empty;
                  }
               }
            }
            else
            {
               for (var matchIndex = 0; matchIndex < result.MatchCount; matchIndex++)
               {
                  if (result[matchIndex, 2].IsNotEmpty())
                  {
                     result[matchIndex] = result[matchIndex, 2];
                  }
                  else
                  {
                     result[matchIndex] = result[matchIndex, 1];
                  }
               }
            }

            var matcherText = result.ToString();
            var numberAccountedFor = false;
            pattern = @"-(< '\') /'#'";
            if (pattern.MatchedBy(matcherText).If(out result))
            {
               numberAccountedFor = true;
               for (var matchIndex = 0; matchIndex < result.MatchCount; matchIndex++)
               {
                  result[matchIndex, 1] = number.ToString();
               }
            }

            matcherText = result.ToString();
            pattern = @"/('\#')";
            if (pattern.MatchedBy(matcherText).If(out result))
            {
               for (var matchIndex = 0; matchIndex < result.MatchCount; matchIndex++)
               {
                  result[matchIndex, 1] = "#";
               }
            }

            matcherText = result.ToString();

            return numberAccountedFor ? matcherText : $"{number} {matcherText}";
         }
         else
         {
            return source;
         }
      }

      public static string Reverse(this string source) => source.Map(new string(source.Select(c => c).Reverse().ToArray()));

      public static string Succ(this string source)
      {
         if (source.IsNotEmpty())
         {
            var builder = new StringBuilder(source);
            return succ(builder, builder.Length - 1).ToString();
         }
         else
         {
            return source;
         }
      }

      private static StringBuilder succ(StringBuilder builder, int index)
      {
         while (true)
         {
            if (builder.Length != 0)
            {
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
               {
                  return new StringBuilder();
               }
            }
            else
            {
               return builder;
            }
         }
      }

      public static string Pred(this string source)
      {
         if (source.IsNotEmpty())
         {
            var builder = new StringBuilder(source);
            return pred(builder, builder.Length - 1).ToString();
         }
         else
         {
            return source;
         }
      }

      private static StringBuilder pred(StringBuilder builder, int index)
      {
         while (true)
         {
            if (builder.Length != 0)
            {
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
            }
            else
            {
               return builder;
            }
         }
      }

      public static bool IsNotEmpty(this string source) => !string.IsNullOrEmpty(source);

      public static bool IsEmpty(this string source) => string.IsNullOrEmpty(source);

      public static bool Has(this string source, string substring, bool ignoreCase = false)
      {
         if (source.IsNotEmpty() && substring.IsNotEmpty())
         {
            return ignoreCase ? source.ToLower().Contains(substring.ToLower()) : source.Contains(substring);
         }
         else
         {
            return false;
         }
      }

      public static bool IsWhitespace(this string source) => source.IsEmpty() || source.IsMatch("^ /s* $");

      public static bool IsQuoted(this string source) => !source.IsEmpty() && source.IsMatch("^ [quote] .*? [quote] $");

      public static bool Same(this string source, string comparison)
      {
         if (source.IsEmpty())
         {
            source = source.ToNonNullString();
         }

         if (comparison.IsEmpty())
         {
            comparison = comparison.ToNonNullString();
         }

         return string.Compare(source, comparison, StringComparison.OrdinalIgnoreCase) == 0;
      }

      public static bool IsGUID(this string source)
      {
         return !source.IsEmpty() && source.IsMatch("^ '{'? [/l /d]8 '-' [/l /d]4 '-' [/l /d]4 '-' [/l /d]4 '-' [/l /d]12 '}'? $");
      }

      public static bool In(this string source, params string[] comparisons) => !source.IsEmpty() && comparisons.Any(source.Same);

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
            {
               return false;
            }
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
            {
               return false;
            }
         }
         catch
         {
            return false;
         }
      }

      public static bool ToBool(this string source, bool defaultValue = false)
      {
         if (source.IsNotEmpty())
         {
            return source switch
            {
               "1" => true,
               "0" => false,
               _ => bool.TryParse(source, out var result) ? result : defaultValue
            };
         }
         else
         {
            return defaultValue;
         }
      }

      public static byte ToByte(this string source, byte defaultValue = 0)
      {
         if (source.IsNotEmpty())
         {
            return byte.TryParse(source, out var result) ? result : defaultValue;
         }
         else
         {
            return defaultValue;
         }
      }

      public static int ToInt(this string source, int defaultValue = 0)
      {
         if (source.IsNotEmpty())
         {
            return int.TryParse(source, out var result) ? result : defaultValue;
         }
         else
         {
            return defaultValue;
         }
      }

      public static long ToLong(this string source, long defaultValue = 0)
      {
         if (source.IsNotEmpty())
         {
            return long.TryParse(source, out var result) ? result : defaultValue;
         }
         else
         {
            return defaultValue;
         }
      }

      public static float ToFloat(this string source, float defaultValue = 0)
      {
         if (source.IsNotEmpty())
         {
            return float.TryParse(source, out var result) ? result : defaultValue;
         }
         else
         {
            return defaultValue;
         }
      }

      public static double ToDouble(this string source, double defaultValue = 0)
      {
         if (source.IsNotEmpty())
         {
            return double.TryParse(source, out var result) ? result : defaultValue;
         }
         else
         {
            return defaultValue;
         }
      }

      public static decimal ToDecimal(this string source, decimal defaultValue = 0)
      {
         if (source.IsNotEmpty())
         {
            return decimal.TryParse(source, out var result) ? result : defaultValue;
         }
         else
         {
            return defaultValue;
         }
      }

      public static DateTime ToDateTime(this string source, DateTime defaultValue)
      {
         if (source.IsNotEmpty())
         {
            return System.DateTime.TryParse(source, out var result) ? result : defaultValue;
         }
         else
         {
            return defaultValue;
         }
      }

      public static DateTime ToDateTime(this string source) => source.ToDateTime(System.DateTime.MinValue);

      public static Guid ToGuid(this string source)
      {
         if (source.IsNotEmpty())
         {
            return System.Guid.TryParse(source, out var guid) ? guid : System.Guid.Empty;
         }
         else
         {
            return System.Guid.Empty;
         }
      }

      public static IMaybe<bool> AsBool(this string source)
      {
         return source.IsNotEmpty() ? maybe(bool.TryParse(source, out var result), () => result) : none<bool>();
      }

      public static IResult<bool> Boolean(this string source) => tryTo(() => bool.Parse(source));

      public static IMaybe<byte> AsByte(this string source)
      {
         return source.IsNotEmpty() ? maybe(byte.TryParse(source, out var result), () => result) : none<byte>();
      }

      public static IResult<byte> Byte(this string source) => tryTo(() => byte.Parse(source));

      public static IMaybe<int> AsInt(this string source)
      {
         return source.IsNotEmpty() ? maybe(int.TryParse(source, out var result), () => result) : none<int>();
      }

      public static IResult<int> Int32(this string source) => tryTo(() => int.Parse(source));

      public static IMaybe<long> AsLong(this string source)
      {
         return source.IsNotEmpty() ? maybe(long.TryParse(source, out var result), () => result) : none<long>();
      }

      public static IResult<long> Int64(this string source) => tryTo(() => long.Parse(source));

      public static IMaybe<float> AsFloat(this string source)
      {
         return source.IsNotEmpty() ? maybe(float.TryParse(source, out var result), () => result) : none<float>();
      }

      public static IResult<float> Single(this string source) => tryTo(() => float.Parse(source));

      public static IMaybe<double> AsDouble(this string source)
      {
         return source.IsNotEmpty() ? maybe(double.TryParse(source, out var result), () => result) : none<double>();
      }

      public static IResult<double> Double(this string source) => tryTo(() => double.Parse(source));

      public static IMaybe<decimal> AsDecimal(this string source)
      {
         return source.IsNotEmpty() ? maybe(decimal.TryParse(source, out var result), () => result) : none<decimal>();
      }

      public static IResult<decimal> Decimal(this string source) => tryTo(() => decimal.Parse(source));

      public static IMaybe<DateTime> AsDateTime(this string source)
      {
         return source.IsNotEmpty() ? maybe(System.DateTime.TryParse(source, out var result), () => result) : none<DateTime>();
      }

      public static IResult<DateTime> DateTime(this string source) => tryTo(() => System.DateTime.Parse(source));

      public static IMaybe<Guid> AsGuid(this string source)
      {
         return source.IsNotEmpty() ? maybe(System.Guid.TryParse(source, out var guid), () => guid) : none<Guid>();
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

      public static T ToEnumeration<T>(this string value, T defaultValue) where T : struct => value.ToEnumeration(true, defaultValue);

      public static IMaybe<T> AsEnumeration<T>(this string value, bool ignoreCase = true) where T : struct
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

      public static IResult<object> Enumeration(this string value, Type enumerationType, bool ignoreCase = true)
      {
         return tryTo(() => Enum.Parse(enumerationType, value, ignoreCase));
      }

      public static IResult<T> Enumeration<T>(this string value, bool ignoreCase = true) where T : struct
      {
         try
         {
            return ToEnumeration<T>(value, ignoreCase).Success();
         }
         catch (Exception exception)
         {
            return failure<T>(exception);
         }
      }

      public static string ExtractFromQuotes(this string source)
      {
         return source.Map(s => s.Matches("^ [quote] /(.*?) [quote] $").Map(result => result.FirstGroup).DefaultTo(() => s));
      }

      public static IMaybe<object> ToObject(this string value)
      {
         if (value == null)
         {
            return none<object>();
         }
         else if (value.IsQuoted())
         {
            return value.ExtractFromQuotes().Some<object>();
         }
         else if (value.IsIntegral())
         {
            var newValue = value.ToLong();
            return (newValue is >= int.MinValue and <= int.MaxValue ? (object)value.ToInt() : value).Some();
         }

         if (value.IsSingle())
         {
            Pattern pattern = "^ /(.+) ['fF'] $";
            if (pattern.MatchedBy(value).If(out var result))
            {
               return some<float, object>(result.FirstGroup.ToFloat());
            }
         }

         if (value.IsDouble())
         {
            Pattern pattern = "^ /(.+) ['dD'] $";
            if (pattern.MatchedBy(value).If(out var result))
            {
               return some<double, object>(result.FirstGroup.ToDouble());
            }
         }

         if (value.IsDecimal())
         {
            Pattern pattern = "^ /(.+) ['mM'] $";
            if (pattern.MatchedBy(value).If(out var result))
            {
               return some<decimal, object>(result.FirstGroup.ToDecimal());
            }
         }

         if (value.IsGUID())
         {
            return some<Guid, object>(new Guid(value));
         }
         else if (value.IsDate())
         {
            return some<DateTime, object>(value.ToDateTime());
         }
         else if (value.Same("false") || value.Same("true"))
         {
            return some<bool, object>(value.ToBool());
         }
         else
         {
            return none<object>();
         }
      }

      public static IMaybe<Type> ToType(this string value)
      {
         if (value.IsEmpty())
         {
            return none<Type>();
         }
         else if (value.IsQuoted())
         {
            return typeof(string).Some();
         }
         else if (value.IsIntegral())
         {
            return typeof(int).Some();
         }
         else if (value.IsSingle())
         {
            return typeof(float).Some();
         }
         else if (value.IsDouble())
         {
            return typeof(double).Some();
         }
         else if (value.IsDecimal())
         {
            return typeof(decimal).Some();
         }
         else if (value.IsGUID())
         {
            return typeof(Guid).Some();
         }
         else if (value.IsDate())
         {
            return typeof(DateTime).Some();
         }
         else if (value.Same("false") || value.Same("true"))
         {
            return typeof(bool).Some();
         }
         else
         {
            return none<Type>();
         }
      }

      public static IMaybe<object> ToParsed(this string value, Type type)
      {
         if (value.IsEmpty())
         {
            return none<object>();
         }
         else if (type == typeof(string))
         {
            return value.ExtractFromQuotes().Some<object>();
         }
         else if (type == typeof(int))
         {
            return value.AsInt().CastAs<object>();
         }
         else if (type == typeof(float))
         {
            return value.AsFloat().CastAs<object>();
         }
         else if (type == typeof(double))
         {
            return value.AsDouble().CastAs<object>();
         }
         else if (type == typeof(decimal))
         {
            return value.AsDecimal().CastAs<object>();
         }
         else if (type == typeof(Guid))
         {
            return value.AsGuid().CastAs<object>();
         }
         else if (type == typeof(DateTime))
         {
            return value.AsDateTime().CastAs<object>();
         }
         else if (type == typeof(bool))
         {
            return value.AsBool().CastAs<object>();
         }
         else
         {
            return none<object>();
         }
      }

      public static IResult<object> AsObject(this string value)
      {
         if (value.IsEmpty())
         {
            return "value is null".Failure<object>();
         }
         else if (value.IsQuoted())
         {
            return value.ExtractFromQuotes().Success<object>();
         }
         else if (value.IsIntegral())
         {
            return
               from newValue in value.Int64()
               from assertion in newValue.Must().BeBetween(int.MinValue).And(int.MaxValue).OrFailure()
               select (object)(int)assertion;
         }

         if (value.IsSingle())
         {
            return
               from result in value.Matches("^ /(.+) ['fF'] $").Result("Single in invalid format")
               from floated in result.FirstGroup.Single()
               select (object)floated;
         }

         if (value.IsDouble())
         {
            return
               from result in value.Matches("^ /(.+) ['dD'] $").Result("Double in invalid format")
               from doubled in result.FirstGroup.Double()
               select (object)doubled;
         }

         if (value.IsDecimal())
         {
            return
               from result in value.Matches("^ /(.+) ['mM'] $").Result("Decimal in invalid format")
               from decimaled in result.FirstGroup.Decimal()
               select (object)decimaled;
         }

         if (value.IsGUID())
         {
            return value.Guid().Map(g => (object)g);
         }
         else if (value.IsDate())
         {
            return value.DateTime().Map(dt => (object)dt);
         }
         else if (value.Same("false") || value.Same("true"))
         {
            return value.Boolean().Map(b => (object)b);
         }
         else
         {
            return $"Couldn't determine type of {value}".Failure<object>();
         }
      }

      public static IResult<Type> Type(this string value)
      {
         if (value.IsEmpty())
         {
            return "value is null".Failure<Type>();
         }
         else if (value.IsQuoted())
         {
            return typeof(string).Success();
         }
         else if (value.IsIntegral())
         {
            return typeof(int).Success();
         }
         else if (value.IsSingle())
         {
            return typeof(float).Success();
         }
         else if (value.IsDouble())
         {
            return typeof(double).Success();
         }
         else if (value.IsDecimal())
         {
            return typeof(decimal).Success();
         }
         else if (value.IsGUID())
         {
            return typeof(Guid).Success();
         }
         else if (value.IsDate())
         {
            return typeof(DateTime).Success();
         }
         else if (value.Same("false") || value.Same("true"))
         {
            return typeof(bool).Success();
         }
         else
         {
            return $"Couldn't determine type of {value}".Failure<Type>();
         }
      }

      public static IResult<object> Parsed(this string value, Type type)
      {
         if (value.IsEmpty())
         {
            return "value is null".Failure<object>();
         }
         else if (type == typeof(string))
         {
            return value.ExtractFromQuotes().Success().CastAs<object>();
         }
         else if (type == typeof(int))
         {
            return value.Int32().CastAs<object>();
         }
         else if (type == typeof(float))
         {
            return value.Single().CastAs<object>();
         }
         else if (type == typeof(double))
         {
            return value.Double().CastAs<object>();
         }
         else if (type == typeof(decimal))
         {
            return value.Decimal().CastAs<object>();
         }
         else if (type == typeof(Guid))
         {
            return value.Guid().CastAs<object>();
         }
         else if (type == typeof(DateTime))
         {
            return value.DateTime().CastAs<object>();
         }
         else if (type == typeof(bool))
         {
            return value.Boolean().CastAs<object>();
         }
         else
         {
            return $"Couldn't determine type of {value}".Failure<object>();
         }
      }

      public static string ToNonNullString(this object value) => value?.ToString() ?? string.Empty;

      public static IMaybe<string> ToIMaybeString(this object value) => maybe(value != null, value.ToString);

      public static string ToLiteral(this object value)
      {
         if (value == null)
         {
            return string.Empty;
         }

         if (value.IsFloat())
         {
            var result = value.ToString();
            if (!result.Has("."))
            {
               result += ".0";
            }

            if (value.IsDouble())
            {
               return result + "d";
            }
            else if (value.IsSingle())
            {
               return result + "f";
            }
            else if (value.IsDecimal())
            {
               return result + "m";
            }
            else
            {
               return result;
            }
         }
         else
         {
            return value.ToString();
         }
      }

      public static IMaybe<int> ExtractInt(this string source)
      {
         return maybe(source.IsNotEmpty(), () => source.Matches("/(['+-']? /d+)")).Map(result => result[0, 1].ToInt());
      }

      public static IMaybe<double> ExtractDouble(this string source)
      {
         return maybe(source.IsNotEmpty(), () => source.Matches("/(['+-']? /d* '.' /d* (['eE'] ['-+']? /d+)?)")).Map(result => result[0, 1].ToDouble());
      }

      public static IMaybe<char> First(this string source) => maybe(source.IsNotEmpty(), () => source[0]);

      public static IMaybe<char> Last(this string source) => maybe(source.IsNotEmpty(), () => source[source.Length - 1]);

      public static IMaybe<string> Left(this string source, int length)
      {
         var minLength = length.MinOf(source.Length);
         return maybe(minLength > 0, () => source.Keep(minLength));
      }

      public static IMaybe<string> Right(this string source, int length)
      {
         var minLength = Math.Min(length, source.Length);
         return maybe(source.IsNotEmpty() && minLength > 0, () => source.Drop(source.Length - minLength).Keep(minLength));
      }

      public static IMaybe<string> Sub(this string source, int index, int length)
      {
         return maybe(source.IsNotEmpty() && length > 0 && index >= 0 && index + length - 1 < source.Length, () => source.Drop(index).Keep(length));
      }

      public static IMaybe<string> Sub(this string source, int index)
      {
         return maybe(source.IsNotEmpty() && index >= 0 && index < source.Length, () => source.Drop(index));
      }

      public static bool IsDate(this string date) => System.DateTime.TryParse(date, out _);

      public static string FromBase64(this string source, Encoding encoding)
      {
         return encoding.GetString(Convert.FromBase64String(source));
      }

      public static byte[] FromBase64(this string source) => Convert.FromBase64String(source);

      public static string Head(this string source) => source.Keep(1);

      public static string Tail(this string source) => source.Drop(1);

      public static string Foot(this string source) => source.Keep(-1);

      public static string Front(this string source) => source.Drop(-1);

      public static IEnumerable<string> Enumerable(this string source) => source.Select(ch => ch.ToString());

      public static StringSegment Balanced(this string source, char left)
      {
         if (source.IsNotEmpty())
         {
            var delimitedText = DelimitedText.BothQuotes();

            var leftOfPairIndex = PAIRS.IndexOf(left);
            if (leftOfPairIndex != -1 && leftOfPairIndex.IsEven())
            {
               var right = PAIRS[leftOfPairIndex + 1];
               var parsed = delimitedText.Destringify(source, true);
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
                        {
                           count++;
                        }
                        else if (ch == right)
                        {
                           count--;
                           if (count == 0)
                           {
                              var segment = parsed.Slice(index, i);
                              segment = delimitedText.Restringify(segment, RestringifyQuotes.None);
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
            {
               return new StringSegment();
            }
         }
         else
         {
            return new StringSegment();
         }
      }

      //public static string[] Words(this string source) => source.IsEmpty() ? new string[0] : source.Split("/s+");

      public static string Drop(this string source, int count)
      {
         if (source.IsEmpty())
         {
            return string.Empty;
         }

         switch (count)
         {
            case > 0:
               count = count.MinOf(source.Length);
               return source.Substring(count);
            case 0:
               return source;
            default:
               count = (-count).MinOf(source.Length);
               return source.Substring(0, source.Length - count);
         }
      }

      public static string Drop(this string source, Pattern pattern)
      {
         if (source.IsEmpty())
         {
            return string.Empty;
         }

         if (pattern.MatchedBy(source).If(out var result))
         {
            var count = result.Index + result.Length;
            return source.Drop(count);
         }
         else
         {
            return source;
         }
      }

      public static string DropWhile(this string source, Predicate<string> predicate)
      {
         if (source.IsEmpty())
         {
            return string.Empty;
         }

         for (var i = 0; i < source.Length; i++)
         {
            var ch = source.Substring(i, 1);
            if (!predicate(ch))
            {
               return source.Slice(i, source.Length - 1);
            }
         }

         return source;
      }

      public static string DropWhile(this string source, string searchString, StringComparison comparisonType = StringComparison.CurrentCulture)
      {
         if (source.IsEmpty())
         {
            return string.Empty;
         }

         var index = source.LastIndexOf(searchString, comparisonType);
         return index > -1 ? Drop(source, index + searchString.Length) : source;
      }

      public static string DropWhile(this string source, params char[] chars)
      {
         if (source.IsEmpty())
         {
            return string.Empty;
         }

         for (var i = 0; i < source.Length; i++)
         {
            if (!chars.Contains(source[i]))
            {
               return source.Drop(i);
            }
         }

         return source;
      }

      public static string DropUntil(this string source, Predicate<string> predicate)
      {
         if (source.IsEmpty())
         {
            return string.Empty;
         }

         for (var i = 0; i < source.Length; i++)
         {
            var ch = source.Substring(i, 1);
            if (predicate(ch))
            {
               return source.Slice(i, source.Length - 1);
            }
         }

         return source;
      }

      public static string DropUntil(this string source, string searchString, StringComparison comparisonType = StringComparison.CurrentCulture)
      {
         if (source.IsEmpty())
         {
            return string.Empty;
         }

         var index = source.IndexOf(searchString, comparisonType);
         return index > -1 ? Drop(source, index) : string.Empty;
      }

      public static string DropUntil(this string source, params char[] chars)
      {
         if (source.IsEmpty())
         {
            return string.Empty;
         }

         for (var i = 0; i < source.Length; i++)
         {
            if (chars.Contains(source[i]))
            {
               return source.Drop(i);
            }
         }

         return source;
      }

      public static string Keep(this string source, int count)
      {
         if (source.IsEmpty())
         {
            return string.Empty;
         }

         switch (count)
         {
            case > 0:
               count = count.MinOf(source.Length);
               return source.Substring(0, count);
            case 0:
               return string.Empty;
            default:
               count = (-count).MinOf(source.Length);
               return source.Substring(source.Length - count);
         }
      }

      public static string Keep(this string source, Pattern pattern)
      {
         if (source.IsEmpty())
         {
            return string.Empty;
         }

         if (pattern.MatchedBy(source).If(out var result))
         {
            var count = result.Index + result.Length;
            return source.Keep(count);
         }
         else
         {
            return source;
         }
      }

      public static string KeepWhile(this string source, Predicate<string> predicate)
      {
         if (source.IsEmpty())
         {
            return string.Empty;
         }

         for (var i = 0; i < source.Length; i++)
         {
            var ch = source.Substring(i, 1);
            if (!predicate(ch))
            {
               return source.Slice(0, i - 1);
            }
         }

         return source;
      }

      public static string KeepWhile(this string source, string searchString, StringComparison comparisonType = StringComparison.CurrentCulture)
      {
         if (source.IsEmpty())
         {
            return string.Empty;
         }

         var index = source.LastIndexOf(searchString, comparisonType);
         return index > -1 ? Keep(source, index + searchString.Length) : string.Empty;
      }

      public static string KeepWhile(this string source, params char[] chars)
      {
         if (source.IsEmpty())
         {
            return string.Empty;
         }

         for (var i = 0; i < source.Length; i++)
         {
            if (!chars.Contains(source[i]))
            {
               return source.Keep(i);
            }
         }

         return source;
      }

      public static string KeepUntil(this string source, Predicate<string> predicate)
      {
         if (source.IsEmpty())
         {
            return string.Empty;
         }

         for (var i = 0; i < source.Length; i++)
         {
            var ch = source.Substring(i, 1);
            if (predicate(ch))
            {
               return source.Slice(0, i - 1);
            }
         }

         return source;
      }

      public static string KeepUntil(this string source, string searchString, StringComparison comparisonType = StringComparison.CurrentCulture)
      {
         if (source.IsEmpty())
         {
            return string.Empty;
         }

         var index = source.IndexOf(searchString, comparisonType);
         return index > -1 ? Keep(source, index) : source;
      }

      public static string KeepUntil(this string source, params char[] chars)
      {
         if (source.IsEmpty())
         {
            return string.Empty;
         }

         for (var i = 0; i < source.Length; i++)
         {
            if (chars.Contains(source[i]))
            {
               return source.Keep(i);
            }
         }

         return source;
      }

      public static string Map(this string source, Func<string, string> func)
      {
         return source.IsNotEmpty() ? func(source) : string.Empty;
      }

      [Obsolete]
      public static string Map(this string source, Func<string> func) => source.IsNotEmpty() ? func() : string.Empty;

      public static string Map(this string source, string replacement) => source.IsNotEmpty() ? replacement : string.Empty;

      public static string If(this string source, Predicate<string> predicate)
      {
         var builder = new StringBuilder();
         for (var i = 0; i < source.Length; i++)
         {
            var ch = source.Substring(i);
            if (predicate(ch))
            {
               builder.Append(ch);
            }
         }

         return builder.ToString();
      }

      public static string Unless(this string source, Predicate<string> predicate)
      {
         if (source.IsEmpty())
         {
            return string.Empty;
         }

         var builder = new StringBuilder();
         for (var i = 0; i < source.Length; i++)
         {
            var ch = source.Substring(i);
            if (!predicate(ch))
            {
               builder.Append(ch);
            }
         }

         return builder.ToString();
      }

      public static IMaybe<int> FromHex(this string source)
      {
         IMaybe<int> matches()
         {
            return source.Matches("^ '0x' /(['0-9a-fA-F']+) $").Map(m => int.Parse(m.FirstGroup, NumberStyles.HexNumber));
         }

         return maybe(source.IsNotEmpty(), matches);
      }

      public static IMaybe<string> GetSignature(this string parameterName)
      {
         return maybe(parameterName.IsNotEmpty(), () => parameterName.Matches("^ '@' /(.*) $").Map(m => m.FirstGroup.SnakeToCamelCase(true)));
      }

      public static IEnumerable<Slice> SlicesOf(this string source, string value, StringComparison comparison = StringComparison.CurrentCulture)
      {
         if (source.IsEmpty())
         {
            yield break;
         }

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

            yield return new Slice(text, start, length);
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
               {
                  str /= characters[rand.Next(length)];
               }
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
         {
            return string.Empty;
         }
      }

      public static string Replace(this string source, params (string, string)[] replacements)
      {
         if (source.IsEmpty())
         {
            return string.Empty;
         }

         var result = source;
         foreach (var replacement in replacements)
         {
            var (oldString, newString) = replacement;
            result = result.Replace(oldString, newString);
         }

         return result;
      }

      public static string ReplaceAll(this string source, params (string, string)[] replacements)
      {
         if (source.IsEmpty())
         {
            return string.Empty;
         }

         var builder = new StringBuilder(source);
         foreach (var (find, replace) in replacements)
         {
            builder.Replace(find, replace);
         }

         return builder.ToString();
      }

      public static IResult<long> ByteSize(this string source)
      {
         if (source.IsEmpty())
         {
            return "Source is empty".Failure<long>();
         }
         else if (source.Matches("^ /(/d+) /['kmg']? $").If(out var result))
         {
            var valueSource = result.FirstGroup;
            var suffix = result.SecondGroup;

            if (valueSource.AsLong().If(out var value))
            {
               if (suffix.IsEmpty())
               {
                  return value.Success();
               }

               value *= 1028;
               if (suffix == "k")
               {
                  return value.Success();
               }

               value *= 1028;
               return suffix == "m" ? value.Success() : (value * 1028).Success();
            }
            else
            {
               return $"{valueSource} can't be converted to a long".Failure<long>();
            }
         }
         else
         {
            return "Badly formatted source".Failure<long>();
         }
      }

      public static IMaybe<long> AsByteSize(this string source) => source.ByteSize().Map(l => l.Some()).Recover(_ => none<long>());

      public static long ToByteSize(this string source, long defaultValue = 0)
      {
         return source.AsByteSize().DefaultTo(() => defaultValue);
      }

      public static string Partition(this string source, int allowedLength, string splitPattern = @"-(< '\')','", int padding = 1)
      {
         if (source.IsEmpty())
         {
            return string.Empty;
         }

         var array = source.Split(splitPattern);
         var result = array.Select(e => $"{e}{" ".Repeat(padding)}").ToString(string.Empty).Trim();

         return result.Center(allowedLength).Elliptical(allowedLength, ' ');
      }

      public static IMaybe<int> Find(this string source, string substring, int startIndex = 0, bool ignoreCase = false)
      {
         if (source.IsNotEmpty() && substring.IsNotEmpty())
         {
            var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            var index = source.IndexOf(substring, startIndex, comparison);

            return maybe(index != -1, () => index);
         }
         else
         {
            return none<int>();
         }
      }

      public static IMaybe<int> FindBackward(this string source, string substring, int startIndex = -1, bool ignoreCase = false)
      {
         if (source.IsNotEmpty() && substring.IsNotEmpty())
         {
            if (startIndex == -1)
            {
               startIndex = source.Length - 1;
            }

            var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            var index = source.LastIndexOf(substring, startIndex, comparison);

            return maybe(index != -1, () => index);
         }
         else
         {
            return none<int>();
         }
      }

      public static IMaybe<Slice> FindByRegex(this string source, Pattern pattern)
      {
         if (pattern.MatchedBy(source).If(out var result))
         {
            var (text, index, length) = result.GetMatch(0);
            return new Slice(text, index, length).Some();
         }
         else
         {
            return none<Slice>();
         }
      }

      public static IEnumerable<int> FindAll(this string source, string substring, bool ignoreCase = false)
      {
         var result = source.Find(substring, 0, ignoreCase);
         while (true)
         {
            if (result.If(out var i))
            {
               yield return i;
            }
            else
            {
               break;
            }

            result = source.Find(substring, i + substring.Length, ignoreCase);
         }
      }

      public static IEnumerable<Slice> FindAllByRegex(this string source, Pattern pattern)
      {
         if (pattern.MatchedBy(source).If(out var result))
         {
            for (var i = 0; i < result.MatchCount; i++)
            {
               var (text, index, length) = result.GetMatch(i);
               yield return new Slice(text, index, length);
            }
         }
      }

      public static string ToCamel(this string source)
      {
         var pascal = source.ToPascal();
         return pascal.Keep(1).ToLower() + pascal.Drop(1);
      }

      public static string ToPascal(this string source)
      {
         static string allPascalCase(string word) => word.Keep(1).ToUpper() + word.Drop(1).ToLower();

         static IEnumerable<string> split(string whole)
         {
            if (whole.IsMatch("^ ['A-Z']+ $"))
            {
               whole = allPascalCase(whole);
            }
            else if (whole.IsMatch("'_'"))
            {
               whole = whole.Split("'_'+").Select(s => s.ToPascal()).ToString("");
            }

            var part = new StringBuilder();
            foreach (var ch in whole)
            {
               if (ch == '_')
               {
                  if (part.Length > 0)
                  {
                     yield return allPascalCase(part.ToString());
                  }

                  part.Clear();
               }
               else if (char.IsUpper(ch) || char.IsDigit(ch))
               {
                  if (part.Length > 0)
                  {
                     yield return part.ToString();
                  }

                  part.Clear();
                  part.Append(ch);
               }
               else if (part.Length == 0)
               {
                  part.Append(char.ToUpper(ch));
               }
               else
               {
                  part.Append(char.ToLower(ch));
               }
            }

            if (part.Length > 0)
            {
               yield return part.ToString();
            }
         }

         if (source.IsEmpty())
         {
            return string.Empty;
         }
         else if (source.IsMatch("^ ['0-9']+ $"))
         {
            return source;
         }

         return split(source).ToArray().ToString("");
      }

      public static IMaybe<bool> IsExactlyEqualTo(this string left, string right)
      {
         if (left == right)
         {
            return true.Some();
         }
         else if (left.Same(right))
         {
            return false.Some();
         }
         else
         {
            return none<bool>();
         }
      }

      public static string Exactly(this string source, int length, bool addEllipsis = true, bool leftJustify = true, bool normalizeWhitespace = true)
      {
         if (source.IsEmpty())
         {
            return " ".Repeat(length);
         }
         else
         {
            var normalized = normalizeWhitespace ? source.NormalizeWhitespace() : source;
            var sourceLength = normalized.Length;
            if (sourceLength <= length)
            {
               return leftJustify ? normalized.LeftJustify(length) : normalized.RightJustify(length);
            }
            else
            {
               var keepCount = length - (addEllipsis ? 3 : 0);
               var keep = normalized.Keep(keepCount);

               var ellipsis = addEllipsis ? "..." : string.Empty;

               return $"{keep}{ellipsis}";
            }
         }
      }

      public static string IndentedLines(this string source, int indentation = 3)
      {
         var indent = " ".Repeat(indentation);
         return source.Lines().Select(line => $"{indent}{line}").ToString("\r\n");
      }

      public static IEnumerable<string> Words(this string source)
      {
         var builder = new StringBuilder();

         foreach (var ch in source)
         {
            if (char.IsUpper(ch))
            {
               if (builder.Length > 0)
               {
                  yield return builder.ToString();

                  builder.Clear();
               }

               builder.Append(ch);
            }
            else if (char.IsLower(ch) || char.IsDigit(ch))
            {
               builder.Append(ch);
            }
            else if (ch == '_' || char.IsPunctuation(ch) || char.IsWhiteSpace(ch))
            {
               if (builder.Length > 0)
               {
                  yield return builder.ToString();

                  builder.Clear();
               }
            }
         }

         if (builder.Length > 0)
         {
            yield return builder.ToString();
         }
      }
   }
}