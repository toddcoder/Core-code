using Core.Markup.Code;
using Core.Markup.Code.Containers;
using Core.Matching;

namespace Core.Markup.Parser
{
   public class ParsingState
   {
      protected Source source;
      protected Document document;
      protected MatchResult result;

      public ParsingState(string source)
      {
         this.source = new Source(source);

         document = new Document();
         result = MatchResult.Empty;
      }

      public Source Source => source;

      public Document Document => document;

      public MatchResult Result => result;

      public bool IsBold { get; set; }

      public bool IsItalic { get; set; }

      public void Add(Container container) => document.Add(container);
   }
}