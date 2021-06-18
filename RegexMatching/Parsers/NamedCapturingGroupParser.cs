using Core.Monads;

namespace Core.RegexMatching.Parsers
{
   public class NamedCapturingGroupParser : BaseParser
   {
      public override string Pattern => $@"^\s*/\(({REGEX_BAL_IDENTIFIER})\b";

      public override IMaybe<string> Parse(string source, ref int index) => $"(?<{tokens[1]}>".Some();
   }
}