using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Core.WinForms.Controls;

public class SubText
{
   protected Color defaultBackColor;
   protected bool useControlForeColor;
   protected bool useControlBackColor;
   protected Size size;

   public SubText(string text, int x, int y, Color defaultForeColor, Color defaultBackColor, Size size)
   {
      this.defaultBackColor = defaultBackColor;
      this.size=size;

      useControlForeColor = false;
      useControlBackColor = false;

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

   public string Text { get; set; }

   public int X { get; set; }

   public int Y { get; set; }

   public SubTextSet Set => new(this, size);

   public string FontName { get; set; }

   public float FontSize { get; set; }

   public FontStyle FontStyle { get; set; }

   public Color ForeColor { get; set; }

   public Color BackColor { get; set; }

   public bool Outline { get; set; }

   public bool UseControlForeColor
   {
      get => useControlForeColor;
      set => useControlForeColor = value;
   }

   public bool UseControlBackColor
   {
      get => useControlBackColor;
      set => useControlBackColor = value;
   }

   public SubText SetFont(string fontName, float fontSize, FontStyle fontStyle)
   {
      FontName = fontName;
      FontSize = fontSize;
      FontStyle = fontStyle;

      return this;
   }

   [Obsolete("Use .Set.FontName")]
   public SubText SetFontName(string fontName)
   {
      FontName = fontName;
      return this;
   }

   [Obsolete("Use .Set.FontSize")]
   public SubText SetFontSize(float fontSize)
   {
      FontSize = fontSize;
      return this;
   }

   [Obsolete("Use .Set.FontStyle")]
   public SubText SetFontStyle(FontStyle fontStyle)
   {
      FontStyle = fontStyle;
      return this;
   }

   [Obsolete("Use Set.ForeColor")]
   public SubText SetForeColor(Color foreColor)
   {
      ForeColor = foreColor;
      useControlForeColor = false;

      return this;
   }

   [Obsolete("Use Set.BackColor")]
   public SubText SetBackColor(Color backColor)
   {
      BackColor = backColor;
      useControlBackColor = false;

      return this;
   }

   [Obsolete("Use Set.Outline")]
   public SubText SetOutline(bool outline)
   {
      Outline = outline;
      return this;
   }

   [Obsolete("Use Set.UseControlForeColor")]
   public SubText SetUseControlForeColor()
   {
      useControlForeColor = true;
      return this;
   }

   [Obsolete("Use Set.UserControlBackColor")]
   public SubText SetUseControlBackColor()
   {
      useControlBackColor = true;
      return this;
   }

   public SubText Draw(Graphics graphics, Color controlForeColor, Color controlBackColor)
   {
      using var font = new Font(FontName, FontSize);
      var location = new Point(X, Y);
      var flags = TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
      var measuredSize = TextRenderer.MeasureText(graphics, Text, font, new Size(int.MaxValue, int.MaxValue), flags);

      var rectangle = new Rectangle(location, measuredSize);

      graphics.HighQuality();
      graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

      var backColor = useControlBackColor ? controlBackColor : BackColor;
      var foreColor = useControlForeColor ? controlForeColor : ForeColor;

      if (backColor != defaultBackColor)
      {
         using var brush = new SolidBrush(BackColor);
         graphics.FillRectangle(brush, rectangle);
      }

      if (Outline)
      {
         using var pen = new Pen(foreColor);
         graphics.DrawRectangle(pen, rectangle);
      }

      TextRenderer.DrawText(graphics, Text, font, rectangle, foreColor, flags);

      return this;
   }
}