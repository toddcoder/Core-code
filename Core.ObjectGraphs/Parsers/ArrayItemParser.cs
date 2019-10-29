using Core.RegularExpressions;
using static Core.Strings.StringFunctions;

namespace Core.ObjectGraphs.Parsers
{
   public class ArrayItemParser : BaseParser
   {
      public override string Pattern => $"^ {{sp}} {REGEX_STEM} /s* {REGEX_VALUE} $";

      public override bool Scan(string[] tokens, int position, Replacer replacer)
      {
         var name = $"${uniqueID()}";
         var value = tokens[1];
         if (!value.IsMatch("'//(' /d+ ')'"))
         {
            value = value.TrimEnd();
         }

         value = replacer.Replace(value);
         value = value.TrimEnd();
         Result = new ObjectGraph(name, value);

         return true;
      }
   }
}