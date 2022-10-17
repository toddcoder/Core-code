using System;
using System.Collections;
using System.Collections.Generic;
using Core.Assertions;
using Core.Monads;
using static Core.Lambdas.LambdaFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Enumerables;

public class Range : IEnumerable<int>
{
   public class RangeEnumerator : IEnumerator<int>
   {
      protected int start;
      protected int stop;
      protected int increment;
      protected Func<int, int, bool> endingPredicate;
      protected Maybe<int> _current;

      public RangeEnumerator(int start, int stop, int increment, Func<int, int, bool> endingPredicate)
      {
         this.start = start;
         this.stop = stop;
         this.increment = increment;
         this.endingPredicate = endingPredicate;
         _current = nil;
      }

      public void Dispose()
      {
      }

      public bool MoveNext()
      {
         if (_current)
         {
            _current += increment;
            return endingPredicate(_current, stop);
         }
         else
         {
            _current = start;
            return endingPredicate(start, stop);
         }
      }

      public void Reset() => _current = nil;

      public int Current
      {
         get
         {
            if (_current)
            {
               return _current;
            }
            else
            {
               throw fail("MoveNext not called");
            }
         }
      }

      object IEnumerator.Current => Current;
   }

   protected int start;
   protected Maybe<int> _stop;
   protected int increment;
   protected Maybe<Func<int, int, bool>> _endingPredicate;
   protected bool inclusive;

   public Range(int start)
   {
      this.start = start;
      _stop = nil;
      increment = 1;
      _endingPredicate = nil;
      inclusive = false;
   }

   public void To(int newStop)
   {
      _stop = newStop;
      _endingPredicate = func<int, int, bool>((x, y) => x <= y);
      inclusive = true;
   }

   public void Until(int newStop)
   {
      _stop = newStop;
      _endingPredicate = func<int, int, bool>((x, y) => x < y);
      inclusive = false;
   }

   public Range By(int newIncrement)
   {
      increment = newIncrement.Must().Not.BeZero().Force();
      if (increment < 0)
      {
         _endingPredicate = func<int, int, bool>((x, y) => x >= y);
      }
      else
      {
         _endingPredicate = func<int, int, bool>((x, y) => x <= y);
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