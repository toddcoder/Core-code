using Core.Monads;

namespace Core.Matching.Parsers;

public class NamedBackReferenceParser : BaseParser
{
   public override string Pattern => $@"^\s*/<({REGEX_IDENTIFIER})>";

   public override Maybe<string> Parse(string source, ref int index) => $@"\k<{tokens[1]}>";
}