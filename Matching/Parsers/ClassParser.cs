using System.Collections.Generic;
using System.Text;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Matching.Parsers
{
   public class ClassParser : BaseParser
   {
      protected List<BaseParser> parsers;

      public ClassParser() => parsers = new List<BaseParser>
      {
         new StringParser(),
         new SlashClassParser(),
         new InsideRangeParser(),
         new UnmodifiedParser(),
         new NamedClassParser(),
         new QuoteParser(),
         new EndOfClassParser()
      };

      public override string Pattern => @"^\s*(-)?\s*(/)?\[";

      public override Maybe<string> Parse(string source, ref int index)
      {
         var negative = tokens[1] == "-";
         var enclose = tokens[2] == "/";

         var content = new StringBuilder();

         while (index < source.Length)
         {
            var added = false;
            foreach (var parser in parsers)
            {
               var result = parser.Scan(source, ref index);
               if (result.If(out var r))
               {
                  if (r == "]")
                  {
                     var value = (negative ? "[^" : "[") + content + "]";
                     if (enclose)
                     {
                        value = $"({value})";
                     }

                     return value.Some();
                  }
                  else
                  {
                     content.Append(r);
                     added = true;
                     break;
                  }
               }
            }

            if (!added)
            {
               return none<string>();
            }
         }

         return none<string>();
      }
   }
}