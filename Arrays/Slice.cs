using System.Collections;
using System.Collections.Generic;
using Core.Assertions;

namespace Core.Arrays
{
   public class Slice<T> : IEnumerable<T>
   {
      public static implicit operator T[](Slice<T> slice) => slice.Array;

      T[] array;
      int startIndex;
      int stopIndex;
      int length;

      public Slice(T[] array, int startIndex)
      {
         this.array = array.MustAs(nameof(array)).Not.BeEmpty().Ensure();
         this.startIndex = startIndex.MustAs(nameof(startIndex)).BeBetween(0).Until(array.Length).Ensure();
         stopIndex = this.array.Length - 1;
         length = stopIndex - this.startIndex;
      }

      public Slice<T> To(int index)
      {
         stopIndex = index.MustAs(nameof(index)).BeBetween(startIndex).Until(array.Length).Ensure();

         length = stopIndex - startIndex;

         return this;
      }

      public Slice<T> For(int count)
      {
         (startIndex + count).MustAs("startIndex + count").BeLessThan(array.Length).Assert();

         length = count;
         stopIndex = startIndex + length;

         return this;
      }

      public T this[int index]
      {
         get
         {
            var offset = index + startIndex;

            offset.MustAs(nameof(offset)).BeBetween(0).And(stopIndex).Assert();

            return array[offset];
         }
      }

      public T[] SourceArray => array;

      public T[] Array => array.RangeOf(startIndex, stopIndex);

      public int StartIndex => startIndex;

      public int StopIndex => stopIndex;

      public int Length => length;

      public IEnumerator<T> GetEnumerator()
      {
         for (var i = startIndex; i <= stopIndex; i++)
         {
            yield return array[i];
         }
      }

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
   }
}