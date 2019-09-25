using System;
using System.Collections;
using System.Collections.Generic;
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

		public static IResult<TValue> Require<TKey, TValue>(this IHash<TKey, TValue> hash, TKey key)
		{
			return hash.Require(key, $"Value for key {key} not found");
		}

		public static IResult<TValue> Require<TKey, TValue>(this IHash<TKey, TValue> hash, TKey key, string message)
		{
			if (hash.ContainsKey(key))
         {
            return hash[key].Success();
         }
         else
         {
            return message.Failure<TValue>();
         }
      }

		public static AutoHash<TKey, TValue> ToAutoHash<TKey, TValue>(this Hash<TKey, TValue> hash, TValue defaultValue)
		{
			return new AutoHash<TKey, TValue>(hash) { Default = DefaultType.Value, DefaultValue = defaultValue };
		}

		public static AutoHash<TKey, TValue> ToAutoHash<TKey, TValue>(this Hash<TKey, TValue> hash,
			Func<TKey, TValue> defaultLambda)
		{
			return new AutoHash<TKey, TValue>(hash) { Default = DefaultType.Lambda, DefaultLambda = defaultLambda };
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

		public static IMaybe<TValue> Map<TKey, TValue>(this IHash<TKey, TValue> hash, TKey key)
		{
			if (hash.ContainsKey(key))
         {
            return hash[key].Some();
         }
         else
         {
            return none<TValue>();
         }
      }

		public static IMaybe<TResult> Map<TKey, TValue, TResult>(this IHash<TKey, TValue> hash, TKey key,
			Func<TValue, TResult> func)
		{
			if (hash.ContainsKey(key))
         {
            return func(hash[key]).Some();
         }
         else
         {
            return none<TResult>();
         }
      }

		public static IMaybe<TResult> Map<TKey, TValue, TResult>(this IHash<TKey, TValue> hash, TKey key,
			Func<TValue, IMaybe<TResult>> func)
		{
			if (hash.ContainsKey(key))
         {
            return func(hash[key]);
         }
         else
         {
            return none<TResult>();
         }
      }

		public static TValue FlatMap<TKey, TValue>(this IHash<TKey, TValue> hash, TKey key, Func<TValue> defaultFunc)
		{
			if (hash.ContainsKey(key))
         {
            return hash[key];
         }
         else
         {
            return defaultFunc();
         }
      }

		public static TResult FlatMap<TKey, TValue, TResult>(this IHash<TKey, TValue> hash, TKey key,
			Func<TValue, TResult> ifTrue, Func<TResult> ifFalse)
		{
			if (hash.ContainsKey(key))
         {
            return ifTrue(hash[key]);
         }
         else
         {
            return ifFalse();
         }
      }

		public static TResult FlatMap<TKey, TValue, TResult>(this IHash<TKey, TValue> hash, TKey key,
			Func<TValue, TResult> ifTrue, TResult ifFalse)
		{
			if (hash.ContainsKey(key))
         {
            return ifTrue(hash[key]);
         }
         else
         {
            return ifFalse;
         }
      }

		public static TValue DefaultTo<TKey, TValue>(this IHash<TKey, TValue> hash, TKey key, Func<TValue> defaultFunc)
		{
			if (hash.ContainsKey(key))
         {
            return hash[key];
         }
         else
         {
            return defaultFunc();
         }
      }

		public static TValue DefaultTo<TKey, TValue>(this IHash<TKey, TValue> hash, TKey key, TValue defaultValue)
		{
			if (hash.ContainsKey(key))
         {
            return hash[key];
         }
         else
         {
            return defaultValue;
         }
      }

		public static IMaybe<TValue> Get<TKey, TValue>(this IHash<TKey, TValue> hash, TKey key)
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

		public static Hash<TKey, TValue> ToHash<TKey, TValue>(this IEnumerable<TValue> enumerable,
			Func<TValue, TKey> keySelector)
		{
			var result = new Hash<TKey, TValue>();
			foreach (var item in enumerable)
         {
            result[keySelector(item)] = item;
         }

         return result;
		}

		public static Hash<TKey, TValue> ToHash<TKey, TValue, T>(this IEnumerable<T> enumerable,
			Func<T, TKey> keySelector, Func<T, TValue> valueSelector)
		{
			var result = new Hash<TKey, TValue>();
			foreach (var item in enumerable)
         {
            result[keySelector(item)] = valueSelector(item);
         }

         return result;
		}

		public static IResult<Hash<TKey, TValue>> TryToHash<TKey, TValue, T>(this IEnumerable<T> enumerable,
			Func<T, TKey> keySelector, Func<T, IResult<TValue>> valueSelector)
		{
			var result = new Hash<TKey, TValue>();
			foreach (var item in enumerable)
         {
            if (valueSelector(item).ValueOrCast<Hash<TKey, TValue>>(out var selector, out var original))
            {
               result[keySelector(item)] = selector;
            }
            else
            {
               return original;
            }
         }

         return result.Success();
		}

		public static IResult<Hash<TKey, TValue>> TryToHash<TKey, TValue, T>(this IEnumerable<T> enumerable,
			Func<T, IResult<TKey>> keySelector, Func<T, IResult<TValue>> valueSelector)
		{
			var result = new Hash<TKey, TValue>();
			foreach (var item in enumerable)
			{
				var pair =
					from selector in valueSelector(item)
					from key in keySelector(item)
					select (value: selector, key);
				if (pair.ValueOrOriginal(out var keyValue, out var original))
            {
               result[keyValue.key] = keyValue.value;
            }
            else
            {
               return original.ExceptionAs<Hash<TKey, TValue>>();
            }
         }

			return result.Success();
		}

		public static IResult<Hash<TKey, TValue>> TryToHash<TKey, TValue, T>(this IEnumerable<T> enumerable,
			Func<T, TKey> keySelector, Func<T, TValue> valueSelector)
		{
			var result = new Hash<TKey, TValue>();
			foreach (var item in enumerable)
         {
            if (tryTo(() => valueSelector(item)).ValueOrCast<Hash<TKey, TValue>>(out var selector, out var original))
            {
               result[keySelector(item)] = selector;
            }
            else
            {
               return original;
            }
         }

         return result.Success();
		}

		public static IEnumerator<T> AsEnumerator<T>(this IEnumerable enumerable) => ((IEnumerable<T>)enumerable).GetEnumerator();
	}
}