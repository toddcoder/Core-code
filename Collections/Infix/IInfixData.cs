using Core.Monads;

namespace Core.Collections.Infix
{
   public interface IInfixData<TValue, TInfix>
   {
      TValue Value { get; }

      Maybe<TInfix> Infix { get; }

      void Deconstruct(out TValue value, out Maybe<TInfix> _infix);
   }
}