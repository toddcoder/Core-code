using System.Drawing;

namespace Core.WinForms.Controls;

public class XSymbolWriter : SymbolWriter
{
   public XSymbolWriter(Color foreColor, Color backColor) : base(foreColor, backColor)
   {
   }

   public override void OnPaint(Graphics g, Rectangle clientRectangle, bool enabled)
   {
      var margin = getMargin(clientRectangle);
      var color = getColor(enabled);
      using var pen = new Pen(color, 2);
      var bottomX = clientRectangle.Right - margin;
      var bottomY = clientRectangle.Bottom - margin;
      g.DrawLine(pen, margin, margin, bottomX, bottomY);
      g.DrawLine(pen, bottomX, margin, margin, bottomY);
   }
}