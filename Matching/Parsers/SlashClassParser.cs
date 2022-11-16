using Core.Monads;
using Core.Strings;

namespace Core.Matching.Parsers;

public class SlashClassParser : BaseParser
{
   public override string Pattern => @"^\s*(-\s*)?/([wdsazbtrnWDSAZBG])";

   public override Maybe<string> Parse(string source, ref int index)
   {
      var letter = tokens[2];
      if (tokens[1].IsNotEmpty())
      {
         letter = letter.ToUpper();
      }

      return $@"\{letter}";
   }
}