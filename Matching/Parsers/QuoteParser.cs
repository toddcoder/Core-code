using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Matching.Parsers;

public class QuoteParser : BaseParser
{
   public override string Pattern => @"^\s*`(quote|apos)\b";

   public override Maybe<string> Parse(string source, ref int index) => tokens[1].ToLower() switch
   {
      "quote" => "\"",
      "apos" => "'",
      _ => nil
   };
}