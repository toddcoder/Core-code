using Core.Monads;

namespace Core.Matching.Parsers;

public class EndOfClassParser : BaseParser
{
   public override string Pattern => @"^\s*\]";

   public override Optional<string> Parse(string source, ref int index) => "]";
}