using System.Drawing;

namespace Core.WinForms.Controls;

public class MinusSymbolWriter : SymbolWriter
{
   public MinusSymbolWriter(Color foreColor, Color backColor) : base(foreColor, backColor)
   {
   }

   public override void OnPaint(Graphics g, Rectangle clientRectangle, bool enabled)
   {
      var y = clientRectangle.Height / 2;
      var margin = getMargin(clientRectangle);

      var color = getColor(enabled);
      using var pen = new Pen(color, 2);
      g.DrawLine(pen, margin, y, clientRectangle.Right - margin, y);
   }
}