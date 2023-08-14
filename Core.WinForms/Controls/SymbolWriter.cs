using System;
using System.Drawing;
using System.Drawing.Drawing2D;

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

   protected int getMargin(Rectangle clientRectangle) => Math.Min(clientRectangle.Height, clientRectangle.Height) / 10;

   protected Color getColor(bool enabled) => enabled ? foreColor : Color.White;

   public abstract void OnPaint(Graphics g, Rectangle clientRectangle, bool enabled);

   public virtual void OnPaintBackground(Graphics g, Rectangle clientRectangle, bool enabled)
   {
      if (enabled)
      {
         using var brush = new SolidBrush(backColor);
         g.FillRectangle(brush, clientRectangle);
      }
      else
      {
         using var disabledBrush = new HatchBrush(HatchStyle.BackwardDiagonal, Color.Black, Color.Gold);
         g.FillRectangle(disabledBrush, clientRectangle);
      }
   }
}