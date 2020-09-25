using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Assertions;
using Core.Monads;
using Core.RegularExpressions;
using static Core.Assertions.AssertionFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Strings
{
   [Obsolete("Use DelimitedText")]
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
         public bool SqlStyle;

         public string Restringed(bool withQuotes, char escape)
         {
            if (withQuotes)
            {
               switch (QuoteType)
               {
                  case QuoteType.Single when SqlStyle:
                     return $"'{String.Replace("'", "''")}'";
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

      public static Destringifier AsSql(string source)
      {
         var _quoteIndex = none<int>();
         var builder = new StringBuilder();
         for (var i = 0; i < source.Length; i++)
         {
            var ch = source[i];
            if (ch == '\'')
            {
               if (_quoteIndex.If(out var quoteIndex))
               {
                  if (source.Drop(i + 1).Keep(1) == "'" && quoteIndex < i - 1)
                  {
                     builder.Append(@"\");
                     builder.Append(ch);
                     i++;
                  }
                  else if (quoteIndex == i - 1)
                  {
                     builder.Append(ch);
                     _quoteIndex = none<int>();
                  }
                  else
                  {
                     builder.Append(ch);
                     _quoteIndex = none<int>();
                  }
               }
               else
               {
                  builder.Append(ch);
                  _quoteIndex = i.Some();
               }
            }
            else
            {
               builder.Append(ch);
            }
         }

         return new Destringifier(builder.ToString(), true) { SingleComment = "--" };
      }

      string source;
      bool sqlStyle;
      StringItem[] stringItems;

      public Destringifier(string source, bool sqlStyle = false)
      {
         this.source = source;
         this.sqlStyle = sqlStyle;
         stringItems = new StringItem[0];
         SingleQuote = '\'';
         DoubleQuote = '"';
         Escape = '\\';
         BeginComment = "/*";
         EndComment = "*/";
         SingleComment = "//";
      }

      public char SingleQuote { get; set; }

      public char DoubleQuote { get; set; }

      public char Escape { get; set; }

      public string BeginComment { get; set; }

      public string EndComment { get; set; }

      public string SingleComment { get; set; }

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
         var _singleComment2 = maybe(SingleComment.Length > 1, () => SingleComment[1]);
         var possibleEither = comment1 == singleComment1;
         var keepComment = !IgnoreMultilineComments;
         var keepSingleComment = !IgnoreSingleComments;

         foreach (var ch in source)
         {
            switch (type)
            {
               case LocationType.Outside:
               {
                  if (ch == SingleQuote)
                  {
                     type = LocationType.InsideSingle;
                  }
                  else if (ch == DoubleQuote)
                  {
                     type = LocationType.InsideDouble;
                  }
                  else if (ch == comment1)
                  {
                     type = possibleEither ? LocationType.PossibleEither : LocationType.PossibleComment;
                     if (keepComment)
                     {
                        outerBuilder.Append(ch);
                     }
                  }
                  else if (ch == singleComment1)
                  {
                     type = _singleComment2.Map(_ => LocationType.PossibleSingleComment).DefaultTo(() => LocationType.SingleComment);
                     if (keepSingleComment)
                     {
                        outerBuilder.Append(ch);
                     }
                  }
                  else
                  {
                     outerBuilder.Append(ch);
                  }

                  break;
               }
               case LocationType.InsideSingle:
               {
                  if (ch == Escape)
                  {
                     type = LocationType.EscapedSingle;
                  }
                  else if (ch == SingleQuote)
                  {
                     outerBuilder.Append($"/({items.Count})");
                     var item = new StringItem { String = innerBuilder.ToString(), QuoteType = QuoteType.Single, SqlStyle = sqlStyle };
                     items.Add(item);
                     type = LocationType.Outside;
                     innerBuilder.Clear();
                  }
                  else
                  {
                     innerBuilder.Append(ch);
                  }

                  break;
               }
               case LocationType.InsideDouble:
               {
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
               }
               case LocationType.EscapedSingle:
               {
                  switch (ch)
                  {
                     case 't':
                     {
                        innerBuilder.Append('\t');
                        break;
                     }
                     case 'r':
                     {
                        innerBuilder.Append('\r');
                        break;
                     }
                     case 'n':
                     {
                        innerBuilder.Append('\n');
                        break;
                     }
                     default:
                     {
                        innerBuilder.Append(ch);
                        break;
                     }
                  }

                  type = LocationType.InsideSingle;
                  break;
               }
               case LocationType.EscapedDouble:
               {
                  switch (ch)
                  {
                     case 't':
                     {
                        innerBuilder.Append('\t');
                        break;
                     }
                     case 'r':
                     {
                        innerBuilder.Append('\r');
                        break;
                     }
                     case 'n':
                     {
                        innerBuilder.Append('\n');
                        break;
                     }
                     default:
                     {
                        innerBuilder.Append(ch);
                        break;
                     }
                  }

                  type = LocationType.InsideDouble;
                  break;
               }
               case LocationType.PossibleComment:
               {
                  if (ch == comment2)
                  {
                     type = LocationType.Comment;
                     if (keepComment)
                     {
                        outerBuilder.Append(ch);
                     }
                  }
                  else
                  {
                     outerBuilder.Append(comment1);
                     type = LocationType.Outside;
                     goto case LocationType.Outside;
                  }

                  break;
               }
               case LocationType.PossibleSingleComment:
               {
                  if (_singleComment2.If(out var singleComment2) && singleComment2 == ch)
                  {
                     type = LocationType.SingleComment;
                     if (keepSingleComment)
                     {
                        outerBuilder.Append(ch);
                     }
                  }
                  else
                  {
                     type = LocationType.Outside;
                     goto case LocationType.Outside;
                  }

                  break;
               }
               case LocationType.Comment:
               {
                  if (ch == endComment1)
                  {
                     type = LocationType.PossibleEndComment;
                  }

                  if (keepComment)
                  {
                     outerBuilder.Append(ch);
                  }

                  break;
               }
               case LocationType.SingleComment:
               {
                  if (ch == '\r' || ch == '\n')
                  {
                     type = LocationType.IgnoreEndOfLine;
                  }

                  if (keepSingleComment)
                  {
                     outerBuilder.Append(ch);
                  }

                  break;
               }
               case LocationType.PossibleEither:
               {
                  if (ch == comment2)
                  {
                     type = LocationType.Comment;
                     if (keepComment)
                     {
                        outerBuilder.Append(ch);
                     }
                  }
                  else if (_singleComment2.If(out var singleComment2) && singleComment2 == ch)
                  {
                     type = LocationType.SingleComment;
                     if (keepSingleComment)
                     {
                        outerBuilder.Append(ch);
                     }
                  }
                  else
                  {
                     outerBuilder.Append(comment1);
                     type = LocationType.Outside;
                     goto case LocationType.Outside;
                  }

                  break;
               }
               case LocationType.PossibleEndComment:
               {
                  type = ch == endComment2 ? LocationType.Outside : LocationType.Comment;
                  if (keepComment)
                  {
                     outerBuilder.Append(ch);
                  }
                  break;
               }
               case LocationType.IgnoreEndOfLine:
               {
                  if (ch != '\r' && ch != '\n')
                  {
                     type = LocationType.Outside;
                     goto case LocationType.Outside;
                  }
                  else if (keepSingleComment)
                  {
                     outerBuilder.Append(ch);
                  }

                  break;
               }
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