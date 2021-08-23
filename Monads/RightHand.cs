using System;

namespace Core.Monads
{
   [Obsolete]
   public class RightHand<T>
   {
      public RightHand(T right)
      {
         Right = right;
      }

      public T Right { get; }
   }
}