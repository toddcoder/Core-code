using Core.Assertions;
using Core.Monads;

namespace Core.Collections
{
   public class RequiredHash<TKey, TValue>
   {
      IHash<TKey, TValue> hash;

      internal RequiredHash(IHash<TKey, TValue> hash)
      {
         this.hash = hash.Must().Ensure<IHash<TKey, TValue>>();
      }

      public IResult<TValue> this[TKey key] => hash.Must().HaveKeyOf(key).Try(() => $"Key {key} not found").Map(d => d[key]);
   }
}