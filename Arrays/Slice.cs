using System.Collections;
using System.Collections.Generic;
using Core.Assertions;

namespace Core.Arrays;

public class Slice<T> : IEnumerable<T>
{
   public static implicit operator T[](Slice<T> slice) => slice.Array;

   protected T[] array;
   protected int startIndex;
   protected int stopIndex;
   protected int length;

   public Slice(T[] array, int startIndex)
   {
      this.array = array.Must().Not.BeEmpty().Force();
      this.startIndex = startIndex.Must().BeBetween(0).Until(array.Length).Force();
      stopIndex = this.array.Length - 1;
      length = stopIndex - this.startIndex;
   }

   public Slice<T> To(int index)
   {
      stopIndex = index.Must().BeBetween(startIndex).Until(array.Length).Force();

      length = stopIndex - startIndex;

      return this;
   }

   public Slice<T> For(int count)
   {
      length = count;
      stopIndex = startIndex + length;
      stopIndex.Must().BeLessThan(array.Length).OrThrow();

      return this;
   }

   public T this[int index]
   {
      get
      {
         var offset = index + startIndex;
         offset.Must().BeBetween(0).And(stopIndex).OrThrow();

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