using System.Drawing;

namespace Core.WinForms.Controls;

public class SubText
{
   public SubText(string text, int x, int y, string fontName, float fontSize, FontStyle fontStyle, Color color)
   {
      Text = text;
      X = x;
      Y = y;
      FontName = fontName;
      FontSize = fontSize;
      FontStyle = fontStyle;
      Color = color;
   }

   public string Text { get; }

   public int X { get; }

   public int Y { get; }

   public string FontName { get; }

   public float FontSize { get; }

   public FontStyle FontStyle { get; }

   public Color Color { get; }
}