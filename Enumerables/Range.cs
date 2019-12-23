using System;
using System.Collections;
using System.Collections.Generic;
using Core.Assertions;
using Core.Exceptions;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Enumerables
{
   public class Range : IEnumerable<int>
   {
      public class RangeEnumerator : IEnumerator<int>
      {
         int start;
         int stop;
         int increment;
         Func<int, int, bool> endingPredicate;
         IMaybe<int> current;

         public RangeEnumerator(int start, int stop, int increment, Func<int, int, bool> endingPredicate)
         {
            this.start = start;
            this.stop = stop;
            this.increment = increment;
            this.endingPredicate = endingPredicate;
            current = none<int>();
         }

         public void Dispose() { }

         public bool MoveNext()
         {
            if (current.If(out var value))
            {
               value += increment;
               current = value.Some();
               return endingPredicate(value, stop);
            }
            else
            {
               current = start.Some();
               return endingPredicate(start, stop);
            }
         }

         public void Reset() => current = none<int>();

         public int Current
         {
            get
            {
               if (current.If(out var value))
               {
                  return value;
               }
               else
               {
                  throw "MoveNext not called".Throws();
               }
            }
         }

         object IEnumerator.Current => Current;
      }

      int start;
      IMaybe<int> stop;
      int increment;
      IMaybe<Func<int, int, bool>> endingPredicate;
      bool inclusive;

      public Range(int start)
      {
         this.start = start;
         stop = none<int>();
         increment = 1;
         endingPredicate = none<Func<int, int, bool>>();
         inclusive = false;
      }

      public void To(int newStop)
      {
         stop = newStop.Some();
         endingPredicate = ((Func<int, int, bool>)((x, y) => x <= y)).Some();
         inclusive = true;
      }

      public void Until(int newStop)
      {
         stop = newStop.Some();
         endingPredicate = ((Func<int, int, bool>)((x, y) => x < y)).Some();
         inclusive = false;
      }

      public Range By(int newIncrement)
      {
         increment = assert(() => newIncrement).Must().Not.BeZero().Force();
         if (increment < 0)
         {
            endingPredicate = inclusive ? ((Func<int, int, bool>)((x, y) => x >= y)).Some() :
               ((Func<int, int, bool>)((x, y) => x > y)).Some();
         }
         else
         {
            endingPredicate = inclusive ? ((Func<int, int, bool>)((x, y) => x <= y)).Some() :
               ((Func<int, int, bool>)((x, y) => x < y)).Some();
         }

         return this;
      }

      public IEnumerator<int> GetEnumerator()
      {
         var s = assert(() => stop).Must().Force("Stop value hasn't been set");
         var p = assert(() => endingPredicate).Must().Force("Ending predicate hasn't been set");

         return new RangeEnumerator(start, s, increment, p);
      }

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
   }
}