using System;
using System.Drawing;

namespace Core.WinForms.Controls;

public class XSymbolWriter : SymbolWriter
{
   public XSymbolWriter(Color foreColor, Color backColor) : base(foreColor, backColor)
   {
   }

   public override void OnPaint(Graphics g, Rectangle clientRectangle)
   {
      var margin = Math.Min(clientRectangle.Height, clientRectangle.Height) / 10;
      using var pen = new Pen(foreColor, 2);
      var bottomX = clientRectangle.Right - margin;
      var bottomY = clientRectangle.Bottom - margin;
      g.DrawLine(pen, margin, margin, bottomX, bottomY);
      g.DrawLine(pen, bottomX, margin, margin, bottomY);
   }
}