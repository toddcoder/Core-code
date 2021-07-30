using System;
using System.Linq;
using Core.Assertions;

namespace Core.Markup.Rtf
{
   public class Color
   {
      protected int color;

      public Color()
      {
         color = 0;
      }

      public Color(byte red, byte green, byte blue)
      {
         color = (red << 16) + (green << 8) + blue;
      }

      public Color(int color)
      {
         this.color = color;
      }

      public Color(string hex)
      {
         hex.Must().HaveLengthOfExactly(6).OrThrow("String parameter hex should be of length 6.");

         hex = hex.ToUpper();
         if (hex.Any(hexDigit => !char.IsDigit(hexDigit) && hexDigit is < 'A' or > 'F'))
         {
            throw new Exception("Characters of parameter hex should be in [0-9,A-F,a-f]");
         }

         var red = Convert.ToByte(hex.Substring(0, 2), 16);
         var green = Convert.ToByte(hex.Substring(2, 2), 16);
         var blue = Convert.ToByte(hex.Substring(4, 2), 16);
         color = (red << 16) + (green << 8) + blue;
      }

      public Color(System.Drawing.Color color)
      {
         this.color = (color.R << 16) + (color.G << 8) + color.B;
      }

      public override bool Equals(object obj) => obj is Color rtfColor && rtfColor.color == color;

      public override int GetHashCode() => color;

      public string Red => ((color >> 16) % 256).ToString();

      public string Green => ((color >> 8) % 256).ToString();

      public string Blue => (color % 256).ToString();
   }
}