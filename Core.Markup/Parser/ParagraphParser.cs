using Core.Matching;
using Core.Monads;

namespace Core.Markup.Parser
{
   public class ParagraphParser : Parser
   {
      protected string paragraphText;

      public ParagraphParser()
      {
         paragraphText = string.Empty;
      }

      public override Pattern Pattern => "^&;u";

      public override Result<int> Parse(ParsingState state)
      {
         return 0;
      }
   }
}