using System.Collections.Generic;

namespace Core.Markup.Code.Blocks
{
   public class List : Block
   {
      protected List<Paragraph> paragraphs;

      public List()
      {
         paragraphs = new List<Paragraph>();
      }
   }
}