using System.Drawing;
using Core.Enumerables;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class AlternateWriter
{
   protected UiAction uiAction;
   protected string[] alternates;
   protected int selectedIndex;

   public AlternateWriter(UiAction uiAction, string[] alternates)
   {
      this.uiAction = uiAction;
      this.alternates = alternates;

      selectedIndex = 0;
   }

   protected (Rectangle indicatorRectangle, Rectangle textRectangle) splitRectangle(Rectangle rectangle)
   {
      var width = rectangle.Height;

      MutRectangle indicatorRectangle = rectangle;
      indicatorRectangle.Width = width - 4;
      indicatorRectangle.Height = width - 4;
      indicatorRectangle.X += 2;
      indicatorRectangle.Y += 2;

      MutRectangle textRectangle = rectangle;
      textRectangle.X = width;
      textRectangle.Width -= width;

      return (indicatorRectangle, textRectangle);
   }

   protected void drawRectangle(Graphics g, Rectangle rectangle, Color color)
   {
      using var pen = new Pen(color);
      g.DrawRectangle(pen, rectangle);
   }

   protected void fillRectangle(Graphics g, Rectangle rectangle, Color color)
   {
      using var brush = new SolidBrush(color);
      g.FillRectangle(brush, rectangle);
   }

   protected void drawX(Graphics g, Rectangle rectangle, Color color)
   {
      using var pen = new Pen(color);
      MutPoint p1 = rectangle.Location;
      MutPoint p2 = new Point(rectangle.Right, rectangle.Bottom);
      g.DrawLine(pen, p1, p2);

      p1.X += rectangle.Width;
      p2.X -= rectangle.Width;
      g.DrawLine(pen, p1, p2);
   }

   public void OnPaint(Graphics g)
   {
      var writer = new UiActionWriter(CardinalAlignment.Center, true, nil, nil, UiActionButtonType.Normal)
      {
         Font = uiAction.Font
      };
      foreach (var (index, rectangle) in uiAction.Rectangles.Indexed())
      {
         var (indicatorRectangle, textRectangle) = splitRectangle(rectangle);
         var color = index == selectedIndex ? Color.Red : Color.Black;
         drawRectangle(g, indicatorRectangle, Color.Black);
         if (index == selectedIndex)
         {
            drawX(g, indicatorRectangle, color);
         }

         drawRectangle(g, textRectangle, color);
         writer.Rectangle = textRectangle;
         writer.Write(alternates[index], g);
      }
   }

   public void OnPaintBackground(Graphics g)
   {
      fillRectangle(g, uiAction.ClientRectangle, Color.AliceBlue);
   }
}