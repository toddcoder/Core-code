using System;
using System.Linq;
using System.Text;
using Core.Exceptions;
using Core.Markup.Code.Extents;
using Core.Matching;
using Core.Monads;
using Core.Strings;

namespace Core.Markup.Parser
{
   public class ExtentsParser : BaseParser<Extent>
   {
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

      protected static Matched<Extent> getStyleOrFormat(ParsingState state)
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

      protected static Matched<Extent> getStyle(ParsingState state, MatchResult result)
      {
         var styleName = result.FirstGroup;
         if (state.Formats.ContainsKey(styleName))
         {
            state.Source.Advance(result.Length);
            return new Style(styleName);
         }
         else
         {
            return $"Style [{styleName}] hasn't been registered".Fail();
         }
      }

      protected static Matched<Extent> getFormat(ParsingState state)
      {
         static ApplicationException fail(string specifier) => $"Specifier {specifier} not recognized".Fail();

         var current = state.Source.Current;
         if (current.Matches("^ /(-[']']+) ']'").If(out var result))
         {
            var specification = result.FirstGroup;
            state.Source.Advance(result.Length);

            var delimitedText = DelimitedText.AsFriendlyPattern();
            var bareText = delimitedText.Destringify(specification);
            var specifiers = bareText.Split("/s* ',' /s*").Select(s => delimitedText.Restringify(s, RestringifyQuotes.None)).ToArray();
            var format = new Format();

            foreach (var specifier in specifiers)
            {
               if (specifier.Matches("^ /(/w [/w '-']+) /s* '=' /s* /(.+) $").If(out result))
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

      protected static Matched<Extent> getText(ParsingState state)
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
                  gathering = false;
                  break;
               default:
                  builder.Append(character);
                  lineEnding = false;
                  break;
            }
         }

         state.Source.Advance(length);
         return new TextExtent(builder.ToString());
      }
   }
}