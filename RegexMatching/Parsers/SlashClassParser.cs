using Core.Monads;
using Core.Strings;

namespace Core.RegexMatching.Parsers
{
   public class SlashClassParser : BaseParser
   {
      public override string Pattern => @"^\s*(-\s*)?/([wdsazbtrnWDSAZBG])";

      public override IMaybe<string> Parse(string source, ref int index)
      {
         var letter = tokens[2];
         if (tokens[1].IsNotEmpty())
         {
            letter = letter.ToUpper();
         }

         return $@"\{letter}".Some();
      }
   }
}