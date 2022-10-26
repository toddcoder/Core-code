using System.Collections.Generic;

namespace Core.Collections;

public class Ring<T>
{
   protected List<T> list;
   protected int index;

   public Ring()
   {
      list = new List<T>();
      index = 0;
   }

   public Ring(IEnumerable<T> enumerable) : this()
   {
      list.AddRange(enumerable);
   }

   public Ring(params T[] args) : this()
   {
      list.AddRange(args);
   }

   public void Add(T item) => list.Add(item);

   public void AddRange(IEnumerable<T> collection) => list.AddRange(collection);

   public int Count => list.Count;

   public T Next()
   {
      var result = list[index];
      if (++index >= list.Count)
      {
         index = 0;
      }

      return result;
   }
}