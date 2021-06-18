using Core.Monads;

namespace Core.RegexMatching.Parsers
{
   public class GroupReferenceParser : BaseParser
   {
      public override string Pattern => @"^\s*/(\d+)";

      public override IMaybe<string> Parse(string source, ref int index)
      {
         var digits = tokens[1];
         if (digits.Length > 1)
         {
            digits = $"{{{digits}}}";
         }

         return $@"\{digits}".Some();
      }
   }
}