using System.Collections;
using System.Collections.Generic;
using Core.Collections;
using Core.Markup.Code.Blocks;
using Core.Markup.Code.Containers;
using Core.Markup.Code.Extents;

namespace Core.Markup.Code
{
   public class Document : IEnumerable<Container>
   {
      protected List<Container> containers;
      protected StringHash<Format> styles;

      public Document()
      {
         containers = new List<Container>();
         styles = new StringHash<Format>(true);
      }

      public void Add(Container container) => containers.Add(container);

      public void RegisterStyle(string styleName, Format format) => styles[styleName] = format;

      public bool StyleIsRegistered(string styleName) => styles.ContainsKey(styleName);

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