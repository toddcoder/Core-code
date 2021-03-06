﻿using System;
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

      public static ComparableAssertion<long> Must(this Expression<Func<long>> expression)
      {
         var (name, value) = resolve(expression);
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

      public static ComparableAssertion<byte> Must(this Expression<Func<byte>> expression)
      {
         var (name, value) = resolve(expression);
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

      public static FloatAssertion Must(this Expression<Func<float>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (FloatAssertion)assertion.Named($"Float {name}");
      }

      public static DoubleAssertion Must(this double value) => new DoubleAssertion(value);

      public static DoubleAssertion Must(this Expression<Func<double>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (DoubleAssertion)assertion.Named($"Double {name}");
      }

      public static ComparableAssertion<DateTime> Must(this DateTime value) => new ComparableAssertion<DateTime>(value);

      public static ComparableAssertion<DateTime> Must(this Expression<Func<DateTime>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (ComparableAssertion<DateTime>)assertion.Named($"Date/time {name}");
      }

      public static BooleanAssertion Must(this bool value) => new BooleanAssertion(value);

      public static BooleanAssertion Must(this Expression<Func<bool>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (BooleanAssertion)assertion.Named($"Boolean {name}");
      }

      public static StringAssertion Must(this string value) => new StringAssertion(value);

      public static StringAssertion Must(this Expression<Func<string>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (StringAssertion)assertion.Named($"String {name}");
      }

      public static ArrayAssertion<T> Must<T>(this T[] value) => new ArrayAssertion<T>(value);

      public static ArrayAssertion<T> Must<T>(this Expression<Func<T[]>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (ArrayAssertion<T>)assertion.Named($"Array {name}");
      }

      public static ListAssertion<T> Must<T>(this List<T> value) => new ListAssertion<T>(value);

      public static ListAssertion<T> Must<T>(this Expression<Func<List<T>>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (ListAssertion<T>)assertion.Named($"List {name}");
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

      public static MaybeAssertion<T> Must<T>(this IMaybe<T> value) => new MaybeAssertion<T>(value);

      public static MaybeAssertion<T> Must<T>(this Expression<Func<IMaybe<T>>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (MaybeAssertion<T>)assertion.Named($"Optional of {typeof(T).Name} {name}");
      }

      public static ResultAssertion<T> Must<T>(this IResult<T> value) => new ResultAssertion<T>(value);

      public static ResultAssertion<T> Must<T>(this Expression<Func<IResult<T>>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (ResultAssertion<T>)assertion.Named($"Result of {typeof(T).Name} {name}");
      }

      public static MatchedAssertion<T> Must<T>(this IMatched<T> value) => new MatchedAssertion<T>(value);

      public static MatchedAssertion<T> Must<T>(this Expression<Func<IMatched<T>>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (MatchedAssertion<T>)assertion.Named($"Match of {typeof(T).Name} {name}");
      }

      public static CompletionAssertion<T> Must<T>(this ICompletion<T> value) => new CompletionAssertion<T>(value);

      public static CompletionAssertion<T> Must<T>(this Expression<Func<ICompletion<T>>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (CompletionAssertion<T>)assertion.Named($"Async Result of {typeof(T).Name} {name}");
      }

      public static FileNameAssertion Must(this FileName value) => new FileNameAssertion(value);

      public static FileNameAssertion Must(this Expression<Func<FileName>> expression)
      {
         var (_, value) = resolve(expression);
         var assertion = value.Must();

         return (FileNameAssertion)assertion.Named($"File {value}");
      }

      public static FolderNameAssertion Must(this FolderName value) => new FolderNameAssertion(value);

      public static FolderNameAssertion Must(this Expression<Func<FolderName>> expression)
      {
         var (_, value) = resolve(expression);
         var assertion = value.Must();

         return (FolderNameAssertion)assertion.Named($"Folder {value}");
      }

      public static DictionaryAssertion<TKey, TValue> Must<TKey, TValue>(this Dictionary<TKey, TValue> value)
      {
         return new DictionaryAssertion<TKey, TValue>(value);
      }

      public static DictionaryAssertion<TKey, TValue> Must<TKey, TValue>(this Expression<Func<Dictionary<TKey, TValue>>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (DictionaryAssertion<TKey, TValue>)assertion.Named($"Dictionary {name}");
      }

      public static DictionaryAssertion<TKey, TValue> Must<TKey, TValue>(this Hash<TKey, TValue> value)
      {
         return new DictionaryAssertion<TKey, TValue>(value);
      }

      public static DictionaryAssertion<TKey, TValue> Must<TKey, TValue>(this Expression<Func<Hash<TKey, TValue>>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (DictionaryAssertion<TKey, TValue>)assertion.Named($"Dictionary {name}");
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

         return (DictionaryAssertion<TKey, TValue>)assertion.Named($"Dictionary {name}");
      }

      public static DictionaryAssertion<TKey, TValue> Must<TKey, TValue>(this AutoHash<TKey, TValue> value)
      {
         Dictionary<TKey, TValue> hash = value.AnyHash().ForceValue();
         return hash.Must();
      }

      public static DictionaryAssertion<TKey, TValue> Must<TKey, TValue>(this Expression<Func<AutoHash<TKey, TValue>>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (DictionaryAssertion<TKey, TValue>)assertion.Named($"Dictionary {name}");
      }

      public static TypeAssertion Must(this Type value) => new TypeAssertion(value);

      public static TypeAssertion Must(this Expression<Func<Type>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (TypeAssertion)assertion.Named($"Type {value.Name} {name}");
      }

      public static MatcherAssertion Must(this Matcher value) => new MatcherAssertion(value);

      public static MatcherAssertion Must(this Expression<Func<Matcher>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (MatcherAssertion)assertion.Named($"Matcher {name}");
      }

      public static TypedAssertion<T> MustOfType<T>(this T value) => new TypedAssertion<T>(value);

      public static TypedAssertion<T> MustOfType<T>(this Expression<Func<T>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (TypedAssertion<T>)assertion.Named($"Object typed {typeof(T).Name} {name}");
      }

      public static EnumAssertion<TEnum> Must<TEnum>(this TEnum value) where TEnum : struct, Enum => new EnumAssertion<TEnum>(value);

      public static EnumAssertion<TEnum> Must<TEnum>(this Expression<Func<TEnum>> expression) where TEnum : struct, Enum
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (EnumAssertion<TEnum>)assertion.Named($"{value.GetType().Name} {name}");
      }
   }
}