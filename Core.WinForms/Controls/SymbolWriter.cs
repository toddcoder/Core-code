using System.Drawing;

namespace Core.WinForms.Controls;

public abstract class SymbolWriter
{
   protected Color foreColor;
   protected Color backColor;

   protected SymbolWriter(Color foreColor, Color backColor)
   {
      this.foreColor = foreColor;
      this.backColor = backColor;
   }

   public abstract void OnPaint(Graphics g, Rectangle clientRectangle);

   public virtual void OnPaintBackground(Graphics g, Rectangle clientRectangle)
   {
      using var brush = new SolidBrush(backColor);
      g.FillRectangle(brush, clientRectangle);
   }
}