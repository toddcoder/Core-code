using System.Collections;
using System.Collections.Generic;
using Core.Assertions;
using Core.Enumerables;
using static Core.Assertions.AssertionFunctions;

namespace Core.Collections.Infix
{
   public class InfixList<TValue, TInfix> : IEnumerable<IInfixData<TValue, TInfix>>
   {
      protected List<IInfixData<TValue, TInfix>> list;
      protected bool stopped;

      public InfixList()
      {
         list = new List<IInfixData<TValue, TInfix>>();
         stopped = false;
      }

      public void Add(TValue value, TInfix infix)
      {
         assert(() => value).Must().Not.BeNull().OrThrow();
         stopped.Must().Not.BeTrue().OrThrow("Last item has been already added");

         var item = new InfixData<TValue, TInfix>(value, infix);
         list.Add(item);
         stopped = item.Infix.IsNone;
      }

      public void Add(TValue value)
      {
         assert(() => value).Must().Not.BeNull().OrThrow();
         stopped.Must().Not.BeTrue().OrThrow("Last item has been already added");

         list.Add(new InfixData<TValue, TInfix>(value));
         stopped = true;
      }

      public void Add(IInfixData<TValue, TInfix> item)
      {
         assert(() => item).Must().Not.BeNull().OrThrow();
         stopped.Must().Not.BeTrue().OrThrow("Last item has been already added");

         list.Add(item);
         stopped = item.Infix.IsNone;
      }

      public IEnumerator<IInfixData<TValue, TInfix>> GetEnumerator() => list.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public override string ToString() => list.ToString(" ");
   }
}