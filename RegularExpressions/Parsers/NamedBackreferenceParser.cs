using Core.Monads;

namespace Core.RegularExpressions.Parsers
{
	public class NamedBackreferenceParser : BaseParser
	{
		public override string Pattern => $@"^\s*/<({REGEX_IDENTIFIER})>";

	   public override IMaybe<string> Parse(string source, ref int index) => new Some<string>($@"\k<{tokens[1]}>");
	}
}