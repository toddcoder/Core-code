using System;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.ObjectGraphs.Configurations.Json
{
   public class ContainerStack
   {
      protected static IContainer[] emptyArray;

      static ContainerStack()
      {
         emptyArray = new IContainer[0];
      }

      protected IContainer[] array;
      protected int size;

      public ContainerStack()
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

      public void Push(IContainer item)
      {
         if (size == array.Length)
         {
            var objArray = new IContainer[array.Length == 0 ? 4 : 2 * array.Length];
            Array.Copy(array, 0, objArray, 0, size);
            array = objArray;
         }

         array[size++] = item;
      }

      public IMaybe<IContainer> Pop() => maybe(size > 0, () =>
      {
         var obj = array[--size];
         array[size] = default;

         return obj;
      });
   }
}