using System;

namespace Core.Collections.Fibonacci;

public class HeapDoubleKey<T> : Heap<T, double> where T : IEquatable<T>
{
   public HeapDoubleKey() : base(double.NegativeInfinity)
   {
   }
}