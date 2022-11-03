using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Assertions;
using Core.Collections;
using Core.Monads;
using Core.Numbers;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Matching;

public class MatchResult : IEnumerable<Match>
{
   public static MatchResult Empty
   {
      get => new(Array.Empty<Match>(), new Hash<int, string>(), new StringHash<int>(true), new Slicer(string.Empty), string.Empty, "");
   }

   protected Match[] matches;
   protected Hash<int, string> indexesToNames;
   protected StringHash<int> namesToIndexes;
   protected Slicer slicer;
   protected string input;
   protected Pattern pattern;

   internal MatchResult(Match[] matches, Hash<int, string> indexesToNames, StringHash<int> namesToIndexes, Slicer slicer, string input, Pattern pattern)
   {
      this.matches = matches;
      this.indexesToNames = indexesToNames;
      this.namesToIndexes = namesToIndexes;
      this.slicer = slicer;
      this.input = input;
      this.pattern = pattern;

      foreach (var match in this.matches)
      {
         match.MatchResult = this;
      }
   }

   public string Input => input;

   public Pattern Pattern => pattern;

   public Match[] Matches => matches;

   protected Maybe<Match> getMatchMaybe(int index) => maybe(index.Between(0).Until(matches.Length), () => matches[index]);

   protected static Maybe<Group> getGroupMaybe(Maybe<Match> match, int index)
   {
      return match.Map(m => maybe(index.Between(0).Until(m.Groups.Length), () => m.Groups[index]));
   }

   public int Index => matches.Length == 0 ? -1 : matches[0].Index;

   public Maybe<int> IndexOf(int matchIndex) => getMatchMaybe(matchIndex).Map(m => m.Index);

   public Maybe<int> IndexOf(int matchIndex, int groupIndex)
   {
      return getGroupMaybe(getMatchMaybe(matchIndex), groupIndex).Map(g => g.Index);
   }

   public int Length => matches.Length == 0 ? 0 : matches[0].Length;

   public Maybe<int> LengthOf(int matchIndex) => getMatchMaybe(matchIndex).Map(m => m.Length);

   public Maybe<int> LengthOf(int matchIndex, int groupIndex)
   {
      return getGroupMaybe(getMatchMaybe(matchIndex), groupIndex).Map(g => g.Length);
   }

   public Match GetMatch(int matchIndex) => getMatch(matchIndex);

   public Group GetGroup(int matchIndex, int groupIndex) => getGroup(getMatch(matchIndex), groupIndex);

   protected static Group getGroup(Match match, int groupIndex)
   {
      string messageFunc() => $"groupIndex must be >= 0 and < {match.Groups.Length}; found value {groupIndex}";

      groupIndex.Must().BeBetween(0).Until(match.Groups.Length).OrThrow(messageFunc);
      return match.Groups[groupIndex];
   }

   protected Match getMatch(int matchIndex)
   {
      string messageFunc() => $"matchIndex must be >= 0 and < {matches.Length}; found value {matchIndex}";

      matchIndex.Must().BeBetween(0).Until(matches.Length).OrThrow(messageFunc);
      return matches[matchIndex];
   }

   public int GroupCount(int index) => getMatch(index).Groups.Length;

   public Maybe<string> NameFromIndex(int index) => indexesToNames.Map(index);

   public Maybe<int> IndexFromName(string name) => namesToIndexes.Map(name);

   public int MatchCount => matches.Length;

   public string this[int matchIndex, int groupIndex]
   {
      get => getGroupMaybe(getMatch(matchIndex), groupIndex).Map(g => g.GetSlice(slicer)) | "";
      set => getGroup(getMatch(matchIndex), groupIndex).SetSlice(slicer, value);
   }

   public string this[int matchIndex, string groupName]
   {
      get
      {
         var index = IndexFromName(groupName).Required($"Group name {groupName} doesn't exist");
         return this[matchIndex, index];
      }
      set
      {
         var index = IndexFromName(groupName).Required($"Group name {groupName} doesn't exist");
         this[matchIndex, index] = value;
      }
   }

   public Maybe<string> Maybe(int matchIndex, int groupIndex)
   {
      var value = this[matchIndex, groupIndex];
      return maybe(value.IsNotEmpty(), () => value);
   }

   public string this[int matchIndex]
   {
      get => getMatchMaybe(matchIndex).Map(m => m.GetSlice(slicer)) | "";
      set => getMatch(matchIndex).SetSlice(slicer, value);
   }

   public Maybe<string> Maybe(int matchIndex)
   {
      var value = this[matchIndex];
      return maybe(value.IsNotEmpty(), () => value);
   }

   public IEnumerator<Match> GetEnumerator()
   {
      foreach (var match in matches)
      {
         yield return match;
      }
   }

   public override string ToString() => slicer.ToString();

   public string Text => slicer.ToString();

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
         var _name = indexesToNames.Maybe[i];
         if (_name)
         {
            hash[_name] = this[0, 1];
         }
      }

      return hash;
   }

   public StringHash<string> ToStringHash(bool ignoreCase)
   {
      var hash = new StringHash<string>(ignoreCase);
      for (var i = 0; i < GroupCount(0); i++)
      {
         var _name = indexesToNames.Maybe[i];
         if (_name)
         {
            hash[_name] = this[0, 1];
         }
      }

      return hash;
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

   public string ZerothGroup
   {
      get => this[0, 0];
      set => this[0, 0] = value;
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

   public string Unmatched => input.Drop(Index + Length);

   public Responding<MatchResult> MatchedBy(Pattern pattern) => pattern.MatchedBy(Unmatched);

   public string[] Groups(int matchIndex) => getMatch(matchIndex).Groups.Select(g => g.GetSlice(slicer)).ToArray();

   public string EvaluateMatches(Func<Match, int, Maybe<string>> func)
   {
      Slicer evaluated = input;

      var matchIndex = 0;
      foreach (var match in matches)
      {
         var _replacement = func(match, matchIndex++);
         if (_replacement)
         {
            evaluated[match.Index, match.Length] = _replacement;
         }
      }

      return evaluated.ToString();
   }

   public string EvaluateGroups(Func<Group, int, Maybe<string>> func, bool skipGroup0)
   {
      Slicer evaluated = input;

      foreach (var match in matches)
      {
         var groups = skipGroup0 ? match.Groups.Skip(1).ToArray() : match.Groups;
         var groupIndex = skipGroup0 ? 1 : 0;
         foreach (var group in groups)
         {
            var _replacement = func(group, groupIndex++);
            if (_replacement)
            {
               evaluated[group.Index, group.Length] = _replacement;
            }
         }
      }

      return evaluated.ToString();
   }
}