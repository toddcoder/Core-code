using System.Linq;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.RegularExpressions.Parsers
{
   public class NamedClassParser : BaseParser
   {
      public override string Pattern => @"^\s*\b(alpha|digit|alnum|blank|cntrl|graph|lower|upper|print|punct|space|" +
         @"xdigit|lcon|ucon|lvow|uvow|squote|dquote|quote)\b";

      public override IMaybe<string> Parse(string source, ref int index)
      {
         switch (tokens[1])
         {
            case "alpha":
               return "a-zA-Z".Some();
            case "digit":
               return "0-9".Some();
            case "alnum":
               return "a-zA-Z0-9".Some();
            case "blank":
               return " \t".Some();
            case "cntrl":
               return new string(Enumerable.Range(0, 32).Select(i => (char)i).ToArray()).Escape(false).Some();
            case "graph":
               return new string(Enumerable.Range(0, 256).Where(i => i != 32).Select(i => (char)i).ToArray()).Escape(false).Some();
            case "lower":
               return "a-z".Some();
            case "upper":
               return "A-Z".Some();
            case "print":
               return new string(Enumerable.Range(0, 256).Select(i => (char)i).ToArray()).Escape(false).Some();
            case "punct":
               return "~`!@#$%^&*()_+=[]{}:;\"'<>,./?\\-".Escape(false).Some();
            case "space":
               return " /t/r/n".Some();
            case "xdigit":
               return "0-9a-fA-F".Some();
            case "lcon":
               return "bcdfghjklmnpqrstvwxyz".Some();
            case "ucon":
               return "BCDFGHJKLMNPQRSTVWXYZ".Some();
            case "lvow":
               return "aeiou".Some();
            case "uvow":
               return "AEIOU".Some();
            case "squote":
               return "'".Some();
            case "dquote":
               return "\"".Some();
            case "quote":
               return "'\"".Some();
            default:
               return none<string>();
         }
      }
   }
}