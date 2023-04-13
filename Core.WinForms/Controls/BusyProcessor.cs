using System.Drawing;

namespace Core.WinForms.Controls;

public abstract class BusyProcessor
{
   protected Rectangle clientRectangle;

   public BusyProcessor(Rectangle clientRectangle)
   {
      this.clientRectangle = clientRectangle;
   }

   public abstract void Advance();

   public abstract void OnPaint(Graphics g);
}