﻿using System;
using System.Collections;
using System.Collections.Generic;
using Core.Configurations;
using Core.Exceptions;
using Core.Monads;
using Core.Strings;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Collections
{
   public static class HashExtensions
   {
      public static TValue Default<TKey, TValue>(this Hash<TKey, TValue> hash, TKey key, TValue defaultValue)
      {
         return hash.FlatMap(key, v => v, () => defaultValue);
      }

      public static TValue Default<TValue>(this StringHash<TValue> hash, string key, TValue defaultValue)
      {
         return hash.FlatMap(key, v => v, () => defaultValue);
      }

      public static TValue Value<TKey, TValue>(this IHash<TKey, TValue> hash, TKey key)
      {
         return hash.Value(key, $"Value for key {key} not found");
      }

      public static TValue Value<TKey, TValue>(this IHash<TKey, TValue> hash, TKey key, string message)
      {
         if (hash.ContainsKey(key))
         {
            return hash[key];
         }
         else
         {
            throw message.Throws();
         }
      }

      public static Result<TValue> Require<TKey, TValue>(this IHash<TKey, TValue> hash, TKey key)
      {
         return hash.Require(key, $"Value for key {key} not found");
      }

      public static Result<TValue> Require<TKey, TValue>(this IHash<TKey, TValue> hash, TKey key, string message)
      {
         if (hash.ContainsKey(key))
         {
            return hash[key];
         }
         else
         {
            return fail(message);
         }
      }

      public static AutoHash<TKey, TValue> ToAutoHash<TKey, TValue>(this Hash<TKey, TValue> hash, TValue defaultValue)
      {
         return new AutoHash<TKey, TValue>(hash, hash.Comparer) { Default = DefaultType.Value, DefaultValue = defaultValue };
      }

      public static AutoStringHash<TValue> ToAutoStringHash<TValue>(this StringHash<TValue> hash, TValue defaultValue)
      {
         return new AutoStringHash<TValue>(hash.IgnoreCase, hash) { Default = DefaultType.Value, DefaultValue = defaultValue };
      }

      public static AutoStringHash ToAutoStringHash(this StringHash hash, string defaultValue)
      {
         return new AutoStringHash(hash.IgnoreCase, hash) { Default = DefaultType.Value, DefaultValue = defaultValue };
      }

      public static AutoHash<TKey, TValue> ToAutoHash<TKey, TValue>(this Hash<TKey, TValue> hash,
         Func<TKey, TValue> defaultLambda)
      {
         return new AutoHash<TKey, TValue>(hash, hash.Comparer) { Default = DefaultType.Lambda, DefaultLambda = defaultLambda };
      }

      public static AutoStringHash<TValue> ToAutoStringHash<TValue>(this StringHash<TValue> hash, Func<string, TValue> defaultLambda)
      {
         return new AutoStringHash<TValue>(hash.IgnoreCase, hash) { Default = DefaultType.Lambda, DefaultLambda = defaultLambda };
      }

      public static AutoStringHash ToAutoStringHash(this StringHash hash, Func<string, string> defaultLambda)
      {
         return new AutoStringHash(hash.IgnoreCase, hash) { Default = DefaultType.Lambda, DefaultLambda = defaultLambda };
      }

      public static string Format(this Hash<string, string> hash, string format) => new Formatter(hash).Format(format);

      public static bool If<TKey, TValue>(this IHash<TKey, TValue> hash, TKey key, out TValue value)
      {
         if (hash.ContainsKey(key))
         {
            value = hash[key];
            return true;
         }
         else
         {
            value = default;
            return false;
         }
      }

      public static Maybe<TValue> Map<TKey, TValue>(this IHash<TKey, TValue> hash, TKey key)
      {
         return maybe(hash.ContainsKey(key), () => hash[key]);
      }

      public static Maybe<TResult> Map<TKey, TValue, TResult>(this IHash<TKey, TValue> hash, TKey key,
         Func<TValue, TResult> func)
      {
         return maybe(hash.ContainsKey(key), () => func(hash[key]));
      }

      public static Maybe<TResult> Map<TKey, TValue, TResult>(this IHash<TKey, TValue> hash, TKey key,
         Func<TValue, Maybe<TResult>> func)
      {
         return maybe(hash.ContainsKey(key), () => func(hash[key]));
      }

      public static TValue FlatMap<TKey, TValue>(this IHash<TKey, TValue> hash, TKey key, Func<TValue> defaultFunc)
      {
         return hash.ContainsKey(key) ? hash[key] : defaultFunc();
      }

      public static TResult FlatMap<TKey, TValue, TResult>(this IHash<TKey, TValue> hash, TKey key,
         Func<TValue, TResult> ifTrue, Func<TResult> ifFalse)
      {
         return hash.ContainsKey(key) ? ifTrue(hash[key]) : ifFalse();
      }

      public static TResult FlatMap<TKey, TValue, TResult>(this IHash<TKey, TValue> hash, TKey key,
         Func<TValue, TResult> ifTrue, TResult ifFalse)
      {
         return hash.ContainsKey(key) ? ifTrue(hash[key]) : ifFalse;
      }

      public static TValue DefaultTo<TKey, TValue>(this IHash<TKey, TValue> hash, TKey key, Func<TValue> defaultFunc)
      {
         return hash.ContainsKey(key) ? hash[key] : defaultFunc();
      }

      public static TValue DefaultTo<TKey, TValue>(this IHash<TKey, TValue> hash, TKey key, TValue defaultValue)
      {
         return hash.ContainsKey(key) ? hash[key] : defaultValue;
      }

      public static Maybe<TValue> Get<TKey, TValue>(this IHash<TKey, TValue> hash, TKey key)
      {
         return maybe(hash.ContainsKey(key), () => hash[key]);
      }

      public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> item, out TKey key, out TValue value)
      {
         key = item.Key;
         value = item.Value;
      }

      public static Hash<TKey, TValue> ToHash<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
      {
         return new Hash<TKey, TValue>(dictionary);
      }

      public static StringHash<TValue> ToStringHash<TValue>(this Dictionary<string, TValue> dictionary, bool ignoreCase)
      {
         return new StringHash<TValue>(ignoreCase, dictionary);
      }

      public static StringHash ToStringHash(this Dictionary<string, string> dictionary, bool ignoreCase)
      {
         return new StringHash(ignoreCase, dictionary);
      }

      public static Hash<TKey, TValue> ToHash<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
      {
         return new Hash<TKey, TValue>(dictionary, comparer);
      }

      public static Hash<TKey, TValue> ToHash<TKey, TValue>(this IEnumerable<TValue> enumerable, Func<TValue, TKey> keySelector)
      {
         var result = new Hash<TKey, TValue>();
         foreach (var item in enumerable)
         {
            result[keySelector(item)] = item;
         }

         return result;
      }

      public static StringHash<TValue> ToStringHash<TValue>(this IEnumerable<TValue> enumerable, Func<TValue, string> keySelector, bool ignoreCase)
      {
         var result = new StringHash<TValue>(ignoreCase);
         foreach (var item in enumerable)
         {
            result[keySelector(item)] = item;
         }

         return result;
      }

      public static StringHash ToStringHash<TValue>(this IEnumerable<TValue> enumerable, Func<TValue, string> keySelector,
         Func<TValue, string> valueSelector, bool ignoreCase)
      {
         var result = new StringHash(ignoreCase);
         foreach (var item in enumerable)
         {
            result[keySelector(item)] = valueSelector(item);
         }

         return result;
      }

      public static Hash<TKey, TValue> ToHash<TKey, TValue>(this IEnumerable<TValue> enumerable, Func<TValue, TKey> keySelector,
         IEqualityComparer<TKey> comparer)
      {
         var result = new Hash<TKey, TValue>(comparer);
         foreach (var item in enumerable)
         {
            result[keySelector(item)] = item;
         }

         return result;
      }

      public static Hash<TKey, TValue> ToHash<TKey, TValue, T>(this IEnumerable<T> enumerable, Func<T, TKey> keySelector,
         Func<T, TValue> valueSelector)
      {
         var result = new Hash<TKey, TValue>();
         foreach (var item in enumerable)
         {
            result[keySelector(item)] = valueSelector(item);
         }

         return result;
      }

      public static StringHash<TValue> ToStringHash<TValue, T>(this IEnumerable<T> enumerable, Func<T, string> keySelector,
         Func<T, TValue> valueSelector, bool ignoreCase)
      {
         var result = new StringHash<TValue>(ignoreCase);
         foreach (var item in enumerable)
         {
            result[keySelector(item)] = valueSelector(item);
         }

         return result;
      }

      public static Hash<TKey, TValue> ToHash<TKey, TValue, T>(this IEnumerable<T> enumerable, Func<T, TKey> keySelector,
         Func<T, TValue> valueSelector, IEqualityComparer<TKey> comparer)
      {
         var result = new Hash<TKey, TValue>(comparer);
         foreach (var item in enumerable)
         {
            result[keySelector(item)] = valueSelector(item);
         }

         return result;
      }

      public static Result<Hash<TKey, TValue>> TryToHash<TKey, TValue, T>(this IEnumerable<T> enumerable, Func<T, TKey> keySelector,
         Func<T, Result<TValue>> valueSelector)
      {
         var result = new Hash<TKey, TValue>();
         foreach (var item in enumerable)
         {
            if (valueSelector(item).If(out var selector, out var exception))
            {
               result[keySelector(item)] = selector;
            }
            else
            {
               return exception;
            }
         }

         return result;
      }

      public static Result<StringHash<TValue>> TryToStringHash<TValue, T>(this IEnumerable<T> enumerable, Func<T, string> keySelector,
         Func<T, Result<TValue>> valueSelector, bool ignoreCase)
      {
         var result = new StringHash<TValue>(ignoreCase);
         foreach (var item in enumerable)
         {
            if (valueSelector(item).If(out var selector, out var exception))
            {
               result[keySelector(item)] = selector;
            }
            else
            {
               return exception;
            }
         }

         return result;
      }

      public static Result<Hash<TKey, TValue>> TryToHash<TKey, TValue, T>(this IEnumerable<T> enumerable, Func<T, TKey> keySelector,
         Func<T, Result<TValue>> valueSelector, IEqualityComparer<TKey> comparer)
      {
         var result = new Hash<TKey, TValue>(comparer);
         foreach (var item in enumerable)
         {
            if (valueSelector(item).If(out var selector, out var exception))
            {
               result[keySelector(item)] = selector;
            }
            else
            {
               return exception;
            }
         }

         return result;
      }

      public static Result<Hash<TKey, TValue>> TryToHash<TKey, TValue, T>(this IEnumerable<T> enumerable, Func<T, Result<TKey>> keySelector,
         Func<T, Result<TValue>> valueSelector)
      {
         var result = new Hash<TKey, TValue>();
         foreach (var item in enumerable)
         {
            var pair =
               from selector in valueSelector(item)
               from key in keySelector(item)
               select (value: selector, key);
            if (pair.If(out var keyValue, out var exception))
            {
               result[keyValue.key] = keyValue.value;
            }
            else
            {
               return exception;
            }
         }

         return result;
      }

      public static Result<StringHash<TValue>> TryToStringHash<TValue, T>(this IEnumerable<T> enumerable, Func<T, Result<string>> keySelector,
         Func<T, Result<TValue>> valueSelector, bool ignoreCase)
      {
         var result = new StringHash<TValue>(ignoreCase);
         foreach (var item in enumerable)
         {
            var pair =
               from selector in valueSelector(item)
               from key in keySelector(item)
               select (value: selector, key);
            if (pair.If(out var keyValue, out var exception))
            {
               result[keyValue.key] = keyValue.value;
            }
            else
            {
               return exception;
            }
         }

         return result;
      }

      public static Result<Hash<TKey, TValue>> TryToHash<TKey, TValue, T>(this IEnumerable<T> enumerable, Func<T, Result<TKey>> keySelector,
         Func<T, Result<TValue>> valueSelector, IEqualityComparer<TKey> comparer)
      {
         var result = new Hash<TKey, TValue>(comparer);
         foreach (var item in enumerable)
         {
            var pair =
               from selector in valueSelector(item)
               from key in keySelector(item)
               select (value: selector, key);
            if (pair.If(out var keyValue, out var exception))
            {
               result[keyValue.key] = keyValue.value;
            }
            else
            {
               return exception;
            }
         }

         return result;
      }

      public static Result<Hash<TKey, TValue>> TryToHash<TKey, TValue, T>(this IEnumerable<T> enumerable, Func<T, TKey> keySelector,
         Func<T, TValue> valueSelector)
      {
         var result = new Hash<TKey, TValue>();
         foreach (var item in enumerable)
         {
            if (tryTo(() => valueSelector(item)).If(out var selector, out var exception))
            {
               result[keySelector(item)] = selector;
            }
            else
            {
               return exception;
            }
         }

         return result;
      }

      public static Result<StringHash<TValue>> TryToStringHash<TValue, T>(this IEnumerable<T> enumerable, Func<T, string> keySelector,
         Func<T, TValue> valueSelector, bool ignoreCase)
      {
         var result = new StringHash<TValue>(ignoreCase);
         foreach (var item in enumerable)
         {
            if (tryTo(() => valueSelector(item)).If(out var selector, out var exception))
            {
               result[keySelector(item)] = selector;
            }
            else
            {
               return exception;
            }
         }

         return result;
      }

      public static Result<Hash<TKey, TValue>> TryToHash<TKey, TValue, T>(this IEnumerable<T> enumerable, Func<T, TKey> keySelector,
         Func<T, TValue> valueSelector, IEqualityComparer<TKey> comparer)
      {
         var result = new Hash<TKey, TValue>(comparer);
         foreach (var item in enumerable)
         {
            if (tryTo(() => valueSelector(item)).If(out var selector, out var exception))
            {
               result[keySelector(item)] = selector;
            }
            else
            {
               return exception;
            }
         }

         return result;
      }

      public static IEnumerator<T> AsEnumerator<T>(this IEnumerable enumerable) => ((IEnumerable<T>)enumerable).GetEnumerator();

      public static Result<Configuration> ToConfiguration<TKey, TValue>(this IHash<TKey, TValue> hash)
      {
         try
         {
            if (hash.AnyHash().If(out var internalHash, out var exception))
            {
               var group = new Group();
               foreach (var (key, value) in internalHash)
               {
                  var keyAsString = key.ToString();
                  group.SetItem(keyAsString, new Item(keyAsString, value.ToString()));
               }

               return new Configuration(group);
            }
            else
            {
               return exception;
            }
         }
         catch (Exception exception)
         {
            return exception;
         }
      }
   }
}