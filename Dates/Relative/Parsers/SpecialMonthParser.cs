using Core.Dates.Relative.DateOperations;
using Core.Monads;
using Core.Strings;

namespace Core.Dates.Relative.Parsers
{
	public class SpecialMonthParser : Parser
	{
		public override string Pattern => "^ /('jan' ('uary')? | 'feb' ('ruary')? | 'mar' ('ch')? | 'apr' ('ril')? " +
			"| 'may' | 'june'? | 'july'? | 'aug' ('ust')? | 'sep' ('tember')? | 'oct' ('ober')? | 'nov' ('ember')? | " +
			"'dec' ('ember')?) $";

		public override IMatched<DateOperation> Parse(string source, string[] tokens)
      {
         return tokens[1].Keep(3).MonthNumber().Match().Map(i => (DateOperation)new AbsoluteMonth(i));
      }
   }
}