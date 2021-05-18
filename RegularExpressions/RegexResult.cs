using System;
using System.Text.RegularExpressions;
using Core.Arrays;
using Core.Numbers;
using Core.Strings;

namespace Core.RegularExpressions
{
   public class RegexResult
   {
      public static RegexResult Empty => new();

      protected RegexPattern pattern;
      protected string restOfText;
      protected int offset;

      internal RegexResult(string text, int index, int length, string[] groups, int itemIndex, RegexPattern pattern, string restOfText, int offset)
      {
         Text = text;
         Index = index;
         Length = length;
         Groups = groups;
         ItemIndex = itemIndex;
         this.pattern = pattern;
         this.restOfText = restOfText;
         this.offset = offset;

         IsMatch = true;
      }

      internal RegexResult(string text)
      {
         Text = text;
         Index = -1;
         Length = -1;
         Groups = Array.Empty<string>();
         ItemIndex = -1;
         pattern = new RegexPattern();
         restOfText = "";
         offset = -1;

         IsMatch = false;
      }

      internal RegexResult() : this("")
      {
      }

      public string Text { get; }

      public int Index { get; }

      public int Length { get; }

      public string[] Groups { get; }

      public int ItemIndex { get; }

      public bool IsMatch { get; }

      public string FirstGroup => Groups.Of(1).DefaultTo(() => "");

      public string SecondGroup => Groups.Of(2).DefaultTo(() => "");

      public string ThirdGroup => Groups.Of(3).DefaultTo(() => "");

      public string FourthGroup => Groups.Of(4).DefaultTo(() => "");

      public string FifthGroup => Groups.Of(5).DefaultTo(() => "");

      public string SixthGroup => Groups.Of(6).DefaultTo(() => "");

      public string SeventhGroup => Groups.Of(7).DefaultTo(() => "");

      public string EightGroup => Groups.Of(8).DefaultTo(() => "");

      public string NinthGroup => Groups.Of(9).DefaultTo(() => "");

      public string TenthGroup => Groups.Of(10).DefaultTo(() => "");

      public void Deconstruct(out string text, out int index, out int length, out string[] groups, out int itemIndex, out bool isMatch)
      {
         text = Text;
         index = Index;
         length = Length;
         groups = Groups;
         itemIndex = ItemIndex;
         isMatch = IsMatch;
      }

      public RegexResult MatchNext()
      {
         if (IsMatch)
         {
            var matcher = new Matcher(pattern.Friendly);
            if (matcher.IsMatch(restOfText, pattern.Pattern, pattern.Options))
            {
               var text = restOfText.Keep(matcher.Length);
               var index = matcher.Index + offset;
               var length = matcher.Length;
               var groups = matcher.Groups(0);
               var itemIndex = ItemIndex + 1;

               return new RegexResult(text, index, length, groups, itemIndex, pattern, restOfText.Drop(matcher.Index + matcher.Length),
                  offset + matcher.Index);
            }
         }

         return new RegexResult(restOfText);
      }

      public RegexResult MatchFirst(string pattern, RegexOptions options)
      {
         if (!pattern.StartsWith("^"))
         {
            pattern = $"^{pattern}";
         }

         var matcher = new Matcher(this.pattern.Friendly);
         if (matcher.IsMatch(Text, pattern, options))
         {
            return new RegexResult(matcher.FirstMatch, matcher.Index, matcher.Length, matcher.Groups(0), 0,
               new RegexPattern(pattern, options, this.pattern.Friendly), Text.Drop(Length), matcher.Index);
         }
         else
         {
            return new RegexResult(Text);
         }
      }

      public RegexResult MatchFirst(string pattern, bool ignoreCase = false, bool multiline = false)
      {
         Bits32<RegexOptions> options = RegexOptions.None;
         options[RegexOptions.IgnoreCase] = ignoreCase;
         options[RegexOptions.Multiline] = multiline;

         return MatchFirst(pattern, options);
      }

      public RegexResult MatchFirst(RegexPattern regexPattern)
      {
         var newPattern = regexPattern.Pattern;
         if (!newPattern.StartsWith("^"))
         {
            newPattern = $"^{regexPattern}";
         }

         regexPattern = regexPattern.WithPattern(newPattern);

         var matcher = new Matcher(regexPattern.Friendly);
         if (matcher.IsMatch(Text, regexPattern))
         {
            return new RegexResult(matcher.FirstMatch, matcher.Index, matcher.Length, matcher.Groups(0), 0, regexPattern, Text.Drop(Length),
               matcher.Index);
         }
         else
         {
            return new RegexResult(Text);
         }
      }
   }
}