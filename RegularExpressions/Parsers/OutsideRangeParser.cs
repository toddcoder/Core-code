using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.RegularExpressions.Parsers
{
   public class OutsideRangeParser : BaseParser
   {
      public static string GetRange(string word)
      {
         var result = "";

         switch (word)
         {
            case "alphanum":
            case "an":
               result = "A-Za-z0-9";
               break;
            case "alpha":
            case "al":
               result = "A-Za-z";
               break;
            case "digit":
               result = "0-9";
               break;
            case "uppercase":
            case "u":
               result = "A-Z";
               break;
            case "lowercase":
            case "l":
               result = "a-z";
               break;
            case "hex":
            case "h":
               result = "0-9A-Fa-f";
               break;
            case "xml0":
            case "x0":
               result = "_:A-Za-z";
               break;
            case "xml":
            case "x":
               result = "-._:A-Za-z0-9";
               break;
            case "crlf":
               result = @"\r\n";
               break;
            case "cr":
               result = @"\r";
               break;
            case "lf":
               result = @"\n";
               break;
            case "punct":
            case "p":
               result = @"!@#$%^&*()_+={}[]:;""'|\,.<>/?-";
               break;
            case "squote":
            case "sq":
               result = "'";
               break;
            case "dquote":
            case "dq":
               result = @"""";
               break;
            case "lvowels":
            case "lv":
               result = "aeiou";
               break;
            case "uvowels":
            case "uv":
               result = "AEIOU";
               break;
            case "lconsonants":
            case "lc":
               result = "bcdfghjklmnpqrstvwxyz";
               break;
            case "uconsonants":
            case "uc":
               result = "BCDFGHJKLMNPQRSTVWXYZ";
               break;
            case "real":
            case "r":
               result = "0-9eE,.+-";
               break;
         }

         return result;
      }

      public override string Pattern => @"^\s*(-)?\s*/(" + REGEX_IDENTIFIER + @")\b";

      public override IMaybe<string> Parse(string source, ref int index)
      {
         var negative = tokens[1] == "-";
         var word = tokens[2];
         var result = GetRange(word);

         return maybe(result.IsNotEmpty(), () => (negative ? "[^" : "[") + result + "]");
      }
   }
}