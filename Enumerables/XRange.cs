using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Collections;

namespace Core.Enumerables
{
   public abstract class XRange<TSource, TDistance> : IComparer<TSource>, IEnumerable<TSource>
   {
      protected TSource from;
      protected TSource to;
      protected TDistance distance;
      protected bool includeFrom;
      protected bool includeTo;
      protected Lazy<TSource> actualFrom;
      protected Lazy<TSource> actualTo;

      protected XRange(TSource from, TSource to, TDistance distance, bool includeFrom, bool includeTo)
      {
         this.from = from;
         this.to = to;
         this.distance = distance;
         this.includeFrom = includeFrom;
         this.includeTo = includeTo;

         actualFrom = new Lazy<TSource>(() => this.includeFrom ? this.from : nextValue(this.from));
         actualTo = new Lazy<TSource>(() => this.includeTo ? this.to : previousValue(this.to));
      }

      public TSource From => from;

      public TSource To => to;

      public bool IncludeFrom => includeFrom;

      public bool IncludeTo => includeTo;

      public TSource ActualFrom => actualFrom.Value;

      public TSource ActualTo => actualTo.Value;

      public TDistance Distance => distance;

      protected virtual string valueAsString(TSource value) => value.ToString();

      protected virtual string rangeStringFormat() => "{0}-{1}";

      protected string rangeString(TSource from, TSource to)
      {
         return string.Format(rangeStringFormat(), valueAsString(from), valueAsString(to));
      }

      public string RangeString(bool showActualEndpoints = false)
      {
         return showActualEndpoints ? rangeString(actualFrom.Value, actualTo.Value) : rangeString(from, to);
      }

      protected abstract TSource nextValue(TSource currentValue);

      protected abstract TSource previousValue(TSource currentValue);

      public abstract int Compare(TSource x, TSource y);

      bool equal(TSource x, TSource y) => Compare(x, y) == 0;

      bool greater(TSource x, TSource y) => Compare(x, y) > 0;

      bool less(TSource x, TSource y) => Compare(x, y) < 0;

      public bool Includes(TSource value)
      {
         return less(actualFrom.Value, value) || equal(actualFrom.Value, value) &&
            (greater(actualTo.Value, value) || equal(value, actualTo.Value));
      }

      public bool Includes(XRange<TSource, TDistance> range) => Includes(range.actualFrom.Value) && Includes(range.actualTo.Value);

      public bool Overlaps(XRange<TSource, TDistance> range) => Includes(range.actualFrom.Value) || Includes(range.actualTo.Value);

      public IEnumerator<TSource> GetEnumerator()
      {
         var currentValue = actualFrom.Value;
         yield return currentValue;

         while (true)
         {
            currentValue = nextValue(currentValue);
            if (greater(currentValue, actualTo.Value) || equal(currentValue, actualTo.Value))
            {
               break;
            }

            yield return currentValue;
         }

         currentValue = actualTo.Value;
         yield return currentValue;
      }

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public IEnumerable<TSource> Overlapping(IEnumerable<XRange<TSource, TDistance>> sourceRanges)
      {
         return this.Where(i => sourceRanges.Count(t =>
            (t.less(t.actualFrom.Value, i) || t.equal(t.actualFrom.Value, i)) &&
            (t.greater(t.actualTo.Value, i) || t.equal(t.actualTo.Value, i))) > 1);
      }

      public IEnumerable<TSource> Missing(IEnumerable<XRange<TSource, TDistance>> sourceRanges)
      {
         return this.Where(i => sourceRanges.All(t => t.greater(t.actualFrom.Value, i) || t.less(t.actualTo.Value, i)));
      }

      public IEnumerable<TSource> Unknown(IEnumerable<XRange<TSource, TDistance>> sourceRanges)
      {
         var set = new Set<TSource>();

         foreach (var sourceRange in sourceRanges.OrderBy(x => x.actualFrom.Value))
         {
            foreach (var item in sourceRange)
            {
               if (!Includes(item))
               {
                  if (!set.Contains(item))
                  {
                     set.Add(item);
                     yield return item;
                  }
               }
            }
         }
      }

      protected IEnumerable<IEnumerable<TSource>> toContiguousSequences(IEnumerable<TSource> sequence, XRange<TSource, TDistance> range)
      {
         sequence = sequence.OrderBy(x => x);
         var enumerator = sequence.GetEnumerator();
         if (!enumerator.MoveNext())
         {
            throw new InvalidOperationException("Sequence is empty");
         }

         var currentList = new List<TSource> { enumerator.Current };
         while (enumerator.MoveNext())
         {
            var current = enumerator.Current;
            var textNextValue = range.nextValue(currentList.Last());
            if (current.Equals(textNextValue))
            {
               currentList.Add(current);
            }
            else
            {
               yield return currentList;

               currentList = new List<TSource> { current };
            }
         }

         yield return currentList;
      }

      public IEnumerable<IEnumerable<TSource>> ToContiguousSequences(IEnumerable<TSource> sequence)
      {
         return toContiguousSequences(sequence, this);
      }

      public IEnumerable<string> ToRangeString(IEnumerable<TSource> source) => toContiguousSequences(source, this).Select(seq =>
      {
         var seqArray = seq.ToArray();
         return rangeString(seqArray.First(), seqArray.Last());
      });
   }
}