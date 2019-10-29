namespace Core.ObjectGraphs.Parsers
{
   public class ReplacementParser : BaseParser
   {
      public override string Pattern => $"^ {{sp}} {REGEX_NAME}/s* '=' /s* {REGEX_VALUE}";

      public override bool Scan(string[] tokens, int position, Replacer replacer)
      {
         var variable = replacer.Replace(tokens[1]);
         var value = replacer.Replace(tokens[2]);
         replacer[variable] = value;

         return true;
      }
   }
}