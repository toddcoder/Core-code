using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.RegularExpressions.Parsers
{
   public class OutsideRangeParser : BaseParser
   {
      public static string GetRange(string word) => word switch
      {
         "alphanum" or "an" => "A-Za-z0-9",
         "alpha"or "al" => "A-Za-z",
         "digit" => "0-9",
         "uppercase"or "u" => "A-Z",
         "lowercase"or "l" => "a-z",
         "hex"or "h" => "0-9A-Fa-f",
         "xml0"or "x0" => "_:A-Za-z",
         "xml"or "x" => "-._:A-Za-z0-9",
         "crlf" => @"\r\n",
         "cr" => @"\r",
         "lf" => @"\n",
         "punct" or "p" => @"!@#$%^&*()_+={}[]:;""'|\,.<>/?-",
         "squote" or "sq" => "'",
         "dquote" or "dq" => @"""",
         "lvowels" or "lv" => "aeiou",
         "uvowels" or "uv" => "AEIOU",
         "lconsonants" or "lc" => "bcdfghjklmnpqrstvwxyz",
         "uconsonants" or "uc" => "BCDFGHJKLMNPQRSTVWXYZ",
         "real" or "r" => "0-9eE,.+-",
         _ => ""
      };

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