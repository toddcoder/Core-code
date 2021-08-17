using System;
using Core.Exceptions;
using Core.Markup.Code.Blocks;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Parser
{
   public class BlocksParser : BaseParser<Block>
   {
      protected ExtentsParser extentsParser;

      public BlocksParser()
      {
         extentsParser = new ExtentsParser();
      }

      public override Match<Block> Parse(ParsingState state)
      {
         static Exception failSpecifier(char specifier) => $"Didn't understand specifier '{specifier}'".Fail();

         if (state.Source.PeekNextLine("^ /(['&%#-']); f").If(out var line))
         {
            var specifier = line[0];
            var _block = specifier switch
            {
               '&' => getParagraph(state),
               '-' => getList(state),
               '%' => getOrderedList(state),
               '#' => getHeader(state),
               _ => failSpecifier(specifier)
            };

            if (_block.ValueOrOriginal(out var block, out var asOriginal))
            {
               state.Source.AdvanceLastPeek();
               return block;
            }
            else
            {
               return asOriginal;
            }
         }
         else
         {
            return nil;
         }
      }

      protected Result<Block> getParagraph(ParsingState state)
      {
         var paragraph = new Paragraph();
         while (state.Source.More)
         {
            if (extentsParser.Parse(state).ValueOrCast(out var extent, out Result<Block> asBlock))
            {
               paragraph.Add(extent);
            }
            else
            {
               return asBlock;
            }
         }

         return paragraph;
      }

      protected Result<Block> getLine(ParsingState state)
      {
         var line = new Line();
         while (state.)
         {
            
         }
      }

      protected Result<Block> getOrderedList(ParsingState state)
      {
      }

      protected Result<Block> getHeader(ParsingState state)
      {
      }
   }
}