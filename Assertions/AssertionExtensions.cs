using System;
using System.Collections.Generic;
using Core.Assertions.Collections;
using Core.Assertions.Comparables;
using Core.Assertions.Objects;
using Core.Assertions.Strings;

namespace Core.Assertions
{
   public static class AssertionExtensions
   {
      public static ComparableAssertion<int> Must(this int value) => new ComparableAssertion<int>(value);

      public static ComparableAssertion<long> Must(this long value) => new ComparableAssertion<long>(value);

      public static ComparableAssertion<byte> Must(this byte value) => new ComparableAssertion<byte>(value);

      public static FloatAssertion Must(this float value) => new FloatAssertion(value);

      public static DoubleAssertion Must(this double value) => new DoubleAssertion(value);

      public static ComparableAssertion<DateTime> Must(this DateTime value) => new ComparableAssertion<DateTime>(value);

      public static BooleanAssertion Must(this bool value) => new BooleanAssertion(value);

      public static StringAssertion Must(this string value) => new StringAssertion(value);

      public static ArrayAssertion<T> Must<T>(this T[] value) => new ArrayAssertion<T>(value);

      public static ListAssertion<T> Must<T>(this List<T> value) => new ListAssertion<T>(value);

      public static ObjectAssertion Must(this object value) => new ObjectAssertion(value);
   }
}