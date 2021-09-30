using System.Linq;
using System.Text;
using Core.Markup.Code.Extents;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ConversionFunctions;

namespace Core.Markup.Parser
{
   public class ExtentsParser : BaseParser<Extent>
   {
      public bool ForLine { get; set; }

      public override Matched<Extent> Parse(ParsingState state)
      {
         var current = state.Source.Current;
         if (current.StartsWith("***"))
         {
            state.Source.Advance(3);
            return new BoldItalic();
         }
         else if (current.StartsWith("**"))
         {
            state.Source.Advance(2);
            return new Bold();
         }
         else if (current.StartsWith("*"))
         {
            state.Source.Advance(1);
            return new Italic();
         }
         else if (current.StartsWith("["))
         {
            return getStyleOrFormat(state);
         }
         else
         {
            return getText(state);
         }
      }

      protected Matched<Extent> getStyleOrFormat(ParsingState state)
      {
         state.Source.Advance(1);
         var current = state.Source.Current;

         if (current.Matches("^ /(/w [/w '-']*) ']'; f").If(out var result))
         {
            return getStyle(state, result);
         }
         else
         {
            return getFormat(state);
         }
      }

      protected Matched<Extent> getStyle(ParsingState state, MatchResult result)
      {
         var styleName = result.FirstGroup;
         if (state.Formats.ContainsKey(styleName))
         {
            state.Source.Advance(result.Length);
            return new Style(styleName);
         }
         else
         {
            return fail($"Style [{styleName}] hasn't been registered");
         }
      }

      protected Matched<Extent> getFormat(ParsingState state)
      {
         var current = state.Source.Current;
         if (current.Matches("^ /(-[']']+) ']'; f").If(out var result))
         {
            var specification = result.FirstGroup;
            state.Source.Advance(result.Length);

            var delimitedText = DelimitedText.AsFriendlyPattern();
            var bareText = delimitedText.Destringify(specification);
            var specifiers = bareText.Split("/s* ',' /s*; f").Select(s => delimitedText.Restringify(s, RestringifyQuotes.None)).ToArray();
            var format = new Format();

            foreach (var specifier in specifiers)
            {
               if (specifier.Matches("^ /(/w [/w '-']+) /s* '=' /s* /(.+) $; f").If(out result))
               {
                  var (name, value) = result;
                  switch (name)
                  {
                     case "font-name":
                        format.FontName = value;
                        break;
                     case "font-size":
                        format.FontSize = Maybe.Single(value);
                        break;
                     case "bold":
                        format.Bold = Maybe.Boolean(result[0, 1]);
                        break;
                     case "italic":
                        format.Italic = Maybe.Boolean(value);
                        break;
                     default:
                        return fail(specifier);
                  }
               }
               else
               {
                  return fail(specifier);
               }
            }

            return format;
         }
         else
         {
            return getText(state);
         }
      }

      protected Matched<Extent> getText(ParsingState state)
      {
         var current = state.Source.Current;
         var builder = new StringBuilder();
         var escaped = false;
         var length = 0;
         var gathering = true;
         var lineEnding = false;

         for (var i = 0; gathering && i < current.Length; i++)
         {
            var character = current[i];
            length++;
            switch (character)
            {
               case '/' when escaped:
                  builder.Append(character);
                  escaped = false;
                  break;
               case '/':
                  escaped = true;
                  break;
               case '*' or '[' when escaped:
                  builder.Append(character);
                  escaped = false;
                  break;
               case '*' or '[':
                  length--;
                  gathering = false;
                  break;
               case '\r' or '\n' when !lineEnding:
                  lineEnding = true;
                  break;
               case '\r' or '\n':
                  break;
               case '&' or '-' or '%' or '#' or '|' when lineEnding:
                  length--;
                  lineEnding = false;
                  break;
               default:
                  if (ForLine && lineEnding)
                  {
                     gathering = false;
                  }
                  else
                  {
                     if (lineEnding)
                     {
                        builder.Append(" ");
                     }

                     builder.Append(character);
                     lineEnding = false;
                  }

                  break;
            }
         }

         state.Source.Advance(length);
         return new TextExtent(builder.ToString());
      }
   }
}