using Core.Monads;

namespace Core.Matching.Parsers;

public class NumericQuantification2Parser : BaseParser
{
   public override string Pattern => @"^\s*%\s*(\d+)";

   public override Maybe<string> Parse(string source, ref int index) => $"{{,{tokens[1]}}}";
}