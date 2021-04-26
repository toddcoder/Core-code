using System;
using System.Collections;
using System.Collections.Generic;
using Core.Assertions;
using Core.Exceptions;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Enumerables
{
   public class Range : IEnumerable<int>
   {
      public class RangeEnumerator : IEnumerator<int>
      {
         protected int start;
         protected int stop;
         protected int increment;
         protected Func<int, int, bool> endingPredicate;
         protected IMaybe<int> _current;

         public RangeEnumerator(int start, int stop, int increment, Func<int, int, bool> endingPredicate)
         {
            this.start = start;
            this.stop = stop;
            this.increment = increment;
            this.endingPredicate = endingPredicate;
            _current = none<int>();
         }

         public void Dispose()
         {
         }

         public bool MoveNext()
         {
            if (_current.If(out var current))
            {
               current += increment;
               _current = current.Some();
               return endingPredicate(current, stop);
            }
            else
            {
               _current = start.Some();
               return endingPredicate(start, stop);
            }
         }

         public void Reset() => _current = none<int>();

         public int Current
         {
            get
            {
               if (_current.If(out var current))
               {
                  return current;
               }
               else
               {
                  throw "MoveNext not called".Throws();
               }
            }
         }

         object IEnumerator.Current => Current;
      }

      protected int start;
      protected IMaybe<int> _stop;
      protected int increment;
      protected IMaybe<Func<int, int, bool>> _endingPredicate;
      protected bool inclusive;

      public Range(int start)
      {
         this.start = start;
         _stop = none<int>();
         increment = 1;
         _endingPredicate = none<Func<int, int, bool>>();
         inclusive = false;
      }

      public void To(int newStop)
      {
         _stop = newStop.Some();
         _endingPredicate = ((Func<int, int, bool>)((x, y) => x <= y)).Some();
         inclusive = true;
      }

      public void Until(int newStop)
      {
         _stop = newStop.Some();
         _endingPredicate = ((Func<int, int, bool>)((x, y) => x < y)).Some();
         inclusive = false;
      }

      public Range By(int newIncrement)
      {
         increment = newIncrement.Must().Not.BeZero().Force();
         if (increment < 0)
         {
            _endingPredicate = inclusive ? ((Func<int, int, bool>)((x, y) => x >= y)).Some() :
               ((Func<int, int, bool>)((x, y) => x > y)).Some();
         }
         else
         {
            _endingPredicate = inclusive ? ((Func<int, int, bool>)((x, y) => x <= y)).Some() :
               ((Func<int, int, bool>)((x, y) => x < y)).Some();
         }

         return this;
      }

      public IEnumerator<int> GetEnumerator()
      {
         var stop = _stop.Must().Force("Stop value hasn't been set");
         var endingPredicate = _endingPredicate.Must().Force("Ending predicate hasn't been set");

         return new RangeEnumerator(start, stop, increment, endingPredicate);
      }

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
   }
}