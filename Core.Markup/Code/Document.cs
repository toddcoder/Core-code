using System.Collections;
using System.Collections.Generic;
using Core.Markup.Code.Containers;

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

      public IEnumerator<Container> GetEnumerator() => containers.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
   }
}