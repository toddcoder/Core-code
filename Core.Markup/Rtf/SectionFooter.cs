using System.Text;

namespace Core.Markup.Rtf
{
   public class SectionFooter : BlockList
   {
      public SectionFooter() : base(true, true, true, true, true)
      {
      }

      public override string Render()
      {
         var result = new StringBuilder();

         result.AppendLine(@"{\footerr \ltrpar \pard\plain");
         result.AppendLine(@"\par ");
         result.Append(base.Render());
         result.AppendLine(@"\par");
         result.AppendLine(@"}");

         return result.ToString();
      }
   }
}