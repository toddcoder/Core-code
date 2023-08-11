using System;
using System.Drawing;

namespace Core.WinForms.Controls;

public class OSymbolWriter : SymbolWriter
{
   public OSymbolWriter(Color foreColor, Color backColor) : base(foreColor, backColor)
   {
   }

   public override void OnPaint(Graphics g, Rectangle clientRectangle)
   {
      var margin = Math.Min(clientRectangle.Height, clientRectangle.Height) / 10;
      var rectangle = clientRectangle.Reposition(margin, margin).Resize(-2 * margin, -2 * margin);
      using var pen = new Pen(foreColor, 2);
      g.DrawEllipse(pen, rectangle);
   }
}