using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Collections;

public class HashMaybe<TKey, TValue>
{
   protected Hash<TKey, TValue> hash;

   public HashMaybe(Hash<TKey, TValue> hash)
   {
      this.hash = hash;
   }

   public Maybe<TValue> this[TKey key]
   {
      get
      {
         if (hash.TryGetValue(key, out var item))
         {
            return item;
         }
         else
         {
            return nil;
         }
      }
      set
      {
         if (value)
         {
            hash[key] = value;
         }
         else
         {
            hash.Remove(key);
         }
      }
   }
}