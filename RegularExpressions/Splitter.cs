using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Enumerables;

namespace Core.RegularExpressions
{
	public class Splitter : Matcher
	{
		protected List<string> subPatterns;

		public Splitter(bool friendly = true) : base(friendly) { }

		protected string getPattern(string input, ref string pattern, RegexOptions options)
		{
			var items = input.Split(pattern, options);
			if (pattern.StartsWith("/(") && pattern.EndsWith(")"))
         {
            pattern = "";
         }

         return items.Select(getSubPattern).Stringify(pattern);
		}

		protected string getSubPattern(string item)
		{
			var escaped = $"/(\'{item.Escape()}\')";
			subPatterns.Add(escaped);

			return escaped;
		}

		public override bool IsMatch(string input, string pattern, RegexOptions options)
		{
			subPatterns = new List<string>();
			pattern = getPattern(input, ref pattern, options);

			return base.IsMatch(input, pattern, options);
		}

		public string GetPattern(int index) => subPatterns[index];
	}
}