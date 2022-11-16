using Core.Monads;
using Core.Strings;

namespace Core.Matching.Parsers;

public class RemainderParser : BaseParser
{
   public override string Pattern => @"^\s*(/\s*)?@";

   public override Maybe<string> Parse(string source, ref int index) => tokens[1].IsNotEmpty() ? "(.*)$" : ".*$";
}