using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Arrays;
using Core.Collections;
using Core.Exceptions;
using Core.Monads;
using Core.Numbers;
using Core.RegularExpressions.Parsers;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Core.RegularExpressions.RegexExtensions;
using RMatch = System.Text.RegularExpressions.Match;
using RGroup = System.Text.RegularExpressions.Group;

namespace Core.RegularExpressions
{
   public class Matcher : IEnumerable<Matcher.Match>
   {
      public class Group
      {
         public static Group Empty => new Group { Index = -1, Length = 0, Text = "", Which = -1 };

         public int Index { get; set; }

         public int Length { get; set; }

         public string Text { get; set; }

         public int Which { get; set; }

         internal string GetSlice(Slicer slicer) => slicer[Index, Length];

         internal void SetSlice(Slicer slicer, string text) => slicer[Index, Length] = text;

         public void Deconstruct(out string text, out int index, out int length)
         {
            text = Text;
            index = Index;
            length = Length;
         }

         public override string ToString() => Text;
      }

      public class Match : Group, IEnumerable<Group>
      {
         public Group[] Groups { get; set; }

         public int MatchCount(int groupIndex) => Groups[groupIndex].Length;

         public string FirstGroup => Groups.Of(1, Empty).Text;

         public string SecondGroup => Groups.Of(2, Empty).Text;

         public string ThirdGroup => Groups.Of(3, Empty).Text;

         public string FourthGroup => Groups.Of(4, Empty).Text;

         public string FifthGroup => Groups.Of(5, Empty).Text;

         public (string firstGroup, string secondGroup) Groups2() => (FirstGroup, SecondGroup);

         public (string firstGroup, string secondGroup, string thirdGroup) Groups3() => (FirstGroup, SecondGroup, ThirdGroup);

         public IEnumerator<Group> GetEnumerator()
         {
            foreach (var group in Groups.Skip(1))
            {
               yield return group;
            }
         }

         IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
      }

      protected static Hash<string, string> friendlyPatterns;
      protected static Hash<string, string> variables;

      static Matcher()
      {
         friendlyPatterns = new Hash<string, string>();
         variables = new Hash<string, string>();
      }

      internal static string getPattern(string source) => friendlyPatterns.Find(source, pattern => new Parser().Parse(source), true);

      public static void SetVariable(string variableName, string pattern) => variables[variableName] = pattern;

      public static IMaybe<string> GetVariable(string variableName) => variables.Map(variableName);

      protected bool friendly;
      protected Match[] matches;
      protected Hash<int, string> indexesToNames;
      protected Hash<string, int> namesToIndexes;
      protected Slicer slicer;

      public Matcher(bool friendly = true)
      {
         this.friendly = friendly;

         matches = new Match[0];
         indexesToNames = new Hash<int, string>();
         namesToIndexes = new Hash<string, int>();
         slicer = new Slicer("");
      }

      public string Pattern { get; set; }

      public int Index => matches.Length == 0 ? -1 : matches[0].Index;

      public IMaybe<int> IndexOf(int matchIndex) => getMatchMaybe(matchIndex).Map(m => m.Index);

      public IMaybe<int> IndexOf(int matchIndex, int groupIndex)
      {
         return getGroupMaybe(getMatchMaybe(matchIndex), groupIndex).Map(g => g.Index);
      }

      public int Length => matches.Length == 0 ? 0 : matches[0].Length;

      public IMaybe<int> LengthOf(int matchIndex) => getMatchMaybe(matchIndex).Map(m => m.Length);

      public IMaybe<int> LengthOf(int matchIndex, int groupIndex)
      {
         return getGroupMaybe(getMatchMaybe(matchIndex), groupIndex).Map(g => g.Length);
      }

      public virtual bool IsMatch(string input, string pattern, RegexOptions options)
      {
         if (friendly)
         {
            pattern = getPattern(pattern);
         }

         Pattern = pattern;
         var regex = new System.Text.RegularExpressions.Regex(pattern, options);
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
         matches = newMatches.ToArray();
         slicer = new Slicer(input);
         indexesToNames.Clear();
         namesToIndexes.Clear();
         foreach (var name in regex.GetGroupNames())
         {
            if (System.Text.RegularExpressions.Regex.IsMatch(name, @"^[+-]?\d+$"))
            {
               continue;
            }

            var index = regex.GroupNumberFromName(name);
            indexesToNames.Add(index, name);
            namesToIndexes.Add(name, index);
         }

         return matches.Length > 0;
      }

      public IMatched<Match[]> MatchAll(string input, string pattern, RegexOptions options)
      {
         try
         {
            return IsMatch(input, pattern, options) ? matches.Matched() : notMatched<Match[]>();
         }
         catch (Exception exception)
         {
            return failedMatch<Match[]>(exception);
         }
      }

      public IMatched<Match[]> MatchAll(string input, string pattern, bool ignoreCase = false, bool multiline = false)
      {
         return MatchAll(input, pattern, GetOptions(ignoreCase, multiline));
      }

      public IMaybe<Match[]> MatchMaybe(string input, string pattern, RegexOptions options)
      {
         return IsMatch(input, pattern, options) ? matches.Some() : none<Match[]>();
      }

      public IMaybe<Match[]> MatchMaybe(string input, string pattern, bool ignoreCase = false, bool multiline = false)
      {
         return MatchMaybe(input, pattern, GetOptions(ignoreCase, multiline));
      }

      public IMaybe<Match> MatchOneMaybe(string input, string pattern, RegexOptions options)
      {
         return IsMatch(input, pattern, options) ? matches[0].Some() : none<Match>();
      }

      public IMaybe<Match> MatchOneMaybe(string input, string pattern, bool ignoreCase = false, bool multiline = false)
      {
         return MatchOneMaybe(input, pattern, GetOptions(ignoreCase, multiline));
      }

      public IMatched<Match> MatchOne(string input, string pattern, RegexOptions options)
      {
         try
         {
            return IsMatch(input, pattern, options) ? matches[0].Matched() : notMatched<Match>();
         }
         catch (Exception exception)
         {
            return failedMatch<Match>(exception);
         }
      }

      public IMatched<Match> MatchOne(string input, string pattern, bool ignoreCase = false, bool multiline = false)
      {
         return MatchOne(input, pattern, GetOptions(ignoreCase, multiline));
      }

      public string FirstMatch
      {
         get => this[0];
         set => this[0] = value;
      }

      public string SecondMatch
      {
         get => this[1];
         set => this[1] = value;
      }

      public string ThirdMatch
      {
         get => this[2];
         set => this[2] = value;
      }

      public string FourthMatch
      {
         get => this[3];
         set => this[3] = value;
      }

      public string FifthMatch
      {
         get => this[4];
         set => this[4] = value;
      }

      public string SixthMatch
      {
         get => this[5];
         set => this[5] = value;
      }

      public string SeventhMatch
      {
         get => this[6];
         set => this[6] = value;
      }

      public string EighthMatch
      {
         get => this[7];
         set => this[7] = value;
      }

      public string NinthMatch
      {
         get => this[8];
         set => this[8] = value;
      }

      public string TenthMatch
      {
         get => this[9];
         set => this[9] = value;
      }

      public string FirstGroup
      {
         get => this[0, 1];
         set => this[0, 1] = value;
      }

      public string SecondGroup
      {
         get => this[0, 2];
         set => this[0, 2] = value;
      }

      public string ThirdGroup
      {
         get => this[0, 3];
         set => this[0, 3] = value;
      }

      public string FourthGroup
      {
         get => this[0, 4];
         set => this[0, 4] = value;
      }

      public string FifthGroup
      {
         get => this[0, 5];
         set => this[0, 5] = value;
      }

      public string SixthGroup
      {
         get => this[0, 6];
         set => this[0, 6] = value;
      }

      public string SeventhGroup
      {
         get => this[0, 7];
         set => this[0, 7] = value;
      }

      public string EighthGroup
      {
         get => this[0, 8];
         set => this[0, 8] = value;
      }

      public string NinthGroup
      {
         get => this[0, 9];
         set => this[0, 9] = value;
      }

      public string TenthGroup
      {
         get => this[0, 10];
         set => this[0, 10] = value;
      }

      public virtual bool IsMatch(string input, string pattern, bool ignoreCase = false, bool multiline = false)
      {
         return IsMatch(input, pattern, GetOptions(ignoreCase, multiline));
      }

      public virtual void Evaluate(string input, string pattern, RegexOptions options) => IsMatch(input, pattern, options);

      public virtual void Evaluate(string input, string pattern, bool ignoreCase = false, bool multiline = false)
      {
         IsMatch(input, pattern, ignoreCase, multiline);
      }

      public int MatchCount => matches.Length;

      public string this[int matchIndex, int groupIndex]
      {
         get => getGroup(getMatch(matchIndex), groupIndex).GetSlice(slicer);
         set => getGroup(getMatch(matchIndex), groupIndex).SetSlice(slicer, value);
      }

      public string this[int matchIndex, string groupName]
      {
         get
         {
            var index = IndexFromName(groupName);
            return this[matchIndex, index.Required($"Group name {groupName} doesn't exist")];
         }
         set
         {
            var index = IndexFromName(groupName);
            this[matchIndex, index.Required($"Group name {groupName} doesn't exist")] = value;
         }
      }

      public IMaybe<string> Maybe(int matchIndex, int groupIndex)
      {
         var value = this[matchIndex, groupIndex];
         return maybe(value.IsNotEmpty(), () => value);
      }

      public string this[int matchIndex]
      {
         get => getMatch(matchIndex).GetSlice(slicer);
         set => getMatch(matchIndex).SetSlice(slicer, value);
      }

      public IMaybe<string> Maybe(int matchIndex)
      {
         var value = this[matchIndex];
         return maybe(value.IsNotEmpty(), () => value);
      }

      public string[] Matches => matches.Select(m => m.GetSlice(slicer)).ToArray();

      public string[] Groups(int matchIndex) => getMatch(matchIndex).Groups.Select(g => g.GetSlice(slicer)).ToArray();

      public string All
      {
         set
         {
            for (var i = 0; i < matches.Length; i++)
            {
               this[i] = value;
            }
         }
      }

      public Match GetMatch(int matchIndex) => getMatch(matchIndex);

      public Group GetGroup(int matchIndex, int groupIndex) => getGroup(getMatch(matchIndex), groupIndex);

      static Group getGroup(Match match, int groupIndex)
      {
         if (groupIndex.Between(0).Until(match.Groups.Length))
         {
            return match.Groups[groupIndex];
         }
         else
         {
            throw $"groupIndex must be >= 0 and < {match.Groups.Length}; found value {groupIndex}".Throws();
         }
      }

      static IMaybe<Group> getGroupMaybe(IMaybe<Match> match, int index)
      {
         return match.Map(m => maybe(index.Between(0).Until(m.Groups.Length), () => m.Groups[index]));
      }

      Match getMatch(int matchIndex)
      {
         if (matchIndex.Between(0).Until(matches.Length))
         {
            return matches[matchIndex];
         }
         else
         {
            throw $"matchIndex must be >= 0 and < {matches.Length}; found value {matchIndex}".Throws();
         }
      }

      IMaybe<Match> getMatchMaybe(int index) => maybe(index.Between(0).Until(matches.Length), () => matches[index]);

      public int GroupCount(int index) => getMatch(index).Groups.Length;

      public IMaybe<string> NameFromIndex(int index) => indexesToNames.Map(index);

      public IMaybe<int> IndexFromName(string name) => namesToIndexes.Map(name);

      public IEnumerator<Match> GetEnumerator()
      {
         foreach (var match in matches)
         {
            yield return match;
         }
      }

      public override string ToString() => slicer.ToString();

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public void Deconstruct(out string group1, out string group2)
      {
         group1 = this[0, 1];
         group2 = this[0, 2];
      }

      public void Deconstruct(out string group1, out string group2, out string group3)
      {
         group1 = this[0, 1];
         group2 = this[0, 2];
         group3 = this[0, 3];
      }

      public void Deconstruct(out string group1, out string group2, out string group3, out string group4)
      {
         group1 = this[0, 1];
         group2 = this[0, 2];
         group3 = this[0, 3];
         group4 = this[0, 4];
      }

      public void Deconstruct(out string group1, out string group2, out string group3, out string group4, out string group5)
      {
         group1 = this[0, 1];
         group2 = this[0, 2];
         group3 = this[0, 3];
         group4 = this[0, 4];
         group5 = this[0, 5];
      }

      public void Deconstruct(out string group1, out string group2, out string group3, out string group4, out string group5,
         out string group6)
      {
         group1 = this[0, 1];
         group2 = this[0, 2];
         group3 = this[0, 3];
         group4 = this[0, 4];
         group5 = this[0, 5];
         group6 = this[0, 6];
      }

      public void Deconstruct(out string group1, out string group2, out string group3, out string group4, out string group5,
         out string group6, out string group7)
      {
         group1 = this[0, 1];
         group2 = this[0, 2];
         group3 = this[0, 3];
         group4 = this[0, 4];
         group5 = this[0, 5];
         group6 = this[0, 6];
         group7 = this[0, 7];
      }

      public void Deconstruct(out string group1, out string group2, out string group3, out string group4, out string group5,
         out string group6, out string group7, out string group8)
      {
         group1 = this[0, 1];
         group2 = this[0, 2];
         group3 = this[0, 3];
         group4 = this[0, 4];
         group5 = this[0, 5];
         group6 = this[0, 6];
         group7 = this[0, 7];
         group8 = this[0, 8];
      }

      public void Deconstruct(out string group1, out string group2, out string group3, out string group4, out string group5,
         out string group6, out string group7, out string group8, out string group9)
      {
         group1 = this[0, 1];
         group2 = this[0, 2];
         group3 = this[0, 3];
         group4 = this[0, 4];
         group5 = this[0, 5];
         group6 = this[0, 6];
         group7 = this[0, 7];
         group8 = this[0, 8];
         group9 = this[0, 9];
      }

      public void Deconstruct(out string group1, out string group2, out string group3, out string group4, out string group5,
         out string group6, out string group7, out string group8, out string group9, out string group10)
      {
         group1 = this[0, 1];
         group2 = this[0, 2];
         group3 = this[0, 3];
         group4 = this[0, 4];
         group5 = this[0, 5];
         group6 = this[0, 6];
         group7 = this[0, 7];
         group8 = this[0, 8];
         group9 = this[0, 9];
         group10 = this[0, 10];
      }

      public Hash<string, string> ToHash()
      {
         var hash = new Hash<string, string>();
         for (var i = 0; i < GroupCount(0); i++)
         {
            if (indexesToNames.If(i, out var name))
            {
               hash[name] = this[0, 1];
            }
         }

         return hash;
      }

      public MatcherTrying TryTo => new MatcherTrying(this);

      public IEnumerable<Match> Matched(string input, string pattern, bool ignoreCase = false, bool multiline = false)
      {
         if (IsMatch(input, pattern, ignoreCase, multiline))
         {
            foreach (var match in this)
            {
               yield return match;
            }
         }
      }

      public IEnumerable<Match> Matched(string input, string pattern, RegexOptions options)
      {
         if (IsMatch(input, pattern, options))
         {
            foreach (var match in this)
            {
               yield return match;
            }
         }
      }
   }
}