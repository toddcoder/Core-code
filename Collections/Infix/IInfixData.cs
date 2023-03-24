using Core.Monads;

namespace Core.Collections.Infix;

public interface IInfixData<TValue, TInfix>
{
   TValue Value { get; }

   Optional<TInfix> Infix { get; }

   void Deconstruct(out TValue value, out Optional<TInfix> _infix);
}