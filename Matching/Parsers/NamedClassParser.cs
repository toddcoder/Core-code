using System.Linq;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Matching.Parsers;

public class NamedClassParser : BaseParser
{
   public override string Pattern => @"^\s*\b(alpha|digit|alnum|blank|cntrl|graph|lower|upper|print|punct|space|" +
      @"xdigit|lcon|ucon|lvow|uvow|squote|dquote|quote)\b";

   public override Maybe<string> Parse(string source, ref int index) => tokens[1] switch
   {
      "alpha" => "a-zA-Z",
      "digit" => "0-9",
      "alnum" => "a-zA-Z0-9",
      "blank" => " \t",
      "cntrl" => escape(new string(Enumerable.Range(0, 32).Select(i => (char)i).ToArray())),
      "graph" => escape(new string(Enumerable.Range(0, 256).Where(i => i != 32).Select(i => (char)i).ToArray())),
      "lower" => "a-z",
      "upper" => "A-Z",
      "print" => escape(new string(Enumerable.Range(0, 256).Select(i => (char)i).ToArray())),
      "punct" => escape("~`!@#$%^&*()_+=[]{}:;\"'<>,./?\\-"),
      "space" => " /t/r/n",
      "xdigit" => "0-9a-fA-F",
      "lcon" => "bcdfghjklmnpqrstvwxyz",
      "ucon" => "BCDFGHJKLMNPQRSTVWXYZ",
      "lvow" => "aeiou",
      "uvow" => "AEIOU",
      "squote" => "'",
      "dquote" => "\"",
      "quote" => "'\"",
      _ => nil
   };
}