using System;
using Core.Assertions;
using Core.Monads;

namespace Core.Collections
{
   public class HashTrying<TKey, TValue>
   {
      protected Hash<TKey, TValue> hash;

      public HashTrying(Hash<TKey, TValue> hash) => this.hash = hash;

      public Result<TValue> this[TKey key] => hash.Must().HaveKeyOf(key).OrFailure().Map(d => d[key].Success());

      public Result<TValue> Find(TKey key, Func<TKey, Result<TValue>> defaultValue, bool addIfNotFound = false)
      {
         var result = this[key];
         if (result.IsSuccessful)
         {
            return result;
         }
         else
         {
            if (defaultValue(key).ValueOrOriginal(out var value, out var original))
            {
               if (addIfNotFound)
               {
                  hash.Add(key, value);
               }

               return value.Success();
            }
            else
            {
               return original;
            }
         }
      }

      public Result<TValue> Map(TKey key, string notFoundMessage)
      {
         if (hash.If(key, out var value))
         {
            return value.Success();
         }
         else
         {
            return notFoundMessage.Failure<TValue>();
         }
      }

      public Result<TValue> Map(TKey key, Func<string> notFoundMessage)
      {
         if (hash.If(key, out var value))
         {
            return value.Success();
         }
         else
         {
            return notFoundMessage().Failure<TValue>();
         }
      }
   }
}