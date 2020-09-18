using System.Collections;
using System.Collections.Generic;
using Core.Assertions;
using Core.Enumerables;
using Core.Monads;
using Core.Objects;
using static Core.Assertions.AssertionFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Collections
{
   public class InfixList<TValue, TInfix> : IEnumerable<InfixList<TValue, TInfix>.InfixData>
   {
      public class InfixData : EquatableBase
      {
         internal InfixData(TValue value, IMaybe<TInfix> infix)
         {
            Value = value;
            Infix = infix;
         }

         internal InfixData(TValue value, TInfix infix) : this(value, infix.Some()) { }

         internal InfixData(TValue value) : this(value, none<TInfix>()) { }

         [Equatable]
         public TValue Value { get; }

         [Equatable]
         public IMaybe<TInfix> Infix { get; }

         public override string ToString() => Value.ToString() + Infix.Map(i => " " + i);
      }

      protected List<InfixData> list;

      public InfixList()
      {
         list = new List<InfixData>();
      }

      public void Add(TValue value, TInfix infix)
      {
         assert(() => value).Must().Not.BeNull().OrThrow();

         list.Add(new InfixData(value, infix));
      }

      public void Add(TValue value)
      {
         assert(() => value).Must().Not.BeNull().OrThrow();

         list.Add(new InfixData(value));
      }

      public IEnumerator<InfixData> GetEnumerator() => list.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public override string ToString() => list.ToString(" ");
   }
}