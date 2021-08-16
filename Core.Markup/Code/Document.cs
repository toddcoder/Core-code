using System.Collections;
using System.Collections.Generic;
using Core.Markup.Code.Blocks;
using Core.Markup.Code.Containers;
using Core.Markup.Code.Extents;

namespace Core.Markup.Code
{
   public class Document : IEnumerable<Container>
   {
      protected List<Container> containers;

      public Document()
      {
         containers = new List<Container>();
      }

      public void Add(Container container) => containers.Add(container);

      public Container CurrentContainer
      {
         get
         {
            if (containers.Count == 0)
            {
               var section = new Section();
               containers.Add(section);
               return section;
            }
            else
            {
               return containers[containers.Count - 1];
            }
         }
      }

      public Block CurrentBlock => CurrentContainer.CurrentBlock;

      public Extent CurrentExtent => CurrentBlock.CurrentExtent;

      public IEnumerator<Container> GetEnumerator() => containers.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
   }
}