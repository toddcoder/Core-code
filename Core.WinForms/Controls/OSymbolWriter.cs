using System.Drawing;

namespace Core.WinForms.Controls;

public class OSymbolWriter : SymbolWriter
{
   public OSymbolWriter(Color foreColor, Color backColor) : base(foreColor, backColor)
   {
   }

   public override void OnPaint(Graphics g, Rectangle clientRectangle, bool enabled)
   {
      var margin = getMargin(clientRectangle);
      var rectangle = clientRectangle.Reposition(margin, margin).Resize(-2 * margin, -2 * margin);
      var color = getColor(enabled);
      using var pen = new Pen(color, 2);
      g.DrawEllipse(pen, rectangle);
   }
}