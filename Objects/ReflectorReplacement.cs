using Core.Monads;
using Core.RegularExpressions;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Objects
{
	public class ReflectorReplacement
	{
		int index;
		int length;
		string memberName;
		IMaybe<IFormatter> formatter;

		public ReflectorReplacement(int index, int length, Matcher.Group group)
		{
			this.index = index;
			this.length = length;

			if (group.Text.MatchOne("^ /(/w+) /s* (/['$,:'] /s* /(.*))? $").If(out var match))
			{
				var (mn, prefix, format) = match.Groups3();
				memberName = mn;
				switch (prefix)
				{
					case ",":
					case ":":
						formatter = some<StandardFormatter, IFormatter>(new StandardFormatter(prefix + format));
						break;
					case "$":
						formatter = some<NewFormatter, IFormatter>(new NewFormatter(format));
						break;
					default:
						formatter = none<IFormatter>();
						break;
				}
			}
		}

		public string MemberName => memberName;

		public void Replace(object obj, IGetter getter, Slicer slicer)
		{
			var value = getter.GetValue(obj);
			slicer[index, length] = formatter.FlatMap(f => f.Format(value), () => value.ToNonNullString());
		}
	}
}