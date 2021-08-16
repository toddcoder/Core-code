using Core.Monads;

namespace Core.Markup.Parser
{
   public class ExtentParsers
   {
      protected TextExtentParser textExtentParser;
      protected StyleAndFormatParser styleAndFormatParser;
      protected BoldItalicParser boldItalicParser;

      public ExtentParsers()
      {
         textExtentParser = new TextExtentParser();
         styleAndFormatParser = new StyleAndFormatParser();
         boldItalicParser = new BoldItalicParser();
      }

      public Matched<Unit> Parse(ParsingState state)
      {
         var anyFound = true;

         while (state.Source.NextLine().If(out var line))
         {
            while (anyFound)
            {
               if (textExtentParser.Parse(state).If(out _, out var _exception))
               {
                  anyFound = true;
                  continue;
               }
               else if (_exception.If(out var exception))
               {
                  return exception;
               }

               if (styleAndFormatParser.Parse(state).If(out _, out _exception))
               {
                  anyFound = true;
                  continue;
               }
               else if (_exception.If(out var exception))
               {
                  return exception;
               }

               if (boldItalicParser.Parse(state).If(out _, out _exception))
               {
                  anyFound = true;
               }
               else if (_exception.If(out var exception))
               {
                  return exception;
               }
            }
         }
      }
   }
}