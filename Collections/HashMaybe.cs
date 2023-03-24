using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Collections;

public class HashOptional<TKey, TValue>
{
   protected Hash<TKey, TValue> hash;

   public HashMaybe(Hash<TKey, TValue> hash)
   {
      this.hash = hash;
   }

   public Optional<TValue> this[TKey key]
   {
      get
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