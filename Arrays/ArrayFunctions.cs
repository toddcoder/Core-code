using static System.Array;

namespace Core.Arrays
{
   public static class ArrayFunctions
   {
      public static T[] array<T>() => new T[0];

      public static T[] array<T>(params T[] elements) => elements;

      public static T[] array<T>(T[] elements, params T[] newElements)
      {
         var newArray = new T[elements.Length + newElements.Length];

         Copy(elements, newArray, elements.Length);
         Copy(newElements, 0, newArray, elements.Length, newElements.Length);

         return newArray;
      }
   }
}