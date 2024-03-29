﻿using System;
using System.Collections;
using System.Collections.Generic;
using Core.Computers;
using Core.Configurations;
using Core.Matching;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.Lazy.LazyMonads;
using static Core.Monads.MonadFunctions;

namespace Core.Collections;

public static class HashExtensions
{
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
         throw fail(message);
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

   [Obsolete("Use Items")]
   public static bool Map<TKey, TValue>(this IHash<TKey, TValue> hash, TKey key, out TValue value)
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

   [Obsolete("Use Items")]
   public static Maybe<TValue> Maybe<TKey, TValue>(this IHash<TKey, TValue> hash, TKey key)
   {
      if (hash.ContainsKey(key))
      {
         return hash[key];
      }
      else
      {
         return nil;
      }
   }

   [Obsolete("Use Items")]
   public static Maybe<TValue> Map<TKey, TValue>(this IHash<TKey, TValue> hash, TKey key)
   {
      return maybe(hash.ContainsKey(key), () => hash[key]);
   }

   [Obsolete("Use Items")]
   public static Maybe<TValue> Of<TKey, TValue>(this IHash<TKey, TValue> hash, TKey key)
   {
      return maybe(hash.ContainsKey(key), () => hash[key]);
   }

   [Obsolete("Use Items")]
   public static Maybe<TResult> Map<TKey, TValue, TResult>(this IHash<TKey, TValue> hash, TKey key,
      Func<TValue, TResult> func)
   {
      return maybe(hash.ContainsKey(key), () => func(hash[key]));
   }

   [Obsolete("Use Items")]
   public static Maybe<TResult> Map<TKey, TValue, TResult>(this IHash<TKey, TValue> hash, TKey key,
      Func<TValue, Maybe<TResult>> func)
   {
      return maybe(hash.ContainsKey(key), () => func(hash[key]));
   }

   [Obsolete("Use Items")]
   public static TValue FlatMap<TKey, TValue>(this IHash<TKey, TValue> hash, TKey key, Func<TValue> defaultFunc)
   {
      return hash.ContainsKey(key) ? hash[key] : defaultFunc();
   }

   [Obsolete("Use Items")]
   public static TResult FlatMap<TKey, TValue, TResult>(this IHash<TKey, TValue> hash, TKey key,
      Func<TValue, TResult> ifTrue, Func<TResult> ifFalse)
   {
      return hash.ContainsKey(key) ? ifTrue(hash[key]) : ifFalse();
   }

   [Obsolete("Use Items")]
   public static TResult FlatMap<TKey, TValue, TResult>(this IHash<TKey, TValue> hash, TKey key,
      Func<TValue, TResult> ifTrue, TResult ifFalse)
   {
      return hash.ContainsKey(key) ? ifTrue(hash[key]) : ifFalse;
   }

   [Obsolete("Use |")]
   public static TValue DefaultTo<TKey, TValue>(this IHash<TKey, TValue> hash, TKey key, Func<TValue> defaultFunc)
   {
      return hash.ContainsKey(key) ? hash[key] : defaultFunc();
   }

   [Obsolete("Use |")]
   public static TValue DefaultTo<TKey, TValue>(this IHash<TKey, TValue> hash, TKey key, TValue defaultValue)
   {
      return hash.ContainsKey(key) ? hash[key] : defaultValue;
   }

   [Obsolete("Use Items")]
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

   public static Hash<TKey, TValue> ToHash<TKey, TValue>(this IEnumerable<(TKey, TValue)> enumerable)
   {
      var result = new Hash<TKey, TValue>();
      foreach (var (key, value) in enumerable)
      {
         result[key] = value;
      }

      return result;
   }

   public static AutoHash<TKey, TValue> ToAutoHash<TKey, TValue>(this IEnumerable<(TKey, TValue)> enumerable, Func<TKey, TValue> defaultLambda,
      bool autoAddDefault = false)
   {
      var result = new AutoHash<TKey, TValue>(defaultLambda, autoAddDefault);
      foreach (var (key, value) in enumerable)
      {
         result[key] = value;
      }

      return result;
   }

   public static AutoHash<TKey, TValue> ToAutoHash<TKey, TValue>(this IEnumerable<(TKey, TValue)> enumerable, TValue defaultValue,
      bool autoAddDefault = false)
   {
      var result = new AutoHash<TKey, TValue>(defaultValue, autoAddDefault);
      foreach (var (key, value) in enumerable)
      {
         result[key] = value;
      }

      return result;
   }

   public static StringHash ToStringHash(this IEnumerable<(string, string)> enumerable, bool ignoreCase)
   {
      var result = new StringHash(ignoreCase);
      foreach (var (key, value) in enumerable)
      {
         result[key] = value;
      }

      return result;
   }

   public static StringHash ToAutoStringHash(this IEnumerable<(string, string)> enumerable, bool ignoreCase, Func<string, string> defaultLambda,
      bool autoAddDefault = false)
   {
      var result = new AutoStringHash(ignoreCase, defaultLambda, autoAddDefault);
      foreach (var (key, value) in enumerable)
      {
         result[key] = value;
      }

      return result;
   }

   public static StringHash ToAutoStringHash(this IEnumerable<(string, string)> enumerable, bool ignoreCase, string defaultValue,
      bool autoAddDefault = false)
   {
      var result = new AutoStringHash(ignoreCase, defaultValue, autoAddDefault);
      foreach (var (key, value) in enumerable)
      {
         result[key] = value;
      }

      return result;
   }

   public static StringHash<TValue> ToStringHash<TValue>(this IEnumerable<(string, TValue)> enumerable, bool ignoreCase)
   {
      var result = new StringHash<TValue>(ignoreCase);
      foreach (var (key, value) in enumerable)
      {
         result[key] = value;
      }

      return result;
   }

   public static AutoStringHash<TValue> ToAutoStringHash<TValue>(this IEnumerable<(string, TValue)> enumerable, bool ignoreCase,
      Func<string, TValue> defaultLambda, bool autoAddDefault = false)
   {
      var result = new AutoStringHash<TValue>(ignoreCase, defaultLambda, autoAddDefault);
      foreach (var (key, value) in enumerable)
      {
         result[key] = value;
      }

      return result;
   }

   public static AutoStringHash<TValue> ToAutoStringHash<TValue>(this IEnumerable<(string, TValue)> enumerable, bool ignoreCase,
      TValue defaultValue, bool autoAddDefault = false)
   {
      var result = new AutoStringHash<TValue>(ignoreCase, defaultValue, autoAddDefault);
      foreach (var (key, value) in enumerable)
      {
         result[key] = value;
      }

      return result;
   }

   public static Result<Hash<TKey, TValue>> TryToHash<TKey, TValue, T>(this IEnumerable<T> enumerable, Func<T, TKey> keySelector,
      Func<T, Result<TValue>> valueSelector)
   {
      var result = new Hash<TKey, TValue>();
      foreach (var item in enumerable)
      {
         var _selector = valueSelector(item);
         if (_selector is (true, var selector))
         {
            result[keySelector(item)] = selector;
         }
         else
         {
            return _selector.Exception;
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
         var _selector = valueSelector(item);
         if (_selector is (true, var selector))
         {
            result[keySelector(item)] = selector;
         }
         else
         {
            return _selector.Exception;
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
         var _selector = valueSelector(item);
         if (_selector is (true, var selector))
         {
            result[keySelector(item)] = selector;
         }
         else
         {
            return _selector.Exception;
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
         var _value = lazy.result(() => valueSelector(item));
         var _key = _value.Then(_ => keySelector(item));
         if (_key)
         {
            result[_key] = _value;
         }
         else
         {
            return _key.Exception;
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
         var _value = lazy.result(() => valueSelector(item));
         var _key = _value.Then(_ => keySelector(item));
         if (_key)
         {
            result[_key] = _value;
         }
         else
         {
            return _key.Exception;
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
         var _value = lazy.result(() => valueSelector(item));
         var _key = _value.Then(_ => keySelector(item));
         if (_key)
         {
            result[_key] = _value;
         }
         else
         {
            return _key.Exception;
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
         var _selector = tryTo(() => valueSelector(item));
         if (_selector is (true, var selector))
         {
            result[keySelector(item)] = selector;
         }
         else
         {
            return _selector.Exception;
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
         var _selector = tryTo(() => valueSelector(item));
         if (_selector is (true, var selector))
         {
            result[keySelector(item)] = selector;
         }
         else
         {
            return _selector.Exception;
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
         var _selector = tryTo(() => valueSelector(item));
         if (_selector is (true, var selector))
         {
            result[keySelector(item)] = selector;
         }
         else
         {
            return _selector.Exception;
         }
      }

      return result;
   }

   public static IEnumerator<T> AsEnumerator<T>(this IEnumerable enumerable) => ((IEnumerable<T>)enumerable).GetEnumerator();

   public static Result<Setting> ToSetting<TKey, TValue>(this IHash<TKey, TValue> hash)
   {
      try
      {
         var _internalHash = hash.AnyHash();
         if (_internalHash is (true, var internalHash))
         {
            var toSetting = new Setting();
            foreach (var (key, value) in internalHash)
            {
               var keyAsString = key.ToString();
               toSetting.SetItem(keyAsString, new Item(keyAsString, value.ToString()));
            }

            return toSetting;
         }
         else
         {
            return _internalHash.Exception;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<Configuration> ToConfiguration<TKey, TValue>(this IHash<TKey, TValue> hash, FileName file, string name = Setting.ROOT_NAME,
      bool save = false)
   {
      var _configuration = hash.ToSetting().Map(setting => new Configuration(file, setting.items, name));
      if (_configuration is (true, var configuration))
      {
         if (save)
         {
            return configuration.Save().Map(_ => configuration);
         }
         else
         {
            return configuration;
         }
      }
      else
      {
         return _configuration.Exception;
      }
   }

   public static Result<Hash<TKey, TValue[]>> GroupToHash<T, TKey, TValue>(this IEnumerable<T> enumerable, Func<T, TKey> keyFunc,
      Func<T, TValue> valueFunc)
   {
      try
      {
         var hash = new AutoHash<TKey, List<TValue>>(_ => new List<TValue>(), true);
         foreach (var item in enumerable)
         {
            var key = keyFunc(item);
            var value = valueFunc(item);
            hash[key].Add(value);
         }

         var result = new Hash<TKey, TValue[]>();
         foreach (var (key, value) in hash)
         {
            result[key] = value.ToArray();
         }

         return result;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<Hash<TKey, T[]>> GroupToHash<T, TKey>(this IEnumerable<T> enumerable, Func<T, TKey> keyFunc)
   {
      try
      {
         var hash = new AutoHash<TKey, List<T>>(_ => new List<T>(), true);
         foreach (var item in enumerable)
         {
            var key = keyFunc(item);
            hash[key].Add(item);
         }

         var result = new Hash<TKey, T[]>();
         foreach (var (key, value) in hash)
         {
            result[key] = value.ToArray();
         }

         return result;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<Hash<TKey, TValue>> GroupToHash<T, TKey, TValue>(this IEnumerable<T> enumerable, Func<T, TKey> keyFunc,
      Func<Maybe<TValue>, T, TValue> valueFunc)
   {
      try
      {
         var hash = new Hash<TKey, TValue>();
         foreach (var item in enumerable)
         {
            var key = keyFunc(item);
            var _value = hash.Maybe[key];
            var value = valueFunc(_value, item);
            hash[key] = value;
         }

         return hash;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Maybe<TaggedValue<MatchResult>> Matches(this Hash<string, Pattern> patterns, string input)
   {
      foreach (var (name, pattern) in patterns)
      {
         var _result = input.Matches(pattern);
         if (_result is (true, var result))
         {
            return new TaggedValue<MatchResult>(name, result);
         }
      }

      return nil;
   }

   public static IHashMaybe<TKey, TValue> Maybe<TKey, TValue>(this IHash<TKey, TValue> hash) => new(hash);
}