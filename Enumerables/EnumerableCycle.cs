using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Core.Enumerables;

public class EnumerableCycle<T> : IEnumerable<T>
{
   protected T[] array;
   protected int index;
   protected int length;
   protected int cycle;

   public EnumerableCycle(IEnumerable<T> enumerable)
   {
      array = enumerable.ToArray();

      index = 0;
      length = array.Length;
      cycle = 0;
   }

   public T Next()
   {
      if (index < length)
      {
         var next = array[index];
         index++;
         return next;
      }
      else
      {
         index = 0;
         cycle++;
         return array[index];
      }
   }

   public int Index => index;

   public int Length => length;

   public int Cycle => cycle;

   public IEnumerator<T> GetEnumerator()
   {
      foreach (var item in array)
      {
         yield return item;
      }
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}