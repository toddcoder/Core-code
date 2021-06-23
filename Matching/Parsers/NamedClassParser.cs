using System.Linq;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Matching.Parsers
{
   public class NamedClassParser : BaseParser
   {
      public override string Pattern => @"^\s*\b(alpha|digit|alnum|blank|cntrl|graph|lower|upper|print|punct|space|" +
         @"xdigit|lcon|ucon|lvow|uvow|squote|dquote|quote)\b";

      public override Maybe<string> Parse(string source, ref int index) => tokens[1] switch
      {
         "alpha" => "a-zA-Z".Some(),
         "digit" => "0-9".Some(),
         "alnum" => "a-zA-Z0-9".Some(),
         "blank" => " \t".Some(),
         "cntrl" => escape(new string(Enumerable.Range(0, 32).Select(i => (char)i).ToArray())).Some(),
         "graph" => escape(new string(Enumerable.Range(0, 256).Where(i => i != 32).Select(i => (char)i).ToArray())).Some(),
         "lower" => "a-z".Some(),
         "upper" => "A-Z".Some(),
         "print" => escape(new string(Enumerable.Range(0, 256).Select(i => (char)i).ToArray())).Some(),
         "punct" => escape("~`!@#$%^&*()_+=[]{}:;\"'<>,./?\\-").Some(),
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