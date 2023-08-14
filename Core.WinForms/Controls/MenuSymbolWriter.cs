using System.Drawing;

namespace Core.WinForms.Controls;

public class MenuSymbolWriter : SymbolWriter
{
   public MenuSymbolWriter(Color foreColor, Color backColor) : base(foreColor, backColor)
   {
   }

   public override void OnPaint(Graphics g, Rectangle clientRectangle, bool enabled)
   {
      var margin = getMargin(clientRectangle);
      var height = 6 + 2 * margin;
      var top = (clientRectangle.Height - height) / 2;
      var color = getColor(enabled);
      using var pen = new Pen(color, 2);
      for (var i = 0; i < 3; i++)
      {
         var y = top + i * (2 + margin);
         g.DrawLine(pen, margin, y, clientRectangle.Right - margin, y);
      }
   }
}