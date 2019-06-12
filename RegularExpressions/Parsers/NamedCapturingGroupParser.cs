using Core.Monads;

namespace Core.RegularExpressions.Parsers
{
	public class NamedCapturingGroupParser : BaseParser
	{
		public override string Pattern => $@"^\s*/\(({REGEX_BAL_IDENTIFIER})\b";

	   public override IMaybe<string> Parse(string source, ref int index) => new Some<string>($"(?<{tokens[1]}>");
	}
}