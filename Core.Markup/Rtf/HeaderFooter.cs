using System;
using System.Text;
using Core.Lists;

namespace Core.Markup.Rtf
{
   public class HeaderFooter : BlockList, IEquatable<HeaderFooter>
   {
      protected HeaderFooterType type;

      public HeaderFooter(HeaderFooterType type) : base(true, false, true, true, false)
      {
         this.type = type;
      }

      public override string Render()
      {
         var result = new StringBuilder();

         switch (type)
         {
            case HeaderFooterType.Header:
               result.AppendLine(@"{\header");
               break;
            case HeaderFooterType.Footer:
               result.AppendLine(@"{\footer");
               break;
            default:
               throw new Exception("Invalid HeaderFooterType");
         }

         result.AppendLine();

         foreach (var block in blocks)
         {
            block.DefaultCharFormat.CopyFrom(defaultCharFormat);
            result.AppendLine(block.Render());
         }

         result.AppendLine("}");
         return result.ToString();
      }

      public bool Equals(HeaderFooter other)
      {
         if (other is null)
         {
            return false;
         }

         return type == other.type && blocks.AllEqualTo(other.blocks);
      }

      public override bool Equals(object obj) => obj is HeaderFooter otherHeaderFooter && Equals(otherHeaderFooter);

      public override int GetHashCode() => (int)type;

      public static bool operator ==(HeaderFooter left, HeaderFooter right) => Equals(left, right);

      public static bool operator !=(HeaderFooter left, HeaderFooter right) => !Equals(left, right);
   }
}