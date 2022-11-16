using Core.Monads;

namespace Core.Matching.Parsers;

public class OverrideParser : BaseParser
{
   public override string Pattern => @"^\s*/(0[0-8]{2,3}|x[0-9a-f]{2}|c[a-z]{1}|u[0-9a-f]{4}|[pP]{[^}]+})";

   public override Maybe<string> Parse(string source, ref int index) => $@"\{tokens[1]}";
}