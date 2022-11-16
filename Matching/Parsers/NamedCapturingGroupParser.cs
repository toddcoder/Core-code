using Core.Monads;

namespace Core.Matching.Parsers;

public class NamedCapturingGroupParser : BaseParser
{
   public override string Pattern => $@"^\s*/\(({REGEX_BAL_IDENTIFIER})\b";

   public override Maybe<string> Parse(string source, ref int index) => $"(?<{tokens[1]}>";
}