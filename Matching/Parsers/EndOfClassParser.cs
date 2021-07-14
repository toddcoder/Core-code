﻿using Core.Monads;

namespace Core.Matching.Parsers
{
   public class EndOfClassParser : BaseParser
   {
      public override string Pattern => @"^\s*\]";

      public override Maybe<string> Parse(string source, ref int index) => "]".Some();
   }
}