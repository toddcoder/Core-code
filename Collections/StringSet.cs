using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Collections
{
	public class StringSet : Set<string>
	{
		static StringComparison getStringComparison(bool ignoreCase)
		{
			return ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
		}

		protected StringComparison stringComparison;

		public StringSet() { }

		public StringSet(bool ignoreCase) => stringComparison = getStringComparison(ignoreCase);

		public StringSet(IEnumerable<string> strings) : base(strings) { }

		public StringSet(IEnumerable<string> strings, bool ignoreCase) : base(strings)
		{
			stringComparison = getStringComparison(ignoreCase);
		}

		public override bool Contains(string item)
		{
			if (item != null)
         {
            return content.Any(i => string.Compare(item, i, stringComparison) == 0);
         }
         else
         {
            return content.Any(i => i == null);
         }
      }
	}
}