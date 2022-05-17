using Core.Assertions;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Matching
{
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
            if (current.IsMatch(pattern) && current.Matches(REGEX_NEXT_LINE).Map(out var result))
            {
               Advance(result.Length);
               return result.FirstGroup;
            }
            else
            {
               return nil;
            }
         }
         else
         {
            return nil;
         }
      }

      public Maybe<string> PeekNextLine(Pattern pattern)
      {
         _peekLength = nil;
         if (More)
         {
            var current = Current;
            if (current.IsMatch(pattern) && current.Matches(REGEX_NEXT_LINE).Map(out var result))
            {
               _peekLength = result.FirstGroup.Length;
               return result.FirstGroup;
            }
            else
            {
               return nil;
            }
         }
         else
         {
            return nil;
         }
      }

      public Maybe<(MatchResult result, string line)> NextLineMatch(Pattern pattern)
      {
         if (More)
         {
            var (line, lineLength) = Current.Matches(REGEX_NEXT_LINE)
               .Map(result => (result.FirstGroup, result.Length))
               .DefaultTo(() => (Current, Current.Length));

            if (line.Matches(pattern).Map(out var lineResult))
            {
               Advance(lineLength);
               return (lineResult, line);
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
               .Map(result => (result.FirstGroup, result.Length))
               .DefaultTo(() => (Current, Current.Length));
            if (line.Matches(pattern).Map(out var lineResult))
            {
               _peekLength = lineLength;
               return (lineResult, line);
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
            if (current.Matches(REGEX_NEXT_LINE).Map(out var result))
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
            string line;
            if (current.Matches(REGEX_NEXT_LINE).Map(out var result))
            {
               line = result.FirstGroup;
            }
            else
            {
               line = current;
            }

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
         while (NextLine().Map(out var line))
         {
            if (line.IsMatch(pattern))
            {
               return line;
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
         if (_peekLength.Map(out var peekLength))
         {
            index += peekLength;
            _peekLength = nil;
         }
      }

      public Matched<Unit> Matched() => More ? unit : nil;
   }
}