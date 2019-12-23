using Core.Assertions;
using Core.RegularExpressions;
using static Core.Assertions.AssertionFunctions;

namespace Core.ObjectGraphs.Parsers
{
   public class MacroParser : BaseParser
   {
      public override string Pattern => $"^ {{sp}} {REGEX_NAME}'(' /(-[')']*) ')' /s* '=' /s*{REGEX_VALUE}";

      public override bool Scan(string[] tokens, int position, Replacer replacer)
      {
         var macroName = replacer.Replace(tokens[1]);
         var parameterSource = replacer.Replace(tokens[2]);
         var value = replacer.Replace(tokens[3]);
         var parameters = parameterSource.Split("/s* ',' /s*");
         var macro = new Macro(macroName, value);
         foreach (var parameter in parameters)
         {
            assert(() => parameter).Must().MatchFriendly(REGEX_NAME)
               .OrThrow(() => AssertionMessage(Source, position, "$name isn't a properly formed name"));
            macro.AddParameter(parameter);
         }

         replacer.AddMacro(macro);
         return true;
      }
   }
}