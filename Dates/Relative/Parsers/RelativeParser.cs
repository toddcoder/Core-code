using System;
using System.Collections.Generic;
using System.Linq;
using Core.Dates.Relative.DateOperations;
using Core.Monads;
using Core.RegularExpressions;
using Core.Strings;

namespace Core.Dates.Relative.Parsers
{
	public class RelativeParser
	{
		string[] source;

		public RelativeParser(string source) => this.source = source.Split("/s* ',' /s*").Select(s => s.Trim()).ToArray();

		static IEnumerable<Parser> parsers()
		{
			yield return new AbsoluteParser();
			yield return new ForwardAndBackwardParser();
			yield return new SpecialDayParser();
			yield return new SpecialMonthParser();
			yield return new ThisParser();
		}

		public IResult<DateTime> Parse(DateTime date)
		{
			var matcher = new Matcher();
			var currentDate = date.Success();

			var list = new List<DateOperation>();

			foreach (var sourceItem in source)
			{
				matcher.Evaluate(sourceItem, "^ /(.*?) (/s+ 'of' /s+ /(.*?))? (/s+ 'of' /s+ /(.*?))? $");
				for (var i = 0; i < matcher.MatchCount; i++)
				for (var j = 1; j < matcher.GroupCount(i); j++)
				{
					var innerSource = matcher[i, j];
					if (innerSource.IsNotEmpty())
						foreach (var result in parsers()
							.Select(parser => parser.Scan(innerSource))
							.Matches())
						{
							list.Add(result);
							break;
						}
				}
			}

			list.Sort();
			foreach (var dateOperation in list)
				if (currentDate.Out(out var current, out currentDate))
				{
					currentDate = dateOperation.Operate(current);
					if (currentDate.Out(out _, out currentDate)) { }
					else
						return currentDate;
				}
				else
					return currentDate;

			return currentDate;
		}
	}
}