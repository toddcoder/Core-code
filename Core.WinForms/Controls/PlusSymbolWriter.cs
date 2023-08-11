using System;
using System.Drawing;

namespace Core.WinForms.Controls;

public class PlusSymbolWriter : SymbolWriter
{
   public PlusSymbolWriter(Color foreColor, Color backColor) : base(foreColor, backColor)
   {
   }

   public override void OnPaint(Graphics g, Rectangle clientRectangle)
   {
      var x = clientRectangle.Width / 2;
      var y = clientRectangle.Height / 2;
      var margin = Math.Min(clientRectangle.Height, clientRectangle.Height) / 10;

      using var pen = new Pen(foreColor, 2);
      g.DrawLine(pen, margin, y, clientRectangle.Right - margin, y);
      g.DrawLine(pen, x, margin, x, clientRectangle.Bottom - margin);
   }
}