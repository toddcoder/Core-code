using System.Drawing;
using System.Windows.Forms;

namespace Core.WinForms.Controls;

public class ProgressDefiniteProcessor
{
   protected Rectangle percentRectangle;
   protected Rectangle textRectangle;
   protected Font font;

   public ProgressDefiniteProcessor(Font font, Graphics graphics, Rectangle clientRectangle)
   {
      this.font = font;

      percentRectangle = getPercentRectangle(graphics, clientRectangle);
      textRectangle = getTextRectangle(clientRectangle);
   }

   public Rectangle PercentRectangle => percentRectangle;

   public Rectangle TextRectangle => textRectangle;

   protected Rectangle getPercentRectangle(Graphics graphics, Rectangle clientRectangle)
   {
      var size = TextRenderer.MeasureText(graphics, "100%", font);
      size = size with { Height = clientRectangle.Height };
      return new Rectangle(clientRectangle.Location, size);
   }

   protected Rectangle getTextRectangle(Rectangle clientRectangle)
   {
      return clientRectangle with { X = clientRectangle.X + percentRectangle.Width, Width = clientRectangle.Width - percentRectangle.Width };
   }

   public void OnPaint(Graphics graphics)
   {
      using var percentBrush = new SolidBrush(Color.LightSteelBlue);
      graphics.FillRectangle(percentBrush, percentRectangle);
   }
}