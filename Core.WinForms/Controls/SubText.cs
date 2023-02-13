using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class SubText : IEquatable<SubText>
{
   protected Size size;
   protected bool invert;
   protected Maybe<Color> _foreColor;
   protected Maybe<Color> _backColor;

   public SubText(string text, int x, int y, Size size, bool invert = false)
   {
      Text = text;
      X = x;
      Y = y;
      this.size = size;
      this.invert = invert;

      Id = Guid.NewGuid();

      _foreColor = nil;
      _backColor = nil;

      FontName = "Consolas";
      FontSize = 12;
      FontStyle = FontStyle.Regular;
      Outline = false;
   }

   public string Text { get; set; }

   public int X { get; set; }

   public int Y { get; set; }

   public SubTextSet Set => new(this, size);

   public string FontName { get; set; }

   public float FontSize { get; set; }

   public FontStyle FontStyle { get; set; }

   public bool Outline { get; set; }

   public bool Invert
   {
      get => invert;
      set => invert = value;
   }

   public Guid Id { get; }

   public Maybe<Color> ForeColor
   {
      set => _foreColor = value;
   }

   public Maybe<Color> BackColor
   {
      set => _backColor = value;
   }

   public SubText SetFont(string fontName, float fontSize, FontStyle fontStyle)
   {
      FontName = fontName;
      FontSize = fontSize;
      FontStyle = fontStyle;

      return this;
   }

   protected SubText draw(Graphics graphics, Color foreColor, Color backColor)
   {
      using var font = new Font(FontName, FontSize, FontStyle);
      var location = new Point(X, Y);
      var flags = TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
      var measuredSize = TextRenderer.MeasureText(graphics, Text, font, new Size(int.MaxValue, int.MaxValue), flags);

      var rectangle = new Rectangle(location, measuredSize);

      graphics.HighQuality();
      graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

      var foreColorToUse = Invert ? backColor : foreColor;
      var backColorToUse = Invert ? foreColor : backColor;

      using var brush = new SolidBrush(backColorToUse);
      graphics.FillRectangle(brush, rectangle);
      if (!invert && Outline)
      {
         using var pen = new Pen(foreColorToUse);
         graphics.DrawRectangle(pen, rectangle);
      }

      TextRenderer.DrawText(graphics, Text, font, rectangle, foreColorToUse, flags);

      return this;
   }

   public SubText Draw(Graphics graphics, Color foreColor, Color backColor)
   {
      foreColor = _foreColor | foreColor;
      backColor = _backColor | backColor;

      return draw(graphics, foreColor, backColor);
   }

   public Maybe<SubText> Draw(Graphics graphics)
   {
      if (_foreColor is (true, var foreColor) && _backColor is (true, var backColor))
      {
         return draw(graphics, foreColor, backColor);
      }
      else
      {
         return nil;
      }
   }

   public bool Equals(SubText other) => Id.Equals(other.Id);

   public override bool Equals(object obj) => obj is SubText other && Equals(other);

   public override int GetHashCode() => Id.GetHashCode();

   public static bool operator ==(SubText left, SubText right) => Equals(left, right);

   public static bool operator !=(SubText left, SubText right) => !Equals(left, right);
}