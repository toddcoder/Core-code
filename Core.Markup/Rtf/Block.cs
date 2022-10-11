namespace Core.Markup.Rtf;

public abstract class Block : Renderable
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
}