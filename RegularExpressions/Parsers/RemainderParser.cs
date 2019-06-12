using Core.Monads;
using Core.Strings;

namespace Core.RegularExpressions.Parsers
{
	public class RemainderParser : BaseParser
	{
		public override string Pattern => @"^\s*(/\s*)?@";

	   public override IMaybe<string> Parse(string source, ref int index) => (tokens[1].IsNotEmpty()? "(.*)$" : ".*$").Some();
	}
}