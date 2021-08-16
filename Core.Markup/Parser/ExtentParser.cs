using Core.Matching;
using Core.Monads;

namespace Core.Markup.Parser
{
   public class ExtentParser : Parser
   {
      protected static Parser[] parsers;

      static ExtentParser()
      {
         parsers=new []{new }
      }

      public override Pattern Pattern => "^ /(-['\r\n']+); f";

      public override Matched<Unit> Parse(ParsingState state)
      {
      }
   }
}