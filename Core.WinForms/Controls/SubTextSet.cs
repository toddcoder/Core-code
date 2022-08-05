using System.Drawing;

namespace Core.WinForms.Controls;

public class SubTextSet
{
   protected SubText subText;

   internal SubTextSet(SubText subText)
   {
      this.subText = subText;
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

   public SubText End => subText;
}