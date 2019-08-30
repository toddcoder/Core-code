using System;
using System.Collections.Generic;
using Core.Monads;

namespace Core.Assertion
{
   public interface IAssertion<T>
   {
      bool Not { get; set; }

      bool Throws { get; set; }

      IMaybe<Exception> Exception { get; }

      IAssertion<T> IsEqualTo(T expected);

      IAssertion<T> IsCloseTo(double expected);

      IAssertion<T> IsCloseTo(double expected, double tolerance);

      IAssertion<T> Contains(T expected);

      IAssertion<T> Contains(T expected, IEqualityComparer<T> comparer);

      IAssertion<T> Contains(string expected);

      IAssertion<T> Contains(string expected, StringComparison stringComparison);

      IAssertion<T> StartsWith(string expected);

      IAssertion<T> EndsWith(string expected);

      IAssertion<T> Between(T upperLimit);

      IAssertion<T> Until(T upperLimit);

      IAssertion<T> IsAssignableFrom(object source);

      IAssertion<T> IsAssignableFrom(Type type, object source);

      IAssertion<T> IsType(object source);

      IAssertion<T> IsType(Type type, object source);

      IAssertion<T> IsEmpty();

      IAssertion<T> IsNull();

      IAssertion<T> CountIs(int expected);
   }
}