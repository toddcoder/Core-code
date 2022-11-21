using Core.Monads;

namespace Core.Collections;

public interface IHash<TKey, TValue>
{
   TValue this[TKey key] { get; }

   bool ContainsKey(TKey key);

   Result<Hash<TKey, TValue>> AnyHash();
}