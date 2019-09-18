using Core.Assertions;
using Core.Monads;
using static Core.Monads.AttemptFunctions;

namespace Core.Collections
{
   public class RequiredHash<TKey, TValue>
   {
      IHash<TKey, TValue> hash;

      internal RequiredHash(IHash<TKey, TValue> hash)
      {
         this.hash = hash.Must().Ensure<IHash<TKey, TValue>>();
      }

      public IResult<TValue> this[TKey key] => assert(hash.ContainsKey(key), () => hash[key], () => $"Key {key} not found");
   }
}