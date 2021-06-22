using Core.Monads;

namespace Core.Matching.Parsers
{
   public class OptionGroupParser : BaseParser
   {
      public override string Pattern => @"^\s*\((-?[imnsx]:)";

      public override IMaybe<string> Parse(string source, ref int index) => $"(?{tokens[1]}".Some();
   }
}