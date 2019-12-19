using System;
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

      public static ComparableAssertion<int> MustAs(this int value, string name)
      {
         var assertion = value.Must();
         return (ComparableAssertion<int>)assertion.Named($"Integer {name}");
      }

      public static ComparableAssertion<int> BeEven(this ComparableAssertion<int> assertion)
      {
         return assertion.IfTrue(i => i % 2 == 0, "$name must $not be even");
      }

      public static ComparableAssertion<int> BeOdd(this ComparableAssertion<int> assertion)
      {
         return assertion.IfTrue(i => i % 2 != 0, "$name must $not be odd");
      }

      public static ComparableAssertion<long> Must(this long value) => new ComparableAssertion<long>(value);

      public static ComparableAssertion<long> MustAs(this long value, string name)
      {
         var assertion = value.Must();
         return (ComparableAssertion<long>)assertion.Named($"Long {name}");
      }

      public static ComparableAssertion<long> BeEven(this ComparableAssertion<long> assertion)
      {
         return assertion.IfTrue(i => i % 2 == 0, "$name must $not be even");
      }

      public static ComparableAssertion<long> BeOdd(this ComparableAssertion<long> assertion)
      {
         return assertion.IfTrue(i => i % 2 != 0, "$name must $not be odd");
      }

      public static ComparableAssertion<byte> Must(this byte value) => new ComparableAssertion<byte>(value);

      public static ComparableAssertion<byte> MustAs(this byte value, string name)
      {
         var assertion = value.Must();
         return (ComparableAssertion<byte>)assertion.Named($"Byte {name}");
      }

      public static ComparableAssertion<byte> BeEven(this ComparableAssertion<byte> assertion)
      {
         return assertion.IfTrue(i => i % 2 == 0, "$name must $not be even");
      }

      public static ComparableAssertion<byte> BeOdd(this ComparableAssertion<byte> assertion)
      {
         return assertion.IfTrue(i => i % 2 != 0, "$name must $not be odd");
      }

      public static FloatAssertion Must(this float value) => new FloatAssertion(value);

      public static FloatAssertion MustAs(this float value, string name)
      {
         var assertion = value.Must();
         return (FloatAssertion)assertion.Named($"Float {name}");
      }

      public static DoubleAssertion Must(this double value) => new DoubleAssertion(value);

      public static DoubleAssertion MustAs(this double value, string name)
      {
         var assertion = value.Must();
         return (DoubleAssertion)assertion.Named($"Double {name}");
      }

      public static ComparableAssertion<DateTime> Must(this DateTime value) => new ComparableAssertion<DateTime>(value);

      public static ComparableAssertion<DateTime> MustAs(this DateTime value, string name)
      {
         var assertion = value.Must();
         return (ComparableAssertion<DateTime>)assertion.Named($"Date/time {name}");
      }

      public static BooleanAssertion Must(this bool value) => new BooleanAssertion(value);

      public static BooleanAssertion MustAs(this bool value, string name)
      {
         var assertion = value.Must();
         return (BooleanAssertion)assertion.Named($"Boolean {name}");
      }

      public static StringAssertion Must(this string value) => new StringAssertion(value);

      public static StringAssertion MustAs(this string value, string name)
      {
         var assertion = value.Must();
         return (StringAssertion)assertion.Named($"String {name}");
      }

      public static ArrayAssertion<T> Must<T>(this T[] value) => new ArrayAssertion<T>(value);

      public static ArrayAssertion<T> MustAs<T>(this T[] value, string name)
      {
         var assertion = value.Must();
         return (ArrayAssertion<T>)assertion.Named($"Array {name}");
      }

      public static ListAssertion<T> Must<T>(this List<T> value) => new ListAssertion<T>(value);

      public static ListAssertion<T> MustAs<T>(this List<T> value, string name)
      {
         var assertion = value.Must();
         return (ListAssertion<T>)assertion.Named($"List {name}");
      }

      public static ObjectAssertion Must(this object value) => new ObjectAssertion(value);

      public static ObjectAssertion MustAs(this object value, string name)
      {
         var assertion = value.Must();
         return (ObjectAssertion)assertion.Named($"Object {name}");
      }

      public static MaybeAssertion<T> Must<T>(this IMaybe<T> value) => new MaybeAssertion<T>(value);

      public static MaybeAssertion<T> MustAs<T>(this IMaybe<T> value, string name)
      {
         var assertion = value.Must();
         return (MaybeAssertion<T>)assertion.Named($"Optional {name}");
      }

      public static ResultAssertion<T> Must<T>(this IResult<T> value) => new ResultAssertion<T>(value);

      public static ResultAssertion<T> MustAs<T>(this IResult<T> value, string name)
      {
         var assertion = value.Must();
         return (ResultAssertion<T>)assertion.Named($"Result {name}");
      }

      public static MatchedAssertion<T> Must<T>(this IMatched<T> value) => new MatchedAssertion<T>(value);

      public static MatchedAssertion<T> MustAs<T>(this IMatched<T> value, string name)
      {
         var assertion = value.Must();
         return (MatchedAssertion<T>)assertion.Named($"Match {name}");
      }

      public static CompletionAssertion<T> Must<T>(this ICompletion<T> value) => new CompletionAssertion<T>(value);

      public static CompletionAssertion<T> MustAs<T>(this ICompletion<T> value, string name)
      {
         var assertion = value.Must();
         return (CompletionAssertion<T>)assertion.Named($"Async result {name}");
      }

      public static FileNameAssertion Must(this FileName value) => new FileNameAssertion(value);

      public static FolderNameAssertion Must(this FolderName value) => new FolderNameAssertion(value);

      public static FileNameAssertion MustAs(this FileName value, string name)
      {
         var assertion = value.Must();
         return (FileNameAssertion)assertion.Named($"File {name}");
      }

      public static FolderNameAssertion MustAs(this FolderName value, string name)
      {
         var assertion = value.Must();
         return (FolderNameAssertion)assertion.Named($"Folder {name}");
      }

      public static DictionaryAssertion<TKey, TValue> Must<TKey, TValue>(this Dictionary<TKey, TValue> value)
      {
         return new DictionaryAssertion<TKey, TValue>(value);
      }

      public static DictionaryAssertion<TKey, TValue> MustAs<TKey, TValue>(this Dictionary<TKey, TValue> value, string name)
      {
         var assertion = value.Must();
         return (DictionaryAssertion<TKey,TValue>)assertion.Named($"Dictionary {name}");
      }

      public static DictionaryAssertion<TKey, TValue> Must<TKey, TValue>(this Hash<TKey, TValue> value)
      {
         return new DictionaryAssertion<TKey, TValue>(value);
      }

      public static DictionaryAssertion<TKey, TValue> MustAs<TKey, TValue>(this Hash<TKey, TValue> value, string name)
      {
         var assertion = value.Must();
         return (DictionaryAssertion<TKey, TValue>)assertion.Named($"Dictionary {name}");
      }

      public static DictionaryAssertion<TKey, TValue> Must<TKey, TValue>(this IHash<TKey, TValue> value)
      {
         Dictionary<TKey, TValue> hash = value.AnyHash().ForceValue();
         return hash.Must();
      }

      public static DictionaryAssertion<TKey, TValue> MustAs<TKey, TValue>(this IHash<TKey, TValue> value, string name)
      {
         var assertion = value.Must();
         return (DictionaryAssertion<TKey, TValue>)assertion.Named($"Dictionary {name}");
      }

      public static TypeAssertion Must(this Type value) => new TypeAssertion(value);

      public static TypeAssertion MustAs(this Type value, string name)
      {
         var assertion = value.Must();
         return (TypeAssertion)assertion.Named($"Type {name}");
      }

      public static MatcherAssertion Must(this Matcher value) => new MatcherAssertion(value);

      public static MatcherAssertion MustAs(this Matcher value, string name)
      {
         var assertion = value.Must();
         return (MatcherAssertion)assertion.Named($"Matcher {name}");
      }

      public static TypedAssertion<T> MustOfType<T>(this T value) => new TypedAssertion<T>(value);

      public static TypedAssertion<T> MustOfTypeAs<T>(this T value, string name)
      {
         var assertion = value.Must();
         return (TypedAssertion<T>)assertion.Named($"{typeof(T).Name} {name}");
      }
   }
}