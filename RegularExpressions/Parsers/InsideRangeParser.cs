using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.RegularExpressions.Parsers
{
   public class InsideRangeParser : BaseParser
   {
      public override string Pattern => $@"^\s*/({REGEX_IDENTIFIER})\b";

      public override IMaybe<string> Parse(string source, ref int index)
      {
         var word = tokens[1];
         var result = OutsideRangeParser.GetRange(word);

         return maybe(result.IsNotEmpty(), () => result);
      }
   }
}