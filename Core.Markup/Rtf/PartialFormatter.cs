namespace Core.Markup.Rtf;

public class PartialFormatter : Formatter
{
   protected int begin;
   protected int end;

   public PartialFormatter(Paragraph paragraph, CharFormat format, int begin, int end) : base(paragraph, format)
   {
      this.begin = begin;
      this.end = end;
   }

   public int Begin => begin;

   public int End => end;

   public override Formatter Hyperlink(Hyperlink hyperlink)
   {
      paragraph.AddPendingHyperlink(hyperlink);
      return this;
   }
}