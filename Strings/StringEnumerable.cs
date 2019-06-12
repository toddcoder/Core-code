using System.Collections;
using System.Collections.Generic;

namespace Core.Strings
{
	public class StringEnumerable : IEnumerable<string>
	{
		string source;

		internal StringEnumerable(string source) => this.source = source;

	   public IEnumerator<string> GetEnumerator() => new StringEnumerator(source);

	   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}