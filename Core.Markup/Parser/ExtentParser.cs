using System.Linq;
using System.Text;
using Core.Markup.Code.Extents;
using Core.Matching;
using Core.Monads;
using Core.Strings;

namespace Core.Markup.Parser
{
   public class ExtentParser : Parser<Extent>
   {
      public override Either<Extent, string> Parse(string line, MatchResult result) => result.FirstGroup switch
      {
         "*" => new Italic().Left<Extent>(),
         "**" => new Bold().Left<Extent>(),
         "***" => new BoldItalic().Left<Extent>(),
         "[" when result.SecondGroup.IsMatch("^ /w [/w '-']* $") => getStyle(result.SecondGroup).Left(),
         "[" => getFormat(result.SecondGroup),
         _ => getTextExtent(line).Left()
      };

      protected static Extent getStyle(string styleName) => new Style(styleName);

      protected static Either<Extent, string> getFormat(string specification)
      {
         var delimitedText = DelimitedText.BothQuotes();
         var destringifiedText = delimitedText.Destringify(specification);
         var specifiers = destringifiedText.Split("/s* ',' /s*; f").Select(s => delimitedText.Restringify(s, RestringifyQuotes.None)).ToArray();
         var format = new Format();
         foreach (var specifier in specifiers)
         {
            if (specifier.Matches("^ /('font-' ('name' | 'size') | 'bold' | 'italic') /s* '=' /s* /(.+) $; fi").If(out var result))
            {
               var (name, value) = result;
               switch (name)
               {
                  case "font-name":
                     format.FontName = value;
                     break;
                  case "font-size":
                     format.FontSize = value.AsFloat();
                     break;
                  case "bold":
                     format.Bold = value.AsBool();
                     break;
                  case "italic":
                     format.Italic = value.AsBool();
                     break;
               }

               return format.Left<Extent>();
            }
         }

         return $"Didn't understand specification [{specification}]".Right();
      }

      protected Extent getTextExtent(string line)
      {
         var escaped = false;
         var builder = new StringBuilder();
         var parsing = true;
         var lineEnding = false;

         for (var i = 0; parsing && i < line.Length; i++)
         {
            var ch = line[i];
            switch (ch)
            {
               case '/':
                  if (escaped)
                  {
                     builder.Append(ch);
                     escaped = false;
                  }
                  else
                  {
                     escaped = true;
                  }

                  break;
               case '*' or '[':
                  if (escaped)
                  {
                     builder.Append(ch);
                     escaped = false;
                  }
                  else
                  {
                     parsing = false;
                  }

                  break;
               case '\r' or '\n':
                  if (!lineEnding)
                  {
                     lineEnding = true;
                  }

                  break;
               default:
                  if (lineEnding)
                  {
                     parsing = false;
                  }
                  else
                  {
                     builder.Append(ch);
                     escaped = false;
                  }

                  break;
            }
         }

         return new TextExtent(builder.ToString());
      }
   }
}