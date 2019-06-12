namespace Core.Collections
{
   public interface IHash<in TKey, out TValue>
   {
      TValue this[TKey key] { get; }

      bool ContainsKey(TKey key);
   }
}