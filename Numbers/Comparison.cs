using System;
using System.Collections.Generic;
using System.Linq;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Numbers;

public class Comparison<T> where T : IComparable<T>
{
   public readonly struct Pair
   {
      private readonly T left;
      private readonly T right;

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

   protected List<Pair> pairs;
   protected Optional<T> _left;

   public Comparison(T left, T right)
   {
      pairs = new List<Pair> { new(left, right) };

      _left = nil;
   }

   public Comparison<T> And(T comparable)
   {
      _left = comparable;
      return this;
   }

   public Comparison<T> ComparedTo(T comparable)
   {
      pairs.Add(new Pair(_left.Required("Then not called"), comparable));
      _left = nil;

      return this;
   }
}