using System.Linq;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.RegularExpressions.Parsers
{
   public class NamedClassParser : BaseParser
   {
      public override string Pattern => @"^\s*\b(alpha|digit|alnum|blank|cntrl|graph|lower|upper|print|punct|space|" +
         @"xdigit|lcon|ucon|lvow|uvow|squote|dquote|quote)\b";

      public override IMaybe<string> Parse(string source, ref int index) => tokens[1] switch
      {
         "alpha" => "a-zA-Z".Some(),
         "digit" => "0-9".Some(),
         "alnum" => "a-zA-Z0-9".Some(),
         "blank" => " \t".Some(),
         "cntrl" => new string(Enumerable.Range(0, 32).Select(i => (char)i).ToArray()).Escape(false).Some(),
         "graph" => new string(Enumerable.Range(0, 256).Where(i => i != 32).Select(i => (char)i).ToArray()).Escape(false).Some(),
         "lower" => "a-z".Some(),
         "upper" => "A-Z".Some(),
         "print" => new string(Enumerable.Range(0, 256).Select(i => (char)i).ToArray()).Escape(false).Some(),
         "punct" => "~`!@#$%^&*()_+=[]{}:;\"'<>,./?\\-".Escape(false).Some(),
         "space" => " /t/r/n".Some(),
         "xdigit" => "0-9a-fA-F".Some(),
         "lcon" => "bcdfghjklmnpqrstvwxyz".Some(),
         "ucon" => "BCDFGHJKLMNPQRSTVWXYZ".Some(),
         "lvow" => "aeiou".Some(),
         "uvow" => "AEIOU".Some(),
         "squote" => "'".Some(),
         "dquote" => "\"".Some(),
         "quote" => "'\"".Some(),
         _ => none<string>()
      };
   }
}