﻿using System;
using System.Collections;
using System.Collections.Generic;
using Core.Assertions;
using Core.Dates.DateIncrements;

namespace Core.Dates
{
   public class DateEnumerator : IEnumerator<DateTime>, IEnumerable<DateTime>
   {
      DateTime begin;
      DateTime end;
      TimeSpan increment;
      DateTime current;

      public DateEnumerator(DateTime begin, DateTime end, TimeSpan increment)
      {
         begin.MustAs(nameof(begin)).BeLessThan(end).Assert();

         this.begin = begin;
         this.end = end;
         this.increment = increment;
         current = this.begin - increment;
      }

      public DateEnumerator(DateTime begin, DateTime end) : this(begin, end, 1.Day()) { }

      public TimeSpan Increment
      {
         get => increment;
         set
         {
            increment = value;
            Reset();
         }
      }

      public DateEnumerator By(TimeSpan newIncrement)
      {
         Increment = newIncrement;
         return this;
      }

      public void Dispose() { }

      public bool MoveNext()
      {
         current += increment;
         return current <= end;
      }

      public void Reset() => current = begin - increment;

      public DateTime Current => current;

      object IEnumerator.Current => Current;

      public IEnumerator<DateTime> GetEnumerator() => this;

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
   }
}