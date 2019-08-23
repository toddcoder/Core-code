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
         switch (type)
         {
            case ">":
               return (negative ? "(?!" : "(?=").Some();
            case "<":
               return (negative ? "(?<!" : "(?<=").Some();
            default:
               return none<string>();
         }
      }
   }
}