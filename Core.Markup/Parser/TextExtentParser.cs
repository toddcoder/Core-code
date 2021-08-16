using System.Text;
using Core.Markup.Code.Extents;
using Core.Matching;
using Core.Monads;

namespace Core.Markup.Parser
{
   public class TextExtentParser : Parser
   {
      public override Pattern Pattern => "^ .* $";

      public override Matched<Unit> Parse(ParsingState state)
      {
         var escaped = false;
         var line = state.Result.FirstGroup;
         var builder = new StringBuilder();
         var parsing = true;
         var lineEnding = false;

         for (var i = 0; parsing && i < line.Length; i++)
         {
            var ch = line[i];
            switch (ch)
            {
               case '/':
                  if (escaped)
                  {
                     builder.Append(ch);
                     escaped = false;
                  }
                  else
                  {
                     escaped = true;
                  }

                  break;
               case '*' or '[':
                  if (escaped)
                  {
                     builder.Append(ch);
                     escaped = false;
                  }
                  else
                  {
                     parsing = false;
                  }

                  break;
               case '\r' or '\n':
                  if (!lineEnding)
                  {
                     lineEnding = true;
                  }

                  break;
               default:
                  if (lineEnding)
                  {
                     parsing = false;
                  }
                  else
                  {
                     builder.Append(ch);
                     escaped = false;
                  }

                  break;
            }
         }

         var textExtent = new TextExtent(builder.ToString());
         state.Document.CurrentBlock.Add(textExtent);
         state.Source.Advance(textExtent.Text.Length);

         return Unit.Value;
      }
   }
}