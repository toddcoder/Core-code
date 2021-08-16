using Core.Matching;
using Core.Monads;
using static Core.Monads.MonadFunctions;

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

      public override Matched<Unit> Parse(ParsingState state) => noMatch<Unit>();
   }
}