using Core.Dates.Relative.DateOperations;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Dates.Relative.Parsers
{
	public class AbsoluteParser : Parser
	{
		public override string Pattern => "^ ('the' /s+)? {unit} /s+ /(/d+) $";

	   public override IMatched<DateOperation> Parse(string source, string[] tokens)
		{
			var unit = tokens[1];
			var amount = tokens[2].ToInt();

			switch (unit.ToLower())
			{
				case "month":
					return new AbsoluteMonth(amount).Matched<DateOperation>();
				case "day":
					return new AbsoluteDay(amount).Matched<DateOperation>();
				case "year":
					return new AbsoluteYear(amount).Matched<DateOperation>();
				default:
				   return notMatched<DateOperation>();
			}
		}
	}
}