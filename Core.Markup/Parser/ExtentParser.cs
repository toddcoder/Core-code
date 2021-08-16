using Core.Matching;
using Core.Monads;

namespace Core.Markup.Parser
{
   public class ExtentParser : Parser
   {
      protected static Parser[] parsers;

      static ExtentParser()
      {
         parsers = new[] { (Parser)new TextExtentParser(), new BoldItalicParser(), new StyleAndFormatParser() };
      }

      public override Pattern Pattern => "^ /(-['\r\n']+); f";

      public override Matched<Unit> Parse(ParsingState state)
      {
         return Unit.Value;
      }
   }
}