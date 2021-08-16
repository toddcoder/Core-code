using System.Collections;
using System.Collections.Generic;
using Core.Enumerables;
using Core.Markup.Code.Extents;
using Core.Monads;

namespace Core.Markup.Code.Blocks
{
   public abstract class Block : IEnumerable<Extent>
   {
      protected List<Extent> extents;

      public Block()
      {
         extents = new List<Extent>();
      }

      public void Add(Extent extent) => extents.Add(extent);

      public IEnumerator<Extent> GetEnumerator() => extents.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public Maybe<Extent> LastExtent() => extents.LastOrNone();
   }
}