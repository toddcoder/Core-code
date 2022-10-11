using System.Linq;
using System.Text;
using Core.Collections;
using Core.Computers;

namespace Core.Markup.Rtf;

public class Document : BlockList
{
   protected PaperSize paperSize;
   protected PaperOrientation paperOrientation;
   protected Margins margins;
   protected Lcid lcid;
   protected HeaderFooter header;
   protected HeaderFooter footer;
   protected StringHash<int> fontTable;
   protected Hash<Color, int> colorTable;

   public Document(PaperSize paperSize = PaperSize.Letter, PaperOrientation paperOrientation = PaperOrientation.Portrait, Lcid lcid = Lcid.English)
   {
      this.paperSize = paperSize;
      this.paperOrientation = paperOrientation;
      this.lcid = lcid;

      margins = new Margins();
      if (paperOrientation == PaperOrientation.Portrait)
      {
         margins[Direction.Top] = DefaultValue.A4_SHORT_EDGES;
         margins[Direction.Right] = DefaultValue.A4_LONG_EDGES;
         margins[Direction.Bottom] = DefaultValue.A4_SHORT_EDGES;
         margins[Direction.Left] = DefaultValue.A4_LONG_EDGES;
      }
      else
      {
         margins[Direction.Top] = DefaultValue.A4_LONG_EDGES;
         margins[Direction.Right] = DefaultValue.A4_SHORT_EDGES;
         margins[Direction.Bottom] = DefaultValue.A4_LONG_EDGES;
         margins[Direction.Left] = DefaultValue.A4_SHORT_EDGES;
      }

      header = new HeaderFooter(HeaderFooterType.Header);
      footer = new HeaderFooter(HeaderFooterType.Footer);

      fontTable = new StringHash<int>(true) { [DefaultValue.FONT] = 0 };
      colorTable = new Hash<Color, int> { [new Color()] = 0 };
   }

   public Margins Margins
   {
      get => margins;
      set => margins = value;
   }

   public HeaderFooter Header => header;

   public HeaderFooter Footer => footer;

   public ColorDescriptor DefaultColor => new(0);

   public FontDescriptor DefaultFont => new(0);

   public void SetDefaultFont(string fontName) => fontTable[fontName] = 0;

   public FontDescriptor Font(string fontName)
   {
      var index = fontTable.Memoize(fontName, _ => fontTable.Count);
      return new FontDescriptor(index);
   }

   public ColorDescriptor Color(Color color)
   {
      var index = colorTable.Memoize(color, _ => colorTable.Count);
      return new ColorDescriptor(index);
   }

   public ColorDescriptor Color(System.Drawing.Color color)
   {
      var rtfColor = new Color(color.R, color.G, color.B);
      return Color(rtfColor);
   }

   public ColorDescriptor Color(byte red, byte green, byte blue) => Color(new Color(red, green, blue));

   public ColorDescriptor Color(int color) => Color(new Color(color));

   public ColorDescriptor Color(string colorName)
   {
      var systemColor = System.Drawing.Color.FromName(colorName);
      var color = new Color(systemColor);

      return Color(color);
   }

   public Table Table(float fontSize)
   {
      var baseValue = paperOrientation == PaperOrientation.Portrait ? paperSize.PaperWidthInPoints(paperOrientation)
         : paperSize.PaperHeightInPoints(paperOrientation);
      var horizontalWidth = baseValue - margins[Direction.Left] - margins[Direction.Right];

      return Table(horizontalWidth, fontSize);
   }

   public override string Render()
   {
      var rtf = new StringBuilder();

      void prolog()
      {
         rtf.AppendLine(@"{\rtf1\ansi\deff0");
         rtf.AppendLine();
      }

      void insertFontTable()
      {
         rtf.AppendLine(@"{\fonttbl");

         foreach (var (font, index) in fontTable.OrderBy(i => i.Value))
         {
            rtf.AppendLine($@"{{\f{index} {font.UnicodeEncode()};}}");
         }

         rtf.AppendLine("}");
         rtf.AppendLine();
      }

      void insertColorTable()
      {
         rtf.AppendLine(@"{\colortbl");
         rtf.AppendLine(";");

         foreach (var (color, _) in colorTable.OrderBy(i => i.Value).Where(i => i.Value != 0))
         {
            rtf.AppendLine($@"\red{color.Red}\green{color.Green}\blue{color.Blue};");
         }

         rtf.AppendLine("}");
         rtf.AppendLine();
      }

      void preliminary()
      {
         rtf.AppendLine($@"\deflang{(int)lcid}\plain\fs{ConversionExtensions.PointToHalfPoint(DefaultValue.FONT_SIZE)}\widowctrl\hyphauto\ftnbj");
         rtf.Append($@"\paperw{paperSize.PaperWidthInTwips(paperOrientation)}");
         rtf.AppendLine($@"\paperh{paperSize.PaperHeightInTwips(paperOrientation)}");

         rtf.AppendLine($@"\margt{margins[Direction.Top].PointsToTwips()}");
         rtf.AppendLine($@"\margr{margins[Direction.Right].PointsToTwips()}");
         rtf.AppendLine($@"\margb{margins[Direction.Bottom].PointsToTwips()}");
         rtf.AppendLine($@"\margl{margins[Direction.Left].PointsToTwips()}");

         if (paperOrientation == PaperOrientation.Landscape)
         {
            rtf.AppendLine(@"\landscape");
         }

         rtf.Append(header.Render());
         rtf.Append(footer.Render());

         rtf.AppendLine();
      }

      void documentBody() => rtf.Append(base.Render());

      void ending() => rtf.AppendLine("}");

      prolog();
      insertFontTable();
      insertColorTable();
      preliminary();
      documentBody();
      ending();

      return rtf.ToString();
   }

   public void Save(FileName file) => file.Text = Render();
}