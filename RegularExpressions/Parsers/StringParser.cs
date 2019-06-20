using System.Text;
using Core.Exceptions;
using Core.Monads;

namespace Core.RegularExpressions.Parsers
{
	public class StringParser : BaseParser
	{
		public override string Pattern => @"^\s*(/)?(['""])";

	   public override IMaybe<string> Parse(string source, ref int index)
	   {
	      var enclose = tokens[1] == "/";
			var quote = tokens[2][0];
			var escaped = false;
			var contents = new StringBuilder();

			for (var i = index; i < source.Length; i++)
			{
				var ch = source[i];
				switch (ch)
				{
					case '/':
						if (escaped)
						{
							contents.Append("/");
							escaped = false;
						}
						else
                  {
                     escaped = true;
                  }

                  break;
					case '\'':
						if (quote == '\'')
                  {
                     if (escaped)
                     {
                        contents.Append("'");
                        escaped = false;
                     }
                     else
                     {
                        index = i + 1;
                        return contents.ToString().Escape(false).Enclose(enclose).Some();
                     }
                  }
                  else
                  {
                     contents.Append("'");
                  }

                  break;
					case '"':
						if (quote == '"')
                  {
                     if (escaped)
                     {
                        contents.Append('"');
                        escaped = false;
                     }
                     else
                     {
                        index = i + 1;
                        return contents.ToString().Escape(false).Enclose(enclose).Some();
                     }
                  }
                  else
                  {
                     contents.Append('"');
                  }

                  break;
					case 't':
						if (escaped)
						{
							contents.Append("\t");
							escaped = false;
						}
						else
                  {
                     contents.Append(ch);
                  }

                  break;
					case 'r':
						if (escaped)
						{
							contents.Append("\r");
							escaped = false;
						}
						else
                  {
                     contents.Append(ch);
                  }

                  break;
					case 'n':
						if (escaped)
						{
							contents.Append("\n");
							escaped = false;
						}
						else
                  {
                     contents.Append(ch);
                  }

                  break;
					default:
						contents.Append(ch);
						escaped = false;
						break;
				}
			}

			throw "Open string".Throws();
		}
	}
}