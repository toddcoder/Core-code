using Core.Markup.Code.Blocks;
using Core.Monads;

namespace Core.Markup.Parser
{
   public class BlocksParser : BaseParser<Block>
   {
      protected ExtentsParser extentsParser;

      public BlocksParser()
      {
         extentsParser = new ExtentsParser();
      }

      public override Matched<Block> Parse(ParsingState state)
      {
         return state.Source.Current.StartsWith("&") ? getParagraph(state) : getLine(state);
      }

      protected Matched<Block> getParagraph(ParsingState state)
      {
         state.Source.Advance(1);
         var paragraph = new Paragraph();
         extentsParser.ForLine = false;

         while (true)
         {
            if (extentsParser.Parse(state).If(out var extent, out var _exception))
            {
               paragraph.Add(extent);
            }
            else if (_exception.If(out var exception))
            {
               return exception;
            }
            else
            {
               return paragraph;
            }
         }
      }

      protected Matched<Block> getLine(ParsingState state)
      {
         state.Source.Advance(1);
         var line = new Line();
         extentsParser.ForLine = true;

         while (true)
         {
            if (extentsParser.Parse(state).If(out var extent, out var _exception))
            {
               line.Add(extent);
            }
            else if (_exception.If(out var exception))
            {
               return exception;
            }
            else
            {
               return line;
            }
         }
      }
   }
}