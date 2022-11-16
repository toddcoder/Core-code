using Core.Monads;
using Core.Strings;

namespace Core.Matching.Parsers;

public class NumericQuantificationParser : BaseParser
{
   public override string Pattern => @"^\s*(\d+)(\s*%\s*)?(\d*)";

   public override Maybe<string> Parse(string source, ref int index)
   {
      var number1 = tokens[1];
      var separator = tokens[2].IsNotEmpty() ? "," : "";
      var number2 = tokens[3];

      return $"{{{number1}{separator}{number2}}}";
   }
}