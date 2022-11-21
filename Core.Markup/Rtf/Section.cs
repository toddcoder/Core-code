using System.Text;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Rtf;

public class Section : Block
{
   protected Alignment alignment;
   protected SectionFooter sectionFooter;
   protected readonly Margins margins;

   public Section(SectionStartEnd startEnd, Document doc)
   {
      ParentDocument = doc;
      alignment = Alignment.None;
      PageOrientation = PaperOrientation.Portrait;
      StartEnd = startEnd;
      FooterPositionFromPageBottom = 720;
      sectionFooter = new SectionFooter();
      margins = new Margins();
   }

   public override bool StartNewPage { get; set; }

   public override Alignment Alignment
   {
      get => alignment;
      set => alignment = value;
   }

   public SectionStartEnd StartEnd { get; }

   public PaperOrientation PageOrientation { get; set; }

   public SectionFooter SectionFooter => sectionFooter;

   public int FooterPositionFromPageBottom { get; }

   public int PageWidth { get; set; }

   public int PageHeight { get; set; }

   public Document ParentDocument { get; }

   public override string Render()
   {
      var result = new StringBuilder();
      if (StartEnd == SectionStartEnd.Start)
      {
         result.AppendLine($@"{{\sectd\ltrsect\footery{FooterPositionFromPageBottom}\sectdefaultcl\sftnbj{AlignmentCode()} ");
         if (PageOrientation == PaperOrientation.Landscape)
         {
            result.Append(@"\lndscpsxn ");
         }

         result.Append($@"\pgwsxn{PageWidth}\pghsxn{PageHeight} ");
         if (!ParentDocument.Margins.Equals(Margins))
         {
            result.Append($@"\marglsxn{Margins[Direction.Left]}\margrsxn{Margins[Direction.Right]}");
            result.Append($@"\margtsxn{Margins[Direction.Top]}\margbsxn{Margins[Direction.Bottom]} ");
         }

         result.AppendLine(sectionFooter.Render());
      }
      else
      {
         result.AppendLine(@"\sect }");
      }

      return result.ToString();
   }

   public override Margins Margins => margins;

   public override CharFormat DefaultCharFormat => throw fail("DefaultCharFormat not supported for sections.");

   public override string BlockHead
   {
      set => throw fail("BlockHead is not supported for sections.");
   }

   public override string BlockTail
   {
      set => throw fail("BlockTail is not supported for sections.");
   }
}