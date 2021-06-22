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
using Core.Matching;
using Core.Monads;
using Core.RegularExpressions;
using static Core.Assertions.AssertionFunctions;

namespace Core.Assertions
{
   public static class AssertionExtensions
   {
      public static ComparableAssertion<int> Must(this int value) => new(value);

      [Obsolete("Use value version")]
      public static ComparableAssertion<int> Must(this Expression<Func<int>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (ComparableAssertion<int>)assertion.Named($"Integer {name}");
      }

      public static ComparableAssertion<int> Must(this (int, string) tuple)
      {
         var (value, name) = tuple;
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

      public static ComparableAssertion<long> Must(this long value) => new(value);

      [Obsolete("Use value version")]
      public static ComparableAssertion<long> Must(this Expression<Func<long>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (ComparableAssertion<long>)assertion.Named($"Long {name}");
      }

      public static ComparableAssertion<long> Must(this (long, string) tuple)
      {
         var (value, name) = tuple;
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

      public static ComparableAssertion<byte> Must(this byte value) => new(value);

      [Obsolete("Use value version")]
      public static ComparableAssertion<byte> Must(this Expression<Func<byte>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (ComparableAssertion<byte>)assertion.Named($"Byte {name}");
      }

      public static ComparableAssertion<byte> Must(this (byte, string) tuple)
      {
         var (value, name) = tuple;
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

      public static FloatAssertion Must(this float value) => new(value);

      [Obsolete("Use value version")]
      public static FloatAssertion Must(this Expression<Func<float>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (FloatAssertion)assertion.Named($"Float {name}");
      }

      public static ComparableAssertion<float> Must(this (float, string) tuple)
      {
         var (value, name) = tuple;
         var assertion = value.Must();

         return (ComparableAssertion<float>)assertion.Named($"Float {name}");
      }

      public static DoubleAssertion Must(this double value) => new(value);

      [Obsolete("Use value version")]
      public static DoubleAssertion Must(this Expression<Func<double>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (DoubleAssertion)assertion.Named($"Double {name}");
      }

      public static ComparableAssertion<double> Must(this (double, string) tuple)
      {
         var (value, name) = tuple;
         var assertion = value.Must();

         return (ComparableAssertion<double>)assertion.Named($"Double {name}");
      }

      public static ComparableAssertion<DateTime> Must(this DateTime value) => new(value);

      [Obsolete("Use value version")]
      public static ComparableAssertion<DateTime> Must(this Expression<Func<DateTime>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (ComparableAssertion<DateTime>)assertion.Named($"Date/time {name}");
      }

      public static ComparableAssertion<DateTime> Must(this (DateTime, string) tuple)
      {
         var (value, name) = tuple;
         var assertion = value.Must();

         return (ComparableAssertion<DateTime>)assertion.Named($"Date/time {name}");
      }

      public static BooleanAssertion Must(this bool value) => new(value);

      [Obsolete("Use value version")]
      public static BooleanAssertion Must(this Expression<Func<bool>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (BooleanAssertion)assertion.Named($"Boolean {name}");
      }

      public static BooleanAssertion Must(this (bool, string) tuple)
      {
         var (value, name) = tuple;
         var assertion = value.Must();

         return (BooleanAssertion)assertion.Named($"Boolean {name}");
      }

      public static StringAssertion Must(this string value) => new(value);

      [Obsolete("Use value version")]
      public static StringAssertion Must(this Expression<Func<string>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (StringAssertion)assertion.Named($"String {name}");
      }

      public static StringAssertion Must(this (string, string) tuple)
      {
         var (value, name) = tuple;
         var assertion = value.Must();

         return (StringAssertion)assertion.Named($"String {name}");
      }

      public static ArrayAssertion<T> Must<T>(this T[] value) => new(value);

      [Obsolete("Use value version")]
      public static ArrayAssertion<T> Must<T>(this Expression<Func<T[]>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (ArrayAssertion<T>)assertion.Named($"Array {name}");
      }

      public static ArrayAssertion<T> Must<T>(this (T[], string) tuple)
      {
         var (value, name) = tuple;
         var assertion = value.Must();

         return (ArrayAssertion<T>)assertion.Named($"Array {name}");
      }

      public static ListAssertion<T> Must<T>(this List<T> value) => new(value);

      [Obsolete("Use value version")]
      public static ListAssertion<T> Must<T>(this Expression<Func<List<T>>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (ListAssertion<T>)assertion.Named($"List {name}");
      }

      public static ListAssertion<T> Must<T>(this (List<T>, string) tuple)
      {
         var (value, name) = tuple;
         var assertion = value.Must();

         return (ListAssertion<T>)assertion.Named($"List {name}");
      }

      public static SetAssertion<T> Must<T>(this Set<T> value) => new(value);

      [Obsolete("Use value version")]
      public static SetAssertion<T> Must<T>(this Expression<Func<Set<T>>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (SetAssertion<T>)assertion.Named($"List {name}");
      }

      public static SetAssertion<T> Must<T>(this (Set<T>, string) tuple)
      {
         var (value, name) = tuple;
         var assertion = value.Must();

         return (SetAssertion<T>)assertion.Named($"Set {name}");
      }

      public static ObjectAssertion Must(this object value)
      {
         if (value is Expression<Func<object>> expression)
         {
            return expression.Must();
         }
         else
         {
            return new ObjectAssertion(value);
         }
      }

      public static ObjectAssertion Must(this Expression<Func<object>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (ObjectAssertion)assertion.Named($"{(value == null ? "Object" : value.GetType().Name)} {name}");
      }

      public static ObjectAssertion Must(this (object, string) tuple)
      {
         var (value, name) = tuple;
         var assertion = value.Must();

         return (ObjectAssertion)assertion.Named($"{(value == null ? "Object" : value.GetType().Name)} {name}");
      }

      public static MaybeAssertion<T> Must<T>(this IMaybe<T> value) => new(value);

      [Obsolete("Use value version")]
      public static MaybeAssertion<T> Must<T>(this Expression<Func<IMaybe<T>>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (MaybeAssertion<T>)assertion.Named($"Optional of {typeof(T).Name} {name}");
      }

      public static MaybeAssertion<T> Must<T>(this (IMaybe<T>, string) tuple)
      {
         var (value, name) = tuple;
         var assertion = value.Must();

         return (MaybeAssertion<T>)assertion.Named($"Optional of {typeof(T).Name} {name}");
      }

      public static ResultAssertion<T> Must<T>(this IResult<T> value) => new(value);

      [Obsolete("Use value version")]
      public static ResultAssertion<T> Must<T>(this Expression<Func<IResult<T>>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (ResultAssertion<T>)assertion.Named($"Result of {typeof(T).Name} {name}");
      }

      public static ResultAssertion<T> Must<T>(this (IResult<T>, string) tuple)
      {
         var (value, name) = tuple;
         var assertion = value.Must();

         return (ResultAssertion<T>)assertion.Named($"Result of {typeof(T).Name} {name}");
      }

      public static MatchedAssertion<T> Must<T>(this IMatched<T> value) => new(value);

      [Obsolete("Use value version")]
      public static MatchedAssertion<T> Must<T>(this Expression<Func<IMatched<T>>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (MatchedAssertion<T>)assertion.Named($"Match of {typeof(T).Name} {name}");
      }

      public static MatchedAssertion<T> Must<T>(this (IMatched<T>, string) tuple)
      {
         var (value, name) = tuple;
         var assertion = value.Must();

         return (MatchedAssertion<T>)assertion.Named($"Match of {typeof(T).Name} {name}");
      }

      public static CompletionAssertion<T> Must<T>(this ICompletion<T> value) => new(value);

      [Obsolete("Use value version")]
      public static CompletionAssertion<T> Must<T>(this Expression<Func<ICompletion<T>>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (CompletionAssertion<T>)assertion.Named($"Async Result of {typeof(T).Name} {name}");
      }

      public static CompletionAssertion<T> Must<T>(this (ICompletion<T>, string) tuple)
      {
         var (value, name) = tuple;
         var assertion = value.Must();

         return (CompletionAssertion<T>)assertion.Named($"Async result of {typeof(T).Name} {name}");
      }

      public static FileNameAssertion Must(this FileName value) => new(value);

      [Obsolete("Use value version")]
      public static FileNameAssertion Must(this Expression<Func<FileName>> expression)
      {
         var (_, value) = resolve(expression);
         var assertion = value.Must();

         return (FileNameAssertion)assertion.Named($"File {value}");
      }

      public static FileNameAssertion Must(this (FileName, string) tuple)
      {
         var (value, _) = tuple;
         var assertion = value.Must();

         return (FileNameAssertion)assertion.Named($"File {value}");
      }

      public static FolderNameAssertion Must(this FolderName value) => new(value);

      [Obsolete("Use value version")]
      public static FolderNameAssertion Must(this Expression<Func<FolderName>> expression)
      {
         var (_, value) = resolve(expression);
         var assertion = value.Must();

         return (FolderNameAssertion)assertion.Named($"Folder {value}");
      }

      public static FolderNameAssertion Must(this (FolderName, string) tuple)
      {
         var (value, _) = tuple;
         var assertion = value.Must();

         return (FolderNameAssertion)assertion.Named($"Folder {value}");
      }

      public static DictionaryAssertion<TKey, TValue> Must<TKey, TValue>(this Dictionary<TKey, TValue> value)
      {
         return new(value);
      }

      [Obsolete("Use value version")]
      public static DictionaryAssertion<TKey, TValue> Must<TKey, TValue>(this Expression<Func<Dictionary<TKey, TValue>>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (DictionaryAssertion<TKey, TValue>)assertion.Named($"Dictionary {name}");
      }

      public static DictionaryAssertion<TKey, TValue> Must<TKey, TValue>(this (Dictionary<TKey, TValue>, string) tuple)
      {
         var (value, name) = tuple;
         var assertion = value.Must();

         return (DictionaryAssertion<TKey, TValue>)assertion.Named($"Dictionary {name}");
      }

      public static DictionaryAssertion<TKey, TValue> Must<TKey, TValue>(this Hash<TKey, TValue> value)
      {
         return new(value);
      }

      [Obsolete("Use value version")]
      public static DictionaryAssertion<TKey, TValue> Must<TKey, TValue>(this Expression<Func<Hash<TKey, TValue>>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (DictionaryAssertion<TKey, TValue>)assertion.Named($"Dictionary {name}");
      }

      public static DictionaryAssertion<TKey, TValue> Must<TKey, TValue>(this (Hash<TKey, TValue>, string) tuple)
      {
         var (value, name) = tuple;
         var assertion = value.Must();

         return (DictionaryAssertion<TKey, TValue>)assertion.Named($"Dictionary {name}");
      }

      public static DictionaryAssertion<TKey, TValue> Must<TKey, TValue>(this IHash<TKey, TValue> value)
      {
         Dictionary<TKey, TValue> hash = value.AnyHash().ForceValue();
         return hash.Must();
      }

      [Obsolete("Use value version")]
      public static DictionaryAssertion<TKey, TValue> Must<TKey, TValue>(this Expression<Func<IHash<TKey, TValue>>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (DictionaryAssertion<TKey, TValue>)assertion.Named($"Dictionary {name}");
      }

      public static DictionaryAssertion<TKey, TValue> Must<TKey, TValue>(this (IHash<TKey, TValue>, string) tuple)
      {
         var (value, name) = tuple;
         var assertion = value.Must();

         return (DictionaryAssertion<TKey, TValue>)assertion.Named($"Dictionary {name}");
      }

      public static DictionaryAssertion<TKey, TValue> Must<TKey, TValue>(this AutoHash<TKey, TValue> value)
      {
         Dictionary<TKey, TValue> hash = value.AnyHash().ForceValue();
         return hash.Must();
      }

      [Obsolete("Use value version")]
      public static DictionaryAssertion<TKey, TValue> Must<TKey, TValue>(this Expression<Func<AutoHash<TKey, TValue>>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (DictionaryAssertion<TKey, TValue>)assertion.Named($"Dictionary {name}");
      }

      public static DictionaryAssertion<TKey, TValue> Must<TKey, TValue>(this (AutoHash<TKey, TValue>, string) tuple)
      {
         var (value, name) = tuple;
         var assertion = value.Must();

         return (DictionaryAssertion<TKey, TValue>)assertion.Named($"Dictionary {name}");
      }

      public static DictionaryAssertion<string, TValue> Must<TValue>(this StringHash<TValue> value)
      {
         Dictionary<string, TValue> hash = value.AnyHash().ForceValue();
         return hash.Must();
      }

      [Obsolete("Use value version")]
      public static DictionaryAssertion<string, TValue> Must<TValue>(this Expression<Func<StringHash<TValue>>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (DictionaryAssertion<string, TValue>)assertion.Named($"Dictionary {name}");
      }

      public static DictionaryAssertion<string, TValue> Must<TValue>(this (StringHash<TValue>, string) tuple)
      {
         var (value, name) = tuple;
         var assertion = value.Must();

         return (DictionaryAssertion<string, TValue>)assertion.Named($"Dictionary {name}");
      }

      public static TypeAssertion Must(this Type value) => new(value);

      [Obsolete("Use value version")]
      public static TypeAssertion Must(this Expression<Func<Type>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (TypeAssertion)assertion.Named($"Type {value.Name} {name}");
      }

      public static TypeAssertion Must(this (Type, string) tuple)
      {
         var (value, name) = tuple;
         var assertion = value.Must();

         return (TypeAssertion)assertion.Named($"Type {value.Name} {name}");
      }

      public static MatcherAssertion Must(this Matcher value) => new(value);

      public static PatternAssertion Must(this Pattern value) => new(value);

      public static ResultAssertion Must(this Result value) => new(value);

      [Obsolete("Use value version")]
      public static MatcherAssertion Must(this Expression<Func<Matcher>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (MatcherAssertion)assertion.Named($"Matcher {name}");
      }

      public static MatcherAssertion Must(this (Matcher, string) tuple)
      {
         var (value, name) = tuple;
         var assertion = value.Must();

         return (MatcherAssertion)assertion.Named($"Matcher {name}");
      }

      public static PatternAssertion Must(this (Pattern, string) tuple)
      {
         var (value, name) = tuple;
         var assertion = value.Must();

         return (PatternAssertion)assertion.Named($"Pattern {name}");
      }

      public static ResultAssertion Must(this (Result, string) tuple)
      {
         var (value, name) = tuple;
         var assertion = value.Must();

         return (ResultAssertion)assertion.Named($"Result {name}");
      }

      public static TypedAssertion<T> MustOfType<T>(this T value) => new(value);

      [Obsolete("Use value version")]
      public static TypedAssertion<T> MustOfType<T>(this Expression<Func<T>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (TypedAssertion<T>)assertion.Named($"Object typed {typeof(T).Name} {name}");
      }

      public static TypedAssertion<T> MustOfType<T>(this (T, string) tuple)
      {
         var (value, name) = tuple;
         var assertion = value.Must();

         return (TypedAssertion<T>)assertion.Named($"Object typed {typeof(T).Name} {name}");
      }

      public static EnumAssertion<TEnum> Must<TEnum>(this TEnum value) where TEnum : struct, Enum => new(value);

      [Obsolete("Use value version")]
      public static EnumAssertion<TEnum> Must<TEnum>(this Expression<Func<TEnum>> expression) where TEnum : struct, Enum
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (EnumAssertion<TEnum>)assertion.Named($"{value.GetType().Name} {name}");
      }

      public static EnumAssertion<TEnum> Must<TEnum>(this (TEnum, string) tuple) where TEnum : struct, Enum
      {
         var (value, name) = tuple;
         var assertion = value.Must();

         return (EnumAssertion<TEnum>)assertion.Named($"{value.GetType().Name} {name}");
      }
   }
}