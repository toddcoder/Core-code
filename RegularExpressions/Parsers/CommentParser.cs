using System.Text;
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
            if (!star && ch == '*')
            {
               star = true;
               continue;
            }

            if (star && ch == '/')
            {
               index = i + 1;
               return new Some<string>($"(?#{contents.ToString().Escape()})");
            }

            contents.Append(ch);
            star = false;
         }

         return none<string>();
      }
   }
}