﻿using System;
using Core.Assertions;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;

namespace Core.Collections
{
   public class HashTrying<TKey, TValue>
   {
      Hash<TKey, TValue> hash;

      public HashTrying(Hash<TKey, TValue> hash) => this.hash = hash;

      public IResult<TValue> this[TKey key]
      {
         get => assert(() => hash).Must().HaveKeyOf(key).OrFailure().Map(d => d[key].Success());
      }

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
   }
}