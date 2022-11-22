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

   public SubTextSet Outline(bool outline)
   {
      subText.Outline = outline;
      return this;
   }

   public SubTextSet UseControlForeColor(bool useControlForeColor)
   {
      subText.UseControlForeColor = useControlForeColor;
      return this;
   }

   public SubTextSet UseControlBackColor(bool useControlBackColor)
   {
      subText.UseControlBackColor = useControlBackColor;
      return this;
   }

   protected Size getTextSize()
   {
      var text = subText.Text;
      using var font = new Font(subText.FontName, subText.FontSize, subText.FontStyle);
      return TextRenderer.MeasureText(text, font);
   }

   public SubTextSet GoToUpperLeft(int margin) => X(margin).Y(margin);

   public SubTextSet GoToUpperRight(int margin)
   {
      var textSize = getTextSize();
      var x = size.Width - textSize.Width - margin;

      return X(x).Y(margin);
   }

   public SubTextSet GoToLowerLeft(int margin)
   {
      var textSize = getTextSize();
      var y = size.Height - textSize.Height - margin;

      return Y(y).X(margin);
   }

   public SubTextSet GoToLowerRight(int margin)
   {
      var textSize = getTextSize();
      var x = size.Width - textSize.Width - margin;
      var y = size.Height - textSize.Height - margin;

      return X(x).Y(y);
   }

   public SubText End => subText;
}