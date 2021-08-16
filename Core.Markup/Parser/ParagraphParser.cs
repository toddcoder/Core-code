using Core.Matching;
using Core.Monads;

namespace Core.Markup.Parser
{
   public class ParagraphParser : Parser
   {
      protected ExtentParsers extentParsers;

      public ParagraphParser()
      {
         extentParsers = new ExtentParsers();
      }
      public override Pattern Pattern => "^ '&'; f";

      public override Matched<Unit> Parse(ParsingState state)
      {
         return Unit.Value;
      }
   }
}