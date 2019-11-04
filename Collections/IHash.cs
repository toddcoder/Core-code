using Core.Monads;

namespace Core.Collections
{
   public interface IHash<TKey, TValue>
   {
      TValue this[TKey key] { get; }

      bool ContainsKey(TKey key);

      IResult<Hash<TKey, TValue>> AnyHash();
   }
}