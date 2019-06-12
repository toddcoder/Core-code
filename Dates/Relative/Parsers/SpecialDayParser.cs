using Core.Dates.Relative.DateOperations;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Dates.Relative.Parsers
{
	public class SpecialDayParser : Parser
	{
		public override string Pattern => "^ ('day' /s+ /('before' | 'after') /s+)? /('tomorrow' | 'yesterday') $";

	   public override IMatched<DateOperation> Parse(string source, string[] tokens)
		{
			var relation = tokens[2].ToLower().NormalizeWhitespace();
			if (tokens[1].IsNotEmpty())
			{
				var before = tokens[1] == "before";
				switch (relation)
				{
					case "tomorrow":
						return new RelativeDay(before ? 0 : 2).Matched<DateOperation>();
					case "yesterday":
						return new RelativeDay(before? -2 : 0).Matched<DateOperation>();
					default:
						return notMatched<DateOperation>();
				}
			}
			switch (relation)
			{
				case "tomorrow":
					return new RelativeDay(1).Matched<DateOperation>();
				case "yesterday":
					return new RelativeDay(-1).Matched<DateOperation>();
				default:
					return notMatched<DateOperation>();
			}
		}
	}
}