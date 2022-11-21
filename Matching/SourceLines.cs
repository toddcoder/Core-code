using System;
using System.Collections.Generic;
using Core.Assertions;
using Core.DataStructures;
using Core.Monads;
using Core.Strings;
using static Core.Monads.Lazy.LazyRepeatingMonads;
using static Core.Monads.MonadFunctions;

namespace Core.Matching;

public class SourceLines
{
   public const string REGEX_NEXT_LINE = "^ /(.*?) (/r /n | /r | /n); fm";

   protected string source;
   protected int index;
   protected int length;
   protected Maybe<int> _peekLength;
   protected MaybeStack<int> bookmarks;

   public SourceLines(string source)
   {
      source.Must().Not.BeNullOrEmpty().OrThrow();
      this.source = source;

      index = 0;
      length = this.source.Length;
      _peekLength = nil;
      bookmarks = new MaybeStack<int>();
   }

   public string Current => source.Drop(index);

   public bool More => index < length;

   public int Index => index;

   public int Length => length;

   public void Advance(int count)
   {
      _peekLength = nil;

      count.Must().Not.BeLessThan(0).OrThrow();
      index += count;
   }

   public void AdvanceToLastPeek()
   {
      if (_peekLength)
      {
         index += _peekLength;
         _peekLength = nil;
      }
   }

   public void Bookmark() => bookmarks.Push(index);

   public void GoToBookmark()
   {
      var _index = bookmarks.Pop();
      if (_index)
      {
         index = _index;
      }
   }

   public IEnumerable<string> Lines()
   {
      while (More)
      {
         var current = Current;
         var _result = current.Matches(REGEX_NEXT_LINE);
         if (_result)
         {
            var result = ~_result;
            Advance(result.Length);
            yield return result.FirstGroup;
         }
         else
         {
            Advance(current.Length);
            yield return current;
         }
      }
   }

   public IEnumerable<string> While(Func<string, bool> predicate)
   {
      while (More)
      {
         var current = Current;
         var _result = current.Matches(REGEX_NEXT_LINE);
         if (_result)
         {
            var result = ~_result;
            var line = result.FirstGroup;
            if (predicate(line))
            {
               Advance(result.Length);
               yield return line;
            }
            else
            {
               break;
            }
         }
         else if (predicate(current))
         {
            Advance(current.Length);
            yield return current;
         }
         else
         {
            break;
         }
      }
   }

   public IEnumerable<string> While(Pattern pattern) => While(s => s.IsMatch(pattern));

   public IEnumerable<string> Until(Func<string, bool> predicate)
   {
      while (More)
      {
         var current = Current;
         var _result = current.Matches(REGEX_NEXT_LINE);
         if (_result)
         {
            var result = ~_result;
            var line = result.FirstGroup;
            if (!predicate(line))
            {
               Advance(result.Length);
               yield return line;
            }
            else
            {
               break;
            }
         }
         else if (!predicate(current))
         {
            Advance(current.Length);
            yield return current;
         }
         else
         {
            break;
         }
      }
   }

   public void Reset()
   {
      index = 0;
      _peekLength = nil;
   }

   public Maybe<string> PeekNextLine()
   {
      _peekLength = nil;

      if (More)
      {
         var current = Current;
         var _line = current.Matches(REGEX_NEXT_LINE).Map(r => r.FirstGroup);
         if (_line)
         {
            _peekLength = (~_line).Length;
            return _line;
         }
         else
         {
            _peekLength = current.Length;
            return current;
         }
      }
      else
      {
         return nil;
      }
   }

   public IEnumerable<(MatchResult result, string line)> WhileMatches(Pattern pattern)
   {
      var _nextLine = lazyRepeating.maybe<MatchResult>();
      var _result = lazyRepeating.maybe<MatchResult>();

      while (More)
      {
         var current = Current;
         if (_nextLine.ValueOf(current.Matches(REGEX_NEXT_LINE)))
         {
            var lineResult = ~_nextLine;
            var line = lineResult.FirstGroup;
            var _line = line.Matches(pattern);
            if (_line)
            {
               Advance(lineResult.Length);
               yield return (_line, line);
            }
            else
            {
               break;
            }
         }
         else if (_result.ValueOf(current.Matches(pattern)))
         {
            Advance(current.Length);
            yield return (_result, current);
         }
         else
         {
            break;
         }
      }
   }
}