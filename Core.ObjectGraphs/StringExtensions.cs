using System.Linq;
using Core.RegularExpressions;

namespace Core.ObjectGraphs
{
	public static class StringExtensions
	{
		public static ObjectGraph ObjectGraph(this string name, params string[] childSources)
		{
			var matcher = new Matcher();
			var type = "";
			if (matcher.IsMatch(name, "^ /(-/s+) /s* ':' /s* /@"))
			{
				name = matcher[0, 1];
				type = matcher[0, 2];
			}
			var root = new ObjectGraph(name, type: type);
			foreach (var child in childSources.Select(childSource => (ObjectGraph)childSource))
         {
            root[child.Key] = child;
         }

         return root;
		}

		public static ObjectGraph SubGraph(this string name, params string[] childSources) => name.ObjectGraph(childSources);
	}
}