using System;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Collections;
using Core.DataStructures;
using Core.Monads;
using Core.Numbers;
using Core.RegularExpressions.Parsers;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using RMatch = System.Text.RegularExpressions.Match;
using RGroup = System.Text.RegularExpressions.Group;
using RRegex = System.Text.RegularExpressions.Regex;

namespace Core.RegexMatching
{
   public class Matcher
   {
      protected static bool isFriendly;
      protected static MaybeStack<bool> friendlyStack;

      public static implicit operator Matcher(string source)
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

            if (options.Contains("f"))
            {
               isFriendly = true;
            }
            else if (options.Contains("u"))
            {
               isFriendly = false;
            }

            var pattern = source.Drop(-match.Length);
            Bits32<RegexOptions> regexOptions = RegexOptions.None;
            regexOptions[RegexOptions.IgnoreCase] = ignoreCase;
            regexOptions[RegexOptions.Multiline] = multiline;

            return new Matcher(pattern, regexOptions);
         }
         else
         {
            return new Matcher(source, RegexOptions.None);
         }
      }

      protected static StringHash<string> friendlyPatterns;

      static Matcher()
      {
         friendlyPatterns = new StringHash<string>(true);
         isFriendly = true;
         friendlyStack = new MaybeStack<bool>();
      }

      internal static string getPattern(string source) => friendlyPatterns.Find(source, _ => new Parser().Parse(source), true);

      public static bool IsFriendly
      {
         get => isFriendly;
         set => isFriendly = value;
      }

      public static void PushFriendly(bool newFriendlyValue)
      {
         friendlyStack.Push(isFriendly);
         isFriendly = newFriendlyValue;
      }

      public static void PopFriendly()
      {
         if (friendlyStack.Pop().If(out var newFriendlyValue))
         {
            isFriendly = newFriendlyValue;
         }
      }

      protected string pattern;
      protected RegexOptions options;

      protected Matcher(string pattern, RegexOptions options)
      {
         this.pattern = isFriendly ? getPattern(pattern) : pattern;
         this.options = options;
      }

      public string Pattern => pattern;

      public RegexOptions Options => options;

      public IMatched<Result> Matches(string input)
      {
         try
         {
            var regex = new RRegex(pattern, options);
            var newMatches = regex.Matches(input)
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
            foreach (var name in regex.GetGroupNames())
            {
               if (RRegex.IsMatch(name, @"^[+-]?\d+$"))
               {
                  continue;
               }

               var index = regex.GroupNumberFromName(name);
               indexesToNames.Add(index, name);
               namesToIndexes.Add(name, index);
            }

            if (matches.Length > 0)
            {
               return new Result(matches, indexesToNames, namesToIndexes, slicer, input).Matched();
            }
            else
            {
               return notMatched<Result>();
            }
         }
         catch (Exception exception)
         {
            return failedMatch<Result>(exception);
         }
      }
   }
}