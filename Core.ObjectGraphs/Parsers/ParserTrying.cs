using Core.Monads;
using static Core.Monads.AttemptFunctions;

namespace Core.ObjectGraphs.Parsers
{
   public class ParserTrying
   {
      protected Parser parser;

      internal ParserTrying(Parser parser) => this.parser = parser;

      public IResult<ObjectGraph> Parse(string configs = Parser.DEFAULT_CONFIGS) => tryTo(() => parser.Parse(configs));
   }
}