using Core.Monads;

namespace Core.Collections.Infix
{
   public interface IInfixData<TValue, TInfix>
   {
      TValue Value { get; }

      IMaybe<TInfix> Infix { get; }

      void Deconstruct(out TValue value, out IMaybe<TInfix> infix);
   }
}