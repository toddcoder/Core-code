using Core.Matching;
using Core.Monads;

namespace Core.Markup.Parser
{
   public class FormatParser : Parser
   {
      public override Pattern Pattern => "^ /'[' /([']']+) ']'; f";

      public override Matched<Unit> Parse(ParsingState state)
      {
         var specification = state.Result.FirstGroup;
         return Unit.Value;
      }
   }
}