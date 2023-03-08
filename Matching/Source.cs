using Core.Assertions;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Matching;

public class Source
{
   public const string REGEX_NEXT_LINE = "^ /(.*?) (/r /n | /r | /n); fm";

   protected string source;
   protected int index;
   protected int length;
   protected Maybe<int> _peekLength;

   public Source(string source)
   {
      source.Must().Not.BeNullOrEmpty().OrThrow();
      this.source = source;

      index = 0;
      length = this.source.Length;
      _peekLength = nil;
   }

   public string Current => source.Drop(index);

   public Maybe<string> NextLine(Pattern pattern)
   {
      if (More)
      {
         var current = Current;
         if (current.IsMatch(pattern))
         {
            var _result = current.Matches(REGEX_NEXT_LINE);
            if (_result is (true, var result))
            {
               Advance(result.Length);
               return result.FirstGroup;
            }
         }
      }

      return nil;
   }

   public Maybe<string> PeekNextLine(Pattern pattern)
   {
      _peekLength = nil;
      if (More)
      {
         var current = Current;
         if (current.IsMatch(pattern))
         {
            var _firstGroup = current.Matches(REGEX_NEXT_LINE).Map(r => r.FirstGroup);
            if (_firstGroup is (true, var firstGroup))
            {
               _peekLength = firstGroup.Length;
               return firstGroup;
            }
         }
      }

      return nil;
   }

   public Maybe<(MatchResult result, string line)> NextLineMatch(Pattern pattern)
   {
      if (More)
      {
         var (line, lineLength) = Current.Matches(REGEX_NEXT_LINE)
            .Map(result => (result.FirstGroup, result.Length)) | (() => (Current, Current.Length));

         var _result = line.Matches(pattern);
         if (_result)
         {
            Advance(lineLength);
            return (_result, line);
         }
      }

      return nil;
   }

   public Maybe<(MatchResult result, string line)> PeekNextLineMatch(Pattern pattern)
   {
      _peekLength = nil;
      if (More)
      {
         var (line, lineLength) = Current.Matches(REGEX_NEXT_LINE)
            .Map(result => (result.FirstGroup, result.Length)) | (() => (Current, Current.Length));
         var _result = line.Matches(pattern);
         if (_result)
         {
            _peekLength = lineLength;
            return (_result, line);
         }
      }

      return nil;
   }

   public Maybe<string> NextLine()
   {
      if (More)
      {
         var current = Current;
         string line;
         var _result = current.Matches(REGEX_NEXT_LINE);
         if (_result is (true, var result))
         {
            line = result.FirstGroup;
            Advance(result.Length);
         }
         else
         {
            line = current;
            Advance(line.Length);
         }

         return line;
      }
      else
      {
         return nil;
      }
   }

   public Maybe<string> PeekNextLine()
   {
      _peekLength = nil;
      if (More)
      {
         var current = Current;
         var line = current.Matches(REGEX_NEXT_LINE).Map(r => r.FirstGroup) | current;

         _peekLength = line.Length;
         return line;
      }
      else
      {
         return nil;
      }
   }

   public Maybe<string> GoTo(Pattern pattern)
   {
      while (true)
      {
         var _line = NextLine();
         if (_line is (true, var line))
         {
            if (line.IsMatch(pattern))
            {
               return line;
            }
         }
         else
         {
            break;
         }
      }

      return nil;
   }

   public bool More => index < length;

   public int Index => index;

   public int Length => length;

   public void Advance(int amount)
   {
      _peekLength = nil;

      amount.Must().Not.BeLessThan(0).OrThrow();
      index += amount;
   }

   public void AdvanceLastPeek()
   {
      if (_peekLength)
      {
         index += _peekLength;
         _peekLength = nil;
      }
   }

   public Optional<Unit> Optional => More ? unit : nil;
}