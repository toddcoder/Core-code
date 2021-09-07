using System;

namespace Core.Markup.Rtf
{
   public abstract class Block : Renderable, IEquatable<Block>
   {
      public abstract Alignment Alignment { get; set; }

      public abstract Margins Margins { get; }

      public abstract CharFormat DefaultCharFormat { get; }

      public abstract bool StartNewPage { get; set; }

      public abstract string BlockHead { set; }

      public abstract string BlockTail { set; }

      public string AlignmentCode() => Alignment switch
      {
         Alignment.Left => @"\ql",
         Alignment.Right => @"\qr",
         Alignment.Center => @"\qc",
         Alignment.FullyJustify => @"\qj",
         _ => @"\qd"
      };

      public bool Equals(Block other)
      {
         if (other is null)
         {
            return false;
         }

         return Alignment == other.Alignment && Equals(Margins, other.Margins) && Equals(DefaultCharFormat, other.DefaultCharFormat) &&
            StartNewPage == other.StartNewPage;
      }

      public override bool Equals(object obj) => obj is Block otherBlock && Equals(otherBlock);

      public override int GetHashCode()
      {
         unchecked
         {
            var hashCode = (int)Alignment;
            hashCode = hashCode * 397 ^ (Margins != null ? Margins.GetHashCode() : 0);
            hashCode = hashCode * 397 ^ (DefaultCharFormat != null ? DefaultCharFormat.GetHashCode() : 0);
            hashCode = hashCode * 397 ^ StartNewPage.GetHashCode();

            return hashCode;
         }
      }

      public static bool operator ==(Block left, Block right) => Equals(left, right);

      public static bool operator !=(Block left, Block right) => !Equals(left, right);
   }
}