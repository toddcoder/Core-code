using System;
using Core.Assertions;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Collections;

public class HashTrying<TKey, TValue>
{
   protected Hash<TKey, TValue> hash;

   public HashTrying(Hash<TKey, TValue> hash) => this.hash = hash;

   public Optional<TValue> this[TKey key] => hash.Must().HaveKeyOf(key).OrFailure().Map(d => d[key].Success());

   public Optional<TValue> Find(TKey key, Func<TKey, Optional<TValue>> defaultValue, bool addIfNotFound = false)
   {
      var result = this[key];
      if (result)
      {
         return result;
      }
      else
      {
         var _value = defaultValue(key);
         if (_value is (true, var value))
         {
            if (addIfNotFound)
            {
               hash.Add(key, value);
            }

            return value;
         }
         else
         {
            return _value.Exception;
         }
      }
   }

   public Optional<TValue> Map(TKey key, string notFoundMessage)
   {
      var _value = hash.Maybe[key];
      if (_value is (true, var value))
      {
         return value;
      }
      else
      {
         return fail(notFoundMessage);
      }
   }

   public Optional<TValue> Map(TKey key, Func<string> notFoundMessage)
   {
      var _value = hash.Maybe[key];
      if (_value is (true, var value))
      {
         return value;
      }
      else
      {
         return fail(notFoundMessage());
      }
   }
}