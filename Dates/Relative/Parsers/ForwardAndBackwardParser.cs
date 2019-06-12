using Core.Dates.Relative.DateOperations;
using Core.Monads;
using Core.RegularExpressions;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Dates.Relative.Parsers
{
	public class ForwardAndBackwardParser : Parser
	{
		public override string Pattern => "^ /(/d+) /s+ {units} /s+ /('ago' | 'from' /s+ 'now') $";

	   public override IMatched<DateOperation> Parse(string source, string[] tokens)
		{
			var amount = tokens[1].ToInt();
			var unit = tokens[2].ToLower().Substitute("'s' $", "");
			var increment = tokens[3] == "ago" ? -1 : 1;
			amount = amount * increment;

			switch (unit)
			{
				case "month":
					return new RelativeMonth(amount).Matched<DateOperation>();
				case "year":
					return new RelativeYear(amount).Matched<DateOperation>();
				case "day":
					return new RelativeDay(amount).Matched<DateOperation>();
            default:
               return notMatched<DateOperation>();
         }
		}
	}
}