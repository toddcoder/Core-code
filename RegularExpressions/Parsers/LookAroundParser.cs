using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.RegularExpressions.Parsers
{
   public class LookAroundParser : BaseParser
   {
      public override string Pattern => @"^\s*(-)?\(([<>])";

      public override IMaybe<string> Parse(string source, ref int index)
      {
         var negative = tokens[1] == "-";
         var type = tokens[2];

         return type switch
         {
            ">" => (negative ? "(?!" : "(?=").Some(),
            "<" => (negative ? "(?<!" : "(?<=").Some(),
            _ => none<string>()
         };
      }
   }
}