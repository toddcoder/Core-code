using System;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Collections;
using Core.Matching.Parsers;
using Core.Monads;
using Core.Numbers;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using RMatch = System.Text.RegularExpressions.Match;
using RGroup = System.Text.RegularExpressions.Group;
using RRegex = System.Text.RegularExpressions.Regex;

namespace Core.Matching
{
   public class Pattern
   {
      protected static bool isFriendly;

      public static implicit operator Pattern(string source)
      {
         var regex = new RRegex(@"\s*;\s*([icmsfu]{1,3})$", RegexOptions.IgnoreCase);
         var matches = regex.Matches(source);
         if (matches.Count > 0)
         {
            var match = matches[0];
            var group = match.Groups[0];
            var ignoreCase = false;
            var multiline = false;
            var options = group.Value;

            if (options.Contains("i"))
            {
               ignoreCase = true;
            }
            else if (options.Contains("c"))
            {
               ignoreCase = false;
            }

            if (options.Contains("m"))
            {
               multiline = true;
            }
            else if (options.Contains("s"))
            {
               multiline = false;
            }

            bool friendly;
            if (options.Contains("f"))
            {
               friendly = true;
            }
            else if (options.Contains("u"))
            {
               friendly = false;
            }
            else
            {
               friendly = isFriendly;
            }

            var pattern = source.Drop(-match.Length);
            Bits32<RegexOptions> regexOptions = RegexOptions.None;
            regexOptions[RegexOptions.IgnoreCase] = ignoreCase;
            regexOptions[RegexOptions.Multiline] = multiline;

            return new Pattern(pattern, regexOptions, friendly);
         }
         else
         {
            return new Pattern(source, RegexOptions.None, isFriendly);
         }
      }

      protected static StringHash<string> friendlyPatterns;

      static Pattern()
      {
         friendlyPatterns = new StringHash<string>(true);
         isFriendly = true;
      }

      internal static string getRegex(string source) => friendlyPatterns.Find(source, _ => new Parser().Parse(source), true);

      public static bool IsFriendly
      {
         get => isFriendly;
         set => isFriendly = value;
      }

      protected string regex;
      protected RegexOptions options;
      protected bool friendly;

      protected Pattern(string regex, RegexOptions options, bool friendly)
      {
         this.regex = friendly ? getRegex(regex) : regex;
         this.options = options;
         this.friendly = friendly;
      }

      public string Regex => regex;

      public RegexOptions Options => options;

      public bool Friendly => friendly;

      public IMatched<MatchResult> MatchedBy(string input)
      {
         try
         {
            var rregex = new RRegex(regex, options);
            var newMatches = rregex.Matches(input)
               .Cast<RMatch>()
               .Select((m, i) => new Match
               {
                  Index = m.Index,
                  Length = m.Length,
                  Text = m.Value,
                  Groups = m.Groups.Cast<RGroup>()
                     .Select((g, j) => new Group { Index = g.Index, Length = g.Length, Text = g.Value, Which = j })
                     .ToArray(),
                  Which = i
               });
            var matches = newMatches.ToArray();
            var slicer = new Slicer(input);
            var indexesToNames = new Hash<int, string>();
            var namesToIndexes = new StringHash<int>(true);
            foreach (var name in rregex.GetGroupNames())
            {
               if (RRegex.IsMatch(name, @"^[+-]?\d+$"))
               {
                  continue;
               }

               var index = rregex.GroupNumberFromName(name);
               indexesToNames.Add(index, name);
               namesToIndexes.Add(name, index);
            }

            if (matches.Length > 0)
            {
               return new MatchResult(matches, indexesToNames, namesToIndexes, slicer, input).Match();
            }
            else
            {
               return noMatch<MatchResult>();
            }
         }
         catch (Exception exception)
         {
            return failedMatch<MatchResult>(exception);
         }
      }

      public Pattern WithIgnoreCase(bool ignoreCase)
      {
         Bits32<RegexOptions> newOptions = options;
         newOptions[RegexOptions.IgnoreCase] = ignoreCase;

         return new Pattern(regex, newOptions, friendly);
      }

      public Pattern WithMultiline(bool multiline)
      {
         Bits32<RegexOptions> newOptions = options;
         newOptions[RegexOptions.Multiline] = multiline;

         return new Pattern(regex, newOptions, friendly);
      }

      public Pattern WithPattern(Func<string, string> patternFunc) => new(patternFunc(regex), options, false);
   }
}