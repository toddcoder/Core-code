using Core.Monads;

namespace Core.RegularExpressions.Parsers
{
   public class NumericQuantification2Parser : BaseParser
   {
      public override string Pattern => @"^\s*%\s*(\d+)";

      public override IMaybe<string> Parse(string source, ref int index) => ("{," + tokens[1] + "}").Some();
   }
}