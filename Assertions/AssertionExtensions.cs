﻿using System;
using System.Collections.Generic;
using Core.Assertions.Collections;
using Core.Assertions.Comparables;
using Core.Assertions.Computers;
using Core.Assertions.Monads;
using Core.Assertions.Objects;
using Core.Assertions.Strings;
using Core.Collections;
using Core.Computers;
using Core.Monads;
using Core.RegularExpressions;

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

      public static MaybeAssertion<T> Must<T>(this IMaybe<T> value) => new MaybeAssertion<T>(value);

      public static ResultAssertion<T> Must<T>(this IResult<T> value) => new ResultAssertion<T>(value);

      public static MatchedAssertion<T> Must<T>(this IMatched<T> value) => new MatchedAssertion<T>(value);

      public static CompletionAssertion<T> Must<T>(this ICompletion<T> value) => new CompletionAssertion<T>(value);

      public static FileNameAssertion Must(this FileName value) => new FileNameAssertion(value);

      public static FolderNameAssertion Must(this FolderName value) => new FolderNameAssertion(value);

      public static DictionaryAssertion<TKey, TValue> Must<TKey, TValue>(this Dictionary<TKey, TValue> value) => new DictionaryAssertion<TKey, TValue>(value);

      public static DictionaryAssertion<TKey, TValue> Must<TKey, TValue>(this IHash<TKey, TValue> value)
      {
         Dictionary<TKey, TValue> hash = value.AnyHash().ForceValue();
         return hash.Must();
      }

      public static TypeAssertion Must(this Type value) => new TypeAssertion(value);

      public static MatcherAssertion Must(this Matcher value) => new MatcherAssertion(value);

      public static TypedAssertion<T> MustOfType<T>(this T value) => new TypedAssertion<T>(value);
   }
}