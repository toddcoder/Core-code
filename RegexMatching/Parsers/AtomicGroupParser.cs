using Core.Monads;

namespace Core.RegexMatching.Parsers
{
   public class AtomicGroupParser : BaseParser
   {
      public override string Pattern => @"^\s*\(\+";

      public override IMaybe<string> Parse(string source, ref int index) => "(?>".Some();
   }
}