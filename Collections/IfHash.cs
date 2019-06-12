using Core.Monads;
using Core.Objects;
using static Core.Booleans.Assertions;
using static Core.Monads.MonadFunctions;

namespace Core.Collections
{
   public class IfHash<TKey, TValue>
   {
      IHash<TKey, TValue> hash;

      internal IfHash(IHash<TKey, TValue> hash) => this.hash = Ensure(hash, h => h.IsNotNull(), "Hash can't be null");

      public IMaybe<TValue> this[TKey key] => when(hash.ContainsKey(key), () => hash[key]);
   }
}