using System.Linq;
using Core.Markup.Code.Extents;
using Core.Matching;
using Core.Monads;
using Core.Strings;

namespace Core.Markup.Parser
{
   public class StyleAndFormatParser : Parser
   {
      public override Either<int, Maybe<string>> Parse(string line, ParsingState state)
      {
         if (line.Matches("^ /'[' /([']']+) ']'; f").If(out var result))
         {
            var specification = state.Result.FirstGroup;
            if (specification.IsMatch("^ [/w '-']+ $; f"))
            {
               if (state.Document.StyleIsRegistered(specification))
               {
                  var style = new Style(specification);
                  state.Document.CurrentBlock.Add(style);
                  state.Source.Advance(specification.Length + 2);

                  return (specification.Length + 2).Left();
               }
               else
               {
                  return $"Style [{specification}] isn't registered".Some().Right();
               }
            }
            else
            {
               var delimitedText = DelimitedText.BothQuotes();
               var destringifiedText = delimitedText.Destringify(specification);
               var specifiers = destringifiedText.Split("/s* ',' /s*; f").Select(s => delimitedText.Restringify(s, RestringifyQuotes.None)).ToArray();
               var format = new Format();
               foreach (var specifier in specifiers)
               {
                  if (specifier.Matches("^ /('font-' ('name' | 'size') | 'bold' | 'italic') /s* '=' /s* /(.+) $; fi").If(out result))
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
                     return (specification.Length + 2).Left();
                  }
               }
            }

            return $"Didn't understand specification [{specification}]".FailedMatch<Unit>();
         }
      }

      public override Matched<Unit> Parse(ParsingState state)
      {
         var specification = state.Result.FirstGroup;
         if (specification.IsMatch("^ [/w '-']+ $; f"))
         {
            if (state.Document.StyleIsRegistered(specification))
            {
               var style = new Style(specification);
               state.Document.CurrentBlock.Add(style);
               state.Source.Advance(specification.Length + 2);

               return Unit.Value;
            }
            else
            {
               return $"Style [{specification}] isn't registered".FailedMatch<Unit>();
            }
         }
         else
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
                  state.Source.Advance(specification.Length + 2);
                  return Unit.Value;
               }
            }
         }

         return $"Didn't understand specification [{specification}]".FailedMatch<Unit>();
      }
   }
}