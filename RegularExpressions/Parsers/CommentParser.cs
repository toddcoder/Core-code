﻿using System.Text;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.RegularExpressions.Parsers
{
   public class CommentParser : BaseParser
   {
      public override string Pattern => @"^\s*/\*";

      public override IMaybe<string> Parse(string source, ref int index)
      {
         var star = false;
         var contents = new StringBuilder();

         for (var i = index; i < source.Length; i++)
         {
            var ch = source[i];
            switch (star)
            {
               case false when ch == '*':
                  star = true;
                  continue;
               case true when ch == '/':
                  index = i + 1;
                  return $"(?#{contents.ToString().Escape()})".Some();
               default:
                  contents.Append(ch);
                  star = false;
                  break;
            }
         }

         return none<string>();
      }
   }
}