using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Matching.Parsers;

public class LookAroundParser : BaseParser
{
   public override string Pattern => @"^\s*(-)?\(([<>])";

   public override Maybe<string> Parse(string source, ref int index)
   {
      var negative = tokens[1] == "-";
      var type = tokens[2];

      return type switch
      {
         ">" => negative ? "(?!" : "(?=",
         "<" => negative ? "(?<!" : "(?<=",
         _ => nil
      };
   }
}