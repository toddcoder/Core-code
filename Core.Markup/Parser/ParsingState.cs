using Core.Matching;

namespace Core.Markup.Parser
{
   public class ParsingState
   {
      protected Source source;

      public ParsingState(string source)
      {
         this.source = new Source(source);
      }

      public Source Source => source;
   }
}