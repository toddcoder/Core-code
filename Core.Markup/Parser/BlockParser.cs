using Core.Markup.Code.Blocks;
using Core.Markup.Code.Extents;
using Core.Matching;
using Core.Monads;

namespace Core.Markup.Parser
{
   public class BlockParser : Parser<Block>
   {
      protected ExtentParser extentParser;

      public BlockParser()
      {
         extentParser = new ExtentParser();
      }

      public override Either<Block, string> Parse(string line, MatchResult result) => result.FirstGroup switch
      {
         "&" => getParagraph(result.SecondGroup).Left(),
         _ => "Don't know".Right()
      };

      protected Block getParagraph(string line)
      {
         var paragraph = new Paragraph();
         while (line.Matches("^ /('*'1%3 | '[') ").If(out var result))
         {
            var either = extentParser.Parse(line, result);
            if (either.IfLeft(out var extent))
            {
               switch (extent)
               {
                  case TextExtent
               }
            }
         }
      }
   }
}