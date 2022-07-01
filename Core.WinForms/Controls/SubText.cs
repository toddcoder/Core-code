using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Core.WinForms.Controls;

public class SubText
{
   protected Color defaultBackColor;

   public SubText(string text, int x, int y, Color defaultForeColor, Color defaultBackColor)
   {
      this.defaultBackColor = defaultBackColor;

      Text = text;
      X = x;
      Y = y;
      FontName = "Consolas";
      FontSize = 12;
      FontStyle = FontStyle.Regular;
      ForeColor = defaultForeColor;
      BackColor = defaultBackColor;
      Outline = false;
   }

   public string Text { get; }

   public int X { get; }

   public int Y { get; }

   public string FontName { get; set; }

   public float FontSize { get; set; }

   public FontStyle FontStyle { get; set; }

   public Color ForeColor { get; set; }

   public Color BackColor { get; set; }

   public bool Outline { get; set; }

   public SubText SetFont(string fontName, float fontSize, FontStyle fontStyle)
   {
      FontName = fontName;
      FontSize = fontSize;
      FontStyle = fontStyle;

      return this;
   }

   public SubText SetFontName(string fontName)
   {
      FontName = fontName;
      return this;
   }

   public SubText SetFontSize(float fontSize)
   {
      FontSize = fontSize;
      return this;
   }

   public SubText SetFontStyle(FontStyle fontStyle)
   {
      FontStyle = fontStyle;
      return this;
   }

   public SubText SetForeColor(Color foreColor)
   {
      ForeColor = foreColor;
      return this;
   }

   public SubText SetBackColor(Color backColor)
   {
      BackColor = backColor;
      return this;
   }

   public SubText SetOutline(bool outline)
   {
      Outline = outline;
      return this;
   }

   public SubText Draw(Graphics graphics)
   {
      using var font = new Font(FontName, FontSize);
      var location = new Point(X, Y);
      var flags = TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
      var size = TextRenderer.MeasureText(graphics, Text, font, new Size(int.MaxValue, int.MaxValue), flags);

      var rectangle = new Rectangle(location, size);

      graphics.HighQuality();
      graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

      if (BackColor != defaultBackColor)
      {
         using var brush = new SolidBrush(BackColor);
         graphics.FillRectangle(brush, rectangle);
      }

      if (Outline)
      {
         using var pen = new Pen(ForeColor);
         graphics.DrawRectangle(pen, rectangle);
      }

      TextRenderer.DrawText(graphics, Text, font, rectangle, ForeColor, flags);

      return this;
   }
}