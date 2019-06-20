using System.Text;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.RegularExpressions.Parsers
{
   public class SpanBreakParser : BaseParser
   {
      public override string Pattern => @"^\s*(-\s*)?(/\s*)?\{";

      public override IMaybe<string> Parse(string source, ref int index)
      {
         var builder = new StringBuilder();
         var not = tokens[1].IsNotEmpty();
         var grouped = tokens[2].IsNotEmpty();
         var prefix = (grouped ? "(" : "") + (not ? "[^" : "[");
         var quantifier = not ? "*?" : "+";
         var suffix = grouped ? "]" + quantifier + ")" : "]" + quantifier;
         var escaped = false;

         for (var i = index; i < source.Length; i++)
         {
            var ch = source[i];
            switch (ch)
            {
               case '/':
                  if (escaped)
                  {
                     builder.Append('/');
                     escaped = false;
                  }
                  else
                  {
                     escaped = true;
                  }

                  break;
               case '}':
                  if (escaped)
                  {
                     builder.Append('}');
                     escaped = false;
                  }
                  else
                  {
                     index = i + 1;
                     return (prefix + builder + suffix).Some();
                  }

                  break;
               case ' ':
               case '\t':
                  continue;
               default:
                  if (escaped)
                  {
                     builder.Append(@"\");
                     builder.Append(ch);
                     escaped = false;
                  }
                  else
                  {
                     builder.Append(ch.ToString().Escape(false));
                  }

                  break;
            }
         }

         return none<string>();
      }
   }
}