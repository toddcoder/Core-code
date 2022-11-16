using Core.Monads;

namespace Core.Matching.Parsers;

public class UnmodifiedParser : BaseParser
{
   public override string Pattern => @"^\s*([*?.+,|^$){}0-9])";

   public override Maybe<string> Parse(string source, ref int index) => tokens[1];
}