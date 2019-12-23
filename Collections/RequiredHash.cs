using Core.Assertions;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;

namespace Core.Collections
{
   public class RequiredHash<TKey, TValue>
   {
      IHash<TKey, TValue> hash;

      internal RequiredHash(IHash<TKey, TValue> hash)
      {
         this.hash = assert(() => hash).Must().Force<IHash<TKey, TValue>>();
      }

      public IResult<TValue> this[TKey key] => assert(() => hash).Must().HaveKeyOf(key).OrFailure().Map(d => d[key]);
   }
}