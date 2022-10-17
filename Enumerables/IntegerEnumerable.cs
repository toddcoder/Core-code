using System;
using System.Collections;
using System.Collections.Generic;

namespace Core.Enumerables;

public class IntegerEnumerable : IEnumerable<int>
{
   protected int start;
   protected int stop;
   protected int increment;
   protected bool inclusive;
   protected Func<int, bool> stillEnumerating;
   protected Func<int, int> incrementing;
   protected bool goingUp;

   public IntegerEnumerable(int size)
   {
      start = 0;

      stop = size;
      increment = 1;
      inclusive = false;
      goingUp = true;
      stillEnumerating = i => i < stop;
      incrementing = i => i + increment;
   }

   protected IntegerEnumerable(int start, int stop, int increment, bool inclusive)
   {
      this.start = start;
      this.stop = stop;
      this.increment = Math.Abs(increment);
      this.inclusive = inclusive;

      goingUp = start <= stop;

      if (goingUp)
      {
         if (inclusive)
         {
            stillEnumerating = i => i <= stop;
         }
         else
         {
            stillEnumerating = i => i < stop;
         }

         incrementing = i => i + this.increment;
      }
      else
      {
         if (inclusive)
         {
            stillEnumerating = i => i >= stop;
         }
         else
         {
            stillEnumerating = i => i > stop;
         }

         incrementing = i => i - this.increment;
      }
   }

   public int Start => start;

   public int Stop => stop;

   public int Increment => increment;

   public bool Inclusive => inclusive;

   public IEnumerator<int> GetEnumerator()
   {
      var index = start;

      while (stillEnumerating(index))
      {
         yield return index;

         index = incrementing(index);
      }
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public IntegerEnumerable From(int newStart) => new(newStart, stop, increment, true);

   public IntegerEnumerable By(int newIncrement) => new(start, stop, newIncrement, inclusive);

   public IntegerEnumerable Inclusively() => new(start, stop, increment, true);

   public IntegerEnumerable Exclusively() => new(start, stop, increment, false);
}