using System.Drawing;
using System.Windows.Forms;

namespace Core.WinForms.Controls;

public class SubTextSet
{
   protected SubText subText;
   protected Size size;

   internal SubTextSet(SubText subText, Size size)
   {
      this.subText = subText;
      this.size = size;
   }

   public SubTextSet Text(string text)
   {
      subText.Text = text;
      return this;
   }

   public SubTextSet X(int x)
   {
      subText.X = x;
      return this;
   }

   public SubTextSet Y(int y)
   {
      subText.Y = y;
      return this;
   }

   public SubTextSet Font(string fontName, float fontSize, FontStyle fontStyle = System.Drawing.FontStyle.Regular)
   {
      subText.FontName = fontName;
      subText.FontSize = fontSize;
      subText.FontStyle = fontStyle;

      return this;
   }

   public SubTextSet FontName(string fontName)
   {
      subText.FontName = fontName;
      return this;
   }

   public SubTextSet FontSize(float fontSize)
   {
      subText.FontSize = fontSize;
      return this;
   }

   public SubTextSet FontStyle(FontStyle fontStyle)
   {
      subText.FontStyle = fontStyle;
      return this;
   }

   public SubTextSet ForeColor(Color foreColor)
   {
      subText.ForeColor = foreColor;
      return this;
   }

   public SubTextSet BackColor(Color backColor)
   {
      subText.BackColor = backColor;
      return this;
   }

   public SubTextSet Outline(bool outline = true)
   {
      subText.Outline = outline;
      return this;
   }

   public SubTextSet Invert(bool invert = true)
   {
      subText.Invert = invert;
      return this;
   }

   public SubTextSet TransparentBackground(bool transparentBackground = false)
   {
      subText.TransparentBackground = transparentBackground;
      return this;
   }

   protected Size getTextSize()
   {
      var text = subText.Text;
      using var font = new Font(subText.FontName, subText.FontSize, subText.FontStyle);
      return TextRenderer.MeasureText(text, font);
   }

   public SubTextSet GoToUpperLeft(int margin)
   {
      subText.SetAlignment(CardinalAlignment.NorthWest);
      subText.SetMargin(margin);

      return this;
   }

   public SubTextSet GoToUpperRight(int margin)
   {
      subText.SetAlignment(CardinalAlignment.NorthEast);
      subText.SetMargin(margin);

      return this;
   }

   public SubTextSet GoToLowerLeft(int margin)
   {
      subText.SetAlignment(CardinalAlignment.SouthWest);
      subText.SetMargin(margin);

      return this;
   }

   public SubTextSet GoToLowerRight(int margin)
   {
      subText.SetAlignment(CardinalAlignment.SouthEast);
      subText.SetMargin(margin);

      return this;
   }

   public SubTextSet GoToMiddleLeft(int margin)
   {
      subText.SetAlignment(CardinalAlignment.West);
      subText.SetMargin(margin);

      return this;
   }

   public SubTextSet GoToMiddleRight(int margin)
   {
      subText.SetAlignment(CardinalAlignment.East);
      subText.SetMargin(margin);

      return this;
   }

   public SubTextSet GoToTop(int margin)
   {
      subText.SetAlignment(CardinalAlignment.North);
      subText.SetMargin(margin);

      return this;
   }

   public SubTextSet GoToBottom(int margin)
   {
      subText.SetAlignment(CardinalAlignment.South);
      subText.SetMargin(margin);

      return this;
   }

   public SubTextSet Alignment(CardinalAlignment alignment)
   {
      subText.SetAlignment(alignment);
      return this;
   }

   public SubTextSet Margin(int margin)
   {
      subText.SetMargin(margin);
      return this;
   }

   public SubTextSet Small() => FontSize(8);

   public SubTextSet Italic() => FontStyle(System.Drawing.FontStyle.Italic);

   public SubTextSet Bold() => FontStyle(System.Drawing.FontStyle.Bold);

   public SubText End => subText;
}