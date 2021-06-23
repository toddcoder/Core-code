using Core.Monads;
using Core.Objects;
using static Core.Monads.MonadFunctions;

namespace Core.Collections.Infix
{
   public class InfixData<TValue, TInfix> : IInfixData<TValue, TInfix>
   {
      public InfixData(TValue value, Maybe<TInfix> infix)
      {
         Value = value;
         Infix = infix;
      }

      public InfixData(TValue value, TInfix infix) : this(value, infix.Some()) { }

      public InfixData(TValue value) : this(value, none<TInfix>()) { }

      [Equatable]
      public TValue Value { get; }

      [Equatable]
      public Maybe<TInfix> Infix { get; }

      public override string ToString() => Value + Infix.Map(i => " " + i).DefaultTo(() => "");

      public void Deconstruct(out TValue value, out Maybe<TInfix> infix)
      {
         value = Value;
         infix = Infix;
      }
   }
}