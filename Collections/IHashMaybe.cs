using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Collections;

// ReSharper disable once InconsistentNaming
public class IHashOptional<TKey, TValue>
{
   protected IHash<TKey, TValue> hash;

   public IHashMaybe(IHash<TKey, TValue> hash)
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
   }
}