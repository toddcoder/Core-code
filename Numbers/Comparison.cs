using System;
using System.Collections.Generic;
using System.Linq;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Numbers
{
   public class Comparison<T>
      where T : IComparable<T>
   {
      public struct Pair
      {
         T left;
         T right;

         public Pair(T left, T right)
         {
            this.left = left;
            this.right = right;
         }

         public T Left => left;

         public T Right => right;
      }

      public static implicit operator int(Comparison<T> comparison) => comparison.pairs
         .Select(pair => pair.Left.CompareTo(pair.Right))
         .Where(compareTo => compareTo != 0)
         .Select(Math.Sign)
         .FirstOrDefault();

      List<Pair> pairs;
      IMaybe<T> left;

      public Comparison(T left, T right)
      {
         pairs = new List<Pair> { new Pair(left, right) };

         this.left = none<T>();
      }

      public Comparison<T> And(T comparable)
      {
         left = comparable.Some();
         return this;
      }

      public Comparison<T> ComparedTo(T comparable)
      {
         pairs.Add(new Pair(left.Required("Then not called"), comparable));
         left = none<T>();

         return this;
      }
   }
}