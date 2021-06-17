using System;
using Core.Arrays;
using Core.Strings;

namespace Core.RegularExpressions
{
   [Obsolete("Use Result")]
   public class RegexResult
   {
      public static RegexResult Empty => new();

      protected RegexPattern pattern;
      protected string restOfText;
      protected int offset;

      internal RegexResult(string text, int index, int length, string[] groups, int itemIndex, Matcher.Match match, RegexPattern pattern,
         string restOfText, int offset)
      {
         Text = text;
         Index = index;
         Length = length;
         Groups = groups;
         ItemIndex = itemIndex;
         Match = match;
         this.pattern = pattern;
         this.restOfText = restOfText;
         this.offset = offset;

         IsMatch = true;
      }

      internal RegexResult(string text, string restOfText)
      {
         Text = text;
         Index = -1;
         Length = -1;
         Groups = Array.Empty<string>();
         ItemIndex = -1;
         Match = new Matcher.Match();
         pattern = new RegexPattern();
         this.restOfText = restOfText;
         offset = -1;

         IsMatch = false;
      }

      internal RegexResult() : this("", "")
      {
      }

      public string Text { get; }

      public int Index { get; }

      public int Length { get; }

      public string[] Groups { get; }

      public int ItemIndex { get; }

      public bool IsMatch { get; }

      public Matcher.Match Match { get; }

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

               restOfText = restOfText.Drop(matcher.Index + matcher.Length);
               var offsetText = offset + matcher.Index;
               return new RegexResult(text, index, length, groups, itemIndex, matcher.GetMatch(0), pattern, restOfText, offsetText);
            }
         }

         return new RegexResult(restOfText, restOfText);
      }

      public RegexResult Matches(string pattern)
      {
         if (!pattern.StartsWith("^"))
         {
            pattern = $"^{pattern}";
         }

         return Matches((RegexPattern)pattern);
      }

      public RegexResult Matches(RegexPattern regexPattern)
      {
         var newPattern = regexPattern.Pattern;
         if (!newPattern.StartsWith("^"))
         {
            newPattern = $"^{regexPattern}";
         }

         regexPattern = regexPattern.WithPattern(newPattern);

         var matcher = new Matcher(regexPattern.Friendly);
         if (matcher.IsMatch(restOfText, regexPattern))
         {
            var text = restOfText.Drop(matcher.Length);
            return new RegexResult(matcher.FirstMatch, matcher.Index, matcher.Length, matcher.Groups(0), 0, matcher.GetMatch(0), regexPattern, text,
               matcher.Index);
         }
         else
         {
            return new RegexResult(restOfText, restOfText);
         }
      }
   }
}