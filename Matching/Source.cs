using Core.Assertions;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Matching
{
   public class Source
   {
      protected const string REGEX_NEXT_LINE = "^ /(.*?) (/r /n | /r | /n); fm";
      protected string source;
      protected int index;
      protected int length;

      public Source(string source)
      {
         source.Must().Not.BeNullOrEmpty().OrThrow();
         this.source = source;

         index = 0;
         length = this.source.Length;
      }

      public string Current => source.Drop(index);

      public Maybe<string> NextLine(Pattern pattern)
      {
         if (More)
         {
            var current = Current;
            if (current.IsMatch(pattern) && current.Matches(REGEX_NEXT_LINE).If(out var result))
            {
               Advance(result.Length);
               return result.FirstGroup.Some();
            }
            else
            {
               return none<string>();
            }
         }
         else
         {
            return none<string>();
         }
      }

      public Maybe<(MatchResult result, string line)> NextLineMatch(Pattern pattern)
      {
         if (More)
         {
            var (line, lineLength) = Current.Matches(REGEX_NEXT_LINE)
               .Map(result => (result.FirstGroup, result.Length))
               .DefaultTo(() => (Current, Current.Length));

            if (line.Matches(pattern).If(out var lineResult))
            {
               Advance(lineLength);
               return (lineResult, line).Some();
            }
         }

         return none<(MatchResult, string)>();
      }

      public Maybe<string> NextLine()
      {
         if (More)
         {
            var current = Current;
            string line;
            if (current.Matches(REGEX_NEXT_LINE).If(out var result))
            {
               line = result.FirstGroup;
               Advance(result.Length);
            }
            else
            {
               line = current;
               Advance(line.Length);
            }

            return line.Some();
         }
         else
         {
            return none<string>();
         }
      }

      public bool More => index < length;

      public int Index => index;

      public int Length => length;

      public void Advance(int amount)
      {
         amount.Must().Not.BeLessThan(0).OrThrow();
         index += amount;
      }

      public Matched<Unit> Matched() => More ? Unit.Matched() : noMatch<Unit>();
   }
}