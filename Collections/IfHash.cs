using Core.Assertions;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Collections;

public class IfHash<TKey, TValue>
{
   protected IHash<TKey, TValue> hash;

   internal IfHash(IHash<TKey, TValue> hash) => this.hash = hash.Must().Not.BeNull().Force<IHash<TKey, TValue>>();

   public Maybe<TValue> this[TKey key] => maybe(hash.ContainsKey(key), () => hash[key]);
}