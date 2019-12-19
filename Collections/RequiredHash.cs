using Core.Assertions;
using Core.Monads;

namespace Core.Collections
{
   public class RequiredHash<TKey, TValue>
   {
      IHash<TKey, TValue> hash;

      internal RequiredHash(IHash<TKey, TValue> hash)
      {
         this.hash = hash.MustAs(nameof(hash)).Ensure<IHash<TKey, TValue>>();
      }

      public IResult<TValue> this[TKey key] => hash.MustAs(nameof(hash)).HaveKeyOf(key).Try().Map(d => d[key]);
   }
}