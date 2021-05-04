using Core.Monads;

namespace Core.RegularExpressions.Parsers
{
   public class VariableParser : BaseParser
   {
      public override string Pattern => @"^\s*%\(([a-zA-Z][a-zA-Z_0-9-]*)\)";

      public override IMaybe<string> Parse(string source, ref int index) => Matcher.GetVariable(tokens[1]);
   }
}