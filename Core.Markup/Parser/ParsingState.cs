using Core.Markup.Code;
using Core.Markup.Code.Containers;
using Core.Matching;

namespace Core.Markup.Parser
{
   public class ParsingState
   {
      protected Source source;
      protected Document document;

      public ParsingState(string source)
      {
         this.source = new Source(source);

         document = new Document();
      }

      public Source Source => source;

      public void Add(Container container) => document.Add(container);
   }
}