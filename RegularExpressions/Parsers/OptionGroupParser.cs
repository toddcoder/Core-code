﻿using Core.Monads;

namespace Core.RegularExpressions.Parsers
{
	public class OptionGroupParser : BaseParser
	{
		public override string Pattern => @"^\s*\((-?[imnsx]:)";

	   public override IMaybe<string> Parse(string source, ref int index) => new Some<string>($"(?{tokens[1]}");
	}
}