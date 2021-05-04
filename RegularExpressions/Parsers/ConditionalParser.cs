using Core.Monads;

namespace Core.RegularExpressions.Parsers
{
   public class ConditionalParser : BaseParser
   {
      public override string Pattern => @"^\s*\(\?";

      public override IMaybe<string> Parse(string source, ref int index) => "(?".Some();
   }
}