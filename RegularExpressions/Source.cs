using Core.Assertions;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.RegularExpressions
{
   public class Source
   {
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

      public bool More => index < length;

      public int Index => index;

      public int Length => length;

      public void Advance(int amount)
      {
         amount.Must().Not.BeLessThan(0).OrThrow();
         index += amount;
      }

      public IMatched<Unit> Matched() => More ? Unit.Matched() : notMatched<Unit>();
   }
}