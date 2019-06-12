using Core.Monads;
using Core.Objects;
using static Core.Booleans.Assertions;
using static Core.Monads.AttemptFunctions;

namespace Core.Collections
{
   public class RequiredHash<TKey, TValue>
   {
      IHash<TKey, TValue> hash;

      internal RequiredHash(IHash<TKey, TValue> hash)
      {
         this.hash = Ensure(hash, h => h.IsNotNull(), "Hash can't be null");
      }

      public IResult<TValue> this[TKey key] => assert(hash.ContainsKey(key), () => hash[key], () => $"Key {key} not found");
   }
}