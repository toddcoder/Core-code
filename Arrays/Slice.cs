using System.Collections;
using System.Collections.Generic;
using Core.Numbers;
using static Core.Booleans.Assertions;

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
         Assert(array.IsNotEmpty(), "Array can't be empty");
         Assert(startIndex.Between(0).Until(array.Length), "Index must be >= 0 and < array length");

         this.array = array;
         this.startIndex = startIndex;
         stopIndex = this.array.Length - 1;
         length = stopIndex - this.startIndex;
      }

      public Slice<T> To(int index)
      {
         Assert(index.Between(startIndex).Until(array.Length), "Stop index must be >= startIndex and < array length");

         stopIndex = index;
         length = stopIndex - startIndex;

         return this;
      }

      public Slice<T> For(int count)
      {
         Assert(startIndex + count < array.Length, "Count makes stop index exceed array length");

         length = count;
         stopIndex = startIndex + length;

         return this;
      }

      public T this[int index]
      {
         get
         {
            var offset = index + startIndex;

            Assert(offset.Between(0).And(stopIndex), "Index outside of slice");

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
            yield return array[i];
      }

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
   }
}