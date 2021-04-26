using System;
using Core.Assertions;
using Core.Monads;

namespace Core.Collections
{
   public class HashTrying<TKey, TValue>
   {
      protected Hash<TKey, TValue> hash;

      public HashTrying(Hash<TKey, TValue> hash) => this.hash = hash;

      public IResult<TValue> this[TKey key] => hash.Must().HaveKeyOf(key).OrFailure().Map(d => d[key].Success());

      public IResult<TValue> Find(TKey key, Func<TKey, IResult<TValue>> defaultValue, bool addIfNotFound = false)
      {
         var result = this[key];
         if (result.HasValue)
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

      public IResult<TValue> Map(TKey key, string notFoundMessage)
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

      public IResult<TValue> Map(TKey key, Func<string> notFoundMessage)
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