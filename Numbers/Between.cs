using System;

namespace Core.Numbers;

public class Between<T> where T : IComparable<T>
{
   protected T number;
   protected T minimum;

   public Between(T number, T minimum)
   {
      this.number = number;
      this.minimum = minimum;
   }

   public bool And(T maximum) => number.CompareTo(minimum) >= 0 && number.CompareTo(maximum) <= 0;

   public bool Until(T maximum) => number.CompareTo(minimum) >= 0 && number.CompareTo(maximum) < 0;
}