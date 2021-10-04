using System.Collections.Generic;

namespace Core.Collections
{
   public interface IEnumerableHash<TKey, TValue> : IHash<TKey, TValue>
   {
      IEnumerable<(TKey, TValue)> Enumerable();
   }
}