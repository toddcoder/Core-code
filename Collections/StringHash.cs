using Core.RegularExpressions;
using Core.Strings;

namespace Core.Collections
{
	public class StringHash : Hash<string, string>
	{
		public static implicit operator StringHash(string keyValues)
		{
			var destringifier = new Destringifier(keyValues);
			var parsed = destringifier.Parse();
			var hash = new StringHash();

			foreach (var keyValue in parsed.Split("/s* ',' /s*"))
			{
				var elements = keyValue.Split("/s* '->' /s*");
				if (elements.Length == 2)
				{
					var key = destringifier.Restring(elements[0], false);
					var value = destringifier.Restring(elements[1], false);
				   hash[key] = value;
				}
			}

			return hash;
		}
	}
}