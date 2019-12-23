using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Assertions;
using Core.RegularExpressions;
using static Core.Assertions.AssertionFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Strings
{
	public class Destringifier
	{
		enum QuoteType
		{
			Single,
			Double
		}

		enum LocationType
		{
			Outside,
			InsideSingle,
			InsideDouble,
			EscapedSingle,
			EscapedDouble,
			PossibleComment,
			Comment,
			PossibleSingleComment,
			SingleComment,
			PossibleEither,
			PossibleEndComment,
			IgnoreEndOfLine
		}

		struct StringItem
		{
			public string String;
			public QuoteType QuoteType;

			public string Restringed(bool withQuotes, char escape)
			{
				if (withQuotes)
            {
               switch (QuoteType)
               {
                  case QuoteType.Single:
                     return $"'{String.Replace("'", escape + "'")}'";
                  case QuoteType.Double:
                     return $"\"{String.Replace("\"", escape + "\"")}\"";
                  default:
                     return String;
               }
            }
            else
            {
               return String;
            }
         }
		}

		string source;
		StringItem[] stringItems;

		public Destringifier(string source)
		{
			this.source = source;
			stringItems = new StringItem[0];
		}

		public char SingleQuote { get; set; } = '\'';

		public char DoubleQuote { get; set; } = '"';

		public char Escape { get; set; } = '\\';

		public string BeginComment { get; set; } = "/*";

		public string EndComment { get; set; } = "*/";

		public string SingleComment { get; set; } = "//";

		public bool IgnoreMultilineComments { get; set; }

		public bool IgnoreSingleComments { get; set; }

		public string Parse()
      {
         assert(() => BeginComment).Must().HaveLengthOf(2).OrThrow();
         assert(() => EndComment).Must().HaveLengthOf(2).OrThrow();
         assert(() => SingleComment).Must().HaveLengthOf(1).OrThrow();

			var outerBuilder = new StringBuilder();
			var innerBuilder = new StringBuilder();
			var items = new List<StringItem>();
			var type = LocationType.Outside;

			var comment1 = BeginComment[0];
			var comment2 = BeginComment[1];
			var endComment1 = EndComment[0];
			var endComment2 = EndComment[1];
			var singleComment1 = SingleComment[0];
			var singleComment2 = maybe(SingleComment.Length > 1, () => SingleComment[1]);
			var possibleEither = comment1 == singleComment1;
			var keepComment = !IgnoreMultilineComments;
			var keepSingleComment = !IgnoreSingleComments;

			foreach (var ch in source)
         {
            switch (type)
            {
               case LocationType.Outside:
                  if (ch == SingleQuote)
                  {
                     type = LocationType.InsideSingle;
                  }
                  else if (ch == DoubleQuote)
                  {
                     type = LocationType.InsideDouble;
                  }
                  else if (ch == comment1 && keepComment)
                  {
                     type = possibleEither ? LocationType.PossibleEither : LocationType.PossibleComment;
                  }
                  else if (ch == singleComment1 && keepSingleComment)
                  {
                     type = singleComment2.IsSome ? LocationType.PossibleSingleComment : LocationType.SingleComment;
                  }
                  else
                  {
                     outerBuilder.Append(ch);
                  }

                  break;
               case LocationType.InsideSingle:
                  if (ch == Escape)
                  {
                     type = LocationType.EscapedSingle;
                  }
                  else if (ch == SingleQuote)
                  {
                     outerBuilder.Append($"/({items.Count})");
                     var item = new StringItem { String = innerBuilder.ToString(), QuoteType = QuoteType.Single };
                     items.Add(item);
                     type = LocationType.Outside;
                     innerBuilder.Clear();
                  }
                  else
                  {
                     innerBuilder.Append(ch);
                  }

                  break;
               case LocationType.InsideDouble:
                  if (ch == Escape)
                  {
                     type = LocationType.EscapedDouble;
                  }
                  else if (ch == DoubleQuote)
                  {
                     outerBuilder.Append($"/({items.Count})");
                     var item = new StringItem { String = innerBuilder.ToString(), QuoteType = QuoteType.Double };
                     items.Add(item);
                     type = LocationType.Outside;
                     innerBuilder.Clear();
                  }
                  else
                  {
                     innerBuilder.Append(ch);
                  }

                  break;
               case LocationType.EscapedSingle:
                  switch (ch)
                  {
                     case 't':
                        innerBuilder.Append('\t');
                        break;
                     case 'r':
                        innerBuilder.Append('\r');
                        break;
                     case 'n':
                        innerBuilder.Append('\n');
                        break;
                     default:
                        innerBuilder.Append(ch);
                        break;
                  }

                  type = LocationType.InsideSingle;
                  break;
               case LocationType.EscapedDouble:
                  switch (ch)
                  {
                     case 't':
                        innerBuilder.Append('\t');
                        break;
                     case 'r':
                        innerBuilder.Append('\r');
                        break;
                     case 'n':
                        innerBuilder.Append('\n');
                        break;
                     default:
                        innerBuilder.Append(ch);
                        break;
                  }

                  type = LocationType.InsideDouble;
                  break;
               case LocationType.PossibleComment:
                  if (ch == comment2)
                  {
                     type = LocationType.Comment;
                  }
                  else
                  {
                     outerBuilder.Append(comment1);
                     type = LocationType.Outside;
                     goto case LocationType.Outside;
                  }

                  break;
               case LocationType.PossibleSingleComment:
                  if (singleComment2.If(out var sc2) && sc2 == ch)
                  {
                     type = LocationType.SingleComment;
                  }
                  else
                  {
                     outerBuilder.Append(singleComment1);
                     type = LocationType.Outside;
                     goto case LocationType.Outside;
                  }

                  break;
               case LocationType.Comment:
                  if (ch == endComment1)
                  {
                     type = LocationType.PossibleEndComment;
                  }

                  break;
               case LocationType.SingleComment:
                  if (ch == '\r' || ch == '\n')
                  {
                     type = LocationType.IgnoreEndOfLine;
                  }

                  break;
               case LocationType.PossibleEither:
                  if (ch == comment2)
                  {
                     type = LocationType.Comment;
                  }
                  else if (singleComment2.If(out sc2) && sc2 == ch)
                  {
                     type = LocationType.SingleComment;
                  }
                  else
                  {
                     outerBuilder.Append(comment1);
                     type = LocationType.Outside;
                     goto case LocationType.Outside;
                  }

                  break;
               case LocationType.PossibleEndComment:
                  type = ch == endComment2 ? LocationType.Outside : LocationType.Comment;
                  break;
               case LocationType.IgnoreEndOfLine:
                  if (ch != '\r' && ch != '\n')
                  {
                     type = LocationType.Outside;
                  }

                  break;
            }
         }

         stringItems = items.ToArray();
			return outerBuilder.ToString();
		}

		public string Restring(string destringified, bool withQuotes)
		{
			var matcher = new Matcher();
			if (matcher.IsMatch(destringified, "'//(' /(/d+) ')'"))
			{
				var maximum = stringItems.Length;
				for (var i = 0; i < matcher.MatchCount; i++)
				{
					var indexAsString = matcher[i, 1];
					var index = indexAsString.AsInt().Required($"Didn't understand {indexAsString} as an integer");
               assert(() => index).Must().BeBetween(0).Until(maximum).OrThrow();
					matcher[i, 0] = stringItems[index].Restringed(withQuotes, Escape);
				}

				return matcher.ToString();
			}
			else
         {
            return destringified;
         }
      }

		public string[] Strings => stringItems.Select(i => i.String).ToArray();

		public string[] QuotedStrings => stringItems.Select(i => i.Restringed(true, Escape)).ToArray();
	}
}