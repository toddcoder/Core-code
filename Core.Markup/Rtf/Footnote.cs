using System.Text;
using Core.Assertions;

namespace Core.Markup.Rtf
{
   public class Footnote : BlockList
   {
      protected int position;

      public Footnote(int position, int textLength) : base(true, false, false, true, false)
      {
         position.Must().BeBetween(0).Until(textLength).OrThrow($"Invalid footnote position: {position} (text length={textLength})");
         this.position = position;
      }

      public int Position => position;

      public override string Render()
      {
         var result = new StringBuilder();

         result.AppendLine(@"{\super\chftn}");
         result.AppendLine(@"{\footnote\plain\chftn");
         blocks[blocks.Count - 1].BlockTail = "}";
         result.Append(base.Render());
         result.AppendLine("}");

         return result.ToString();
      }
   }
}