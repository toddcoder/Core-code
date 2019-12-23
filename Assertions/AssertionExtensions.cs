using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
using Core.Strings;
using static Core.Assertions.AssertionFunctions;

namespace Core.Assertions
{
   public static class AssertionExtensions
   {
      public static ComparableAssertion<int> Must(this int value) => new ComparableAssertion<int>(value);

      public static ComparableAssertion<int> Must(this Expression<Func<int>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (ComparableAssertion<int>)assertion.Named($"Integer {name} -> {value}");
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

      public static ComparableAssertion<long> Must(this Expression<Func<long>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (ComparableAssertion<long>)assertion.Named($"Long {name} -> {value}");
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

      public static ComparableAssertion<byte> Must(this Expression<Func<byte>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (ComparableAssertion<byte>)assertion.Named($"Byte {name} -> {value}");
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

      public static FloatAssertion Must(this Expression<Func<float>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (FloatAssertion)assertion.Named($"Float {name} -> {value}");
      }

      public static DoubleAssertion Must(this double value) => new DoubleAssertion(value);

      public static DoubleAssertion Must(this Expression<Func<double>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (DoubleAssertion)assertion.Named($"Double {name} -> {value}");
      }

      public static ComparableAssertion<DateTime> Must(this DateTime value) => new ComparableAssertion<DateTime>(value);

      public static ComparableAssertion<DateTime> Must(this Expression<Func<DateTime>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (ComparableAssertion<DateTime>)assertion.Named($"Date/time {name} -> {value}");
      }

      public static BooleanAssertion Must(this bool value) => new BooleanAssertion(value);

      public static BooleanAssertion Must(this Expression<Func<bool>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (BooleanAssertion)assertion.Named($"Boolean {name} -> {value}");
      }

      public static StringAssertion Must(this string value) => new StringAssertion(value);

      public static StringAssertion Must(this Expression<Func<string>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (StringAssertion)assertion.Named($"String {name} -> \"{value.Elliptical(80, ' ')}\"");
      }

      public static ArrayAssertion<T> Must<T>(this T[] value) => new ArrayAssertion<T>(value);

      public static ArrayAssertion<T> Must<T>(this Expression<Func<T[]>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (ArrayAssertion<T>)assertion.Named($"Array {name} -> {enumerableImage(value)}");
      }

      public static ListAssertion<T> Must<T>(this List<T> value) => new ListAssertion<T>(value);

      public static ListAssertion<T> Must<T>(this Expression<Func<List<T>>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (ListAssertion<T>)assertion.Named($"List {name} -> {enumerableImage(value)}");
      }

      public static ObjectAssertion Must(this object value) => new ObjectAssertion(value);

      public static ObjectAssertion Must(this Expression<Func<object>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (ObjectAssertion)assertion.Named($"{(value == null ? "Object" : value.GetType().Name)} {name} -> {value}");
      }

      public static MaybeAssertion<T> Must<T>(this IMaybe<T> value) => new MaybeAssertion<T>(value);

      public static MaybeAssertion<T> Must<T>(this Expression<Func<IMaybe<T>>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (MaybeAssertion<T>)assertion.Named($"Optional of {typeof(T).Name} {name} -> {maybeImage(value)}");
      }

      public static ResultAssertion<T> Must<T>(this IResult<T> value) => new ResultAssertion<T>(value);

      public static ResultAssertion<T> Must<T>(this Expression<Func<IResult<T>>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (ResultAssertion<T>)assertion.Named($"Result of {typeof(T).Name} {name} -> {resultImage(value)}");
      }

      public static MatchedAssertion<T> Must<T>(this IMatched<T> value) => new MatchedAssertion<T>(value);

      public static MatchedAssertion<T> Must<T>(this Expression<Func<IMatched<T>>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (MatchedAssertion<T>)assertion.Named($"Match of {typeof(T).Name} {name} -> {matchedImage(value)}");
      }

      public static CompletionAssertion<T> Must<T>(this ICompletion<T> value) => new CompletionAssertion<T>(value);

      public static CompletionAssertion<T> Must<T>(this Expression<Func<ICompletion<T>>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (CompletionAssertion<T>)assertion.Named($"Async Result of {typeof(T).Name} {name} -> {completionImage(value)}");
      }

      public static FileNameAssertion Must(this FileName value) => new FileNameAssertion(value);

      public static FileNameAssertion Must(this Expression<Func<FileName>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (FileNameAssertion)assertion.Named($"File {name} -> {value.TruncateBySubfolder(1)}");
      }

      public static FolderNameAssertion Must(this FolderName value) => new FolderNameAssertion(value);

      public static FolderNameAssertion Must(this Expression<Func<FolderName>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (FolderNameAssertion)assertion.Named($"Folder {name} -> {value}");
      }

      public static DictionaryAssertion<TKey, TValue> Must<TKey, TValue>(this Dictionary<TKey, TValue> value)
      {
         return new DictionaryAssertion<TKey, TValue>(value);
      }

      public static DictionaryAssertion<TKey, TValue> Must<TKey, TValue>(this Expression<Func<Dictionary<TKey, TValue>>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (DictionaryAssertion<TKey, TValue>)assertion.Named($"Dictionary {name} -> {dictionaryImage(value)}");
      }

      public static DictionaryAssertion<TKey, TValue> Must<TKey, TValue>(this Hash<TKey, TValue> value)
      {
         return new DictionaryAssertion<TKey, TValue>(value);
      }

      public static DictionaryAssertion<TKey, TValue> Must<TKey, TValue>(this Expression<Func<Hash<TKey, TValue>>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (DictionaryAssertion<TKey, TValue>)assertion.Named($"Dictionary {name} -> {dictionaryImage(value)}");
      }

      public static DictionaryAssertion<TKey, TValue> Must<TKey, TValue>(this IHash<TKey, TValue> value)
      {
         Dictionary<TKey, TValue> hash = value.AnyHash().ForceValue();
         return hash.Must();
      }

      public static DictionaryAssertion<TKey, TValue> Must<TKey, TValue>(this Expression<Func<IHash<TKey, TValue>>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (DictionaryAssertion<TKey, TValue>)assertion.Named($"Dictionary {name} -> {hashImage(value)}");
      }

      public static TypeAssertion Must(this Type value) => new TypeAssertion(value);

      public static TypeAssertion Must(this Expression<Func<Type>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (TypeAssertion)assertion.Named($"Type {value.Name} {name} -> {value.FullName}");
      }

      public static MatcherAssertion Must(this Matcher value) => new MatcherAssertion(value);

      public static MatcherAssertion Must(this Expression<Func<Matcher>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (MatcherAssertion)assertion.Named($"Matcher {name} -> {value}");
      }

      public static TypedAssertion<T> MustOfType<T>(this T value) => new TypedAssertion<T>(value);

      public static TypedAssertion<T> MustOfType<T>(this Expression<Func<T>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (TypedAssertion<T>)assertion.Named($"Object typed {typeof(T).Name} {name} -> {value}");
      }
   }
}