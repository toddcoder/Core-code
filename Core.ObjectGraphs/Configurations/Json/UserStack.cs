using System;
using System.Collections.Generic;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.ObjectGraphs.Configurations.Json
{
   public class UserStack<T>
   {
      const int DEFAULT_CAPACITY = 4;

      static T[] emptyArray;

      static UserStack()
      {
         emptyArray = new T[0];
      }

      T[] array;
      int size;

      public UserStack()
      {
         array = emptyArray;
         size = 0;
      }

      public int Count => size;

      public void Clear()
      {
         Array.Clear(array, 0, size);
         size = 0;
      }

      public bool Contains(T item)
      {
         var innerSize = size;
         var equalityComparer = EqualityComparer<T>.Default;
         while (innerSize-- > 0)
         {
            if (item == null)
            {
               if (array[size] == null)
               {
                  return true;
               }
            }
            else if (array[size] != null && equalityComparer.Equals(array[size], item))
            {
               return true;
            }
         }

         return false;
      }

      public void Push(T item)
      {
         if (size == array.Length)
         {
            var objArray = new T[array.Length == 0 ? 4 : 2 * array.Length];
            Array.Copy(array, 0, objArray, 0, size);
            array = objArray;
         }

         array[size++] = item;
      }

      public IMaybe<T> Pop() => maybe(size > 0, () =>
      {
         var obj = array[--size];
         array[size] = default;

         return obj;
      });
   }
}