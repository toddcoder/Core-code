using Core.Monads;

namespace Core.RegularExpressions.Parsers
{
	public class UnmodifiedParser : BaseParser
	{
		public override string Pattern => @"^\s*([*?.+,|^$){}0-9])";

	   public override IMaybe<string> Parse(string source, ref int index) => tokens[1].Some();
	}
}