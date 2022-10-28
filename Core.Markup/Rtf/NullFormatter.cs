namespace Core.Markup.Rtf;

public class NullFormatter : Formatter
{
   public NullFormatter(Paragraph paragraph) : base(paragraph, new CharFormat())
   {
   }

   public override Formatter Italic(bool on = true) => this;

   public override Formatter Bold(bool on = true) => this;

   public override Formatter Underline(bool on = true) => this;

   public override Formatter Bullet() => this;

   public override Formatter FontSize(float fontSize) => this;

   public override Formatter Alignment(Alignment alignment) => this;

   public override Formatter ForegroundColor(ColorDescriptor foregroundColor) => this;

   public override Formatter BackgroundColor(ColorDescriptor backgroundColor) => this;

   public override Formatter Font(FontDescriptor font) => this;

   public override Formatter FontStyle(FontStyleFlag fontStyleFlag) => this;

   public override Formatter Hyperlink(Hyperlink hyperlink) => this;

   public override Formatter Bookmark(string bookmark) => this;

   public override Formatter FirstLineIndent(float indentAmount) => this;
}