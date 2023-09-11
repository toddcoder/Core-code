using Core.Monads;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace Core.WinForms.Controls;

public class DeletableWriter : AlternateWriter
{
   protected const int DELETABLE_SIZE = 8;
   protected const int DELETABLE_MARGIN = 2;
   protected const int FULL_DELETABLE_SIZE = DELETABLE_SIZE + DELETABLE_MARGIN;

   protected Rectangle[] deletableRectangles;

   public DeletableWriter(UiAction uiAction, string[] alternates, bool autoSizeText, Maybe<int> _floor, Maybe<int> _ceiling) :
      base(uiAction, alternates, autoSizeText, _floor, _ceiling)
   {
      deletableRectangles = Enumerable.Range(0, alternates.Length).Select(_ => Rectangle.Empty).ToArray();
   }

   protected override (int penSize, Rectangle textRectangle, Rectangle smallRectangle) splitRectangle(Rectangle rectangle)
   {
      var penSize = getPenSize(rectangle);

      var textRectangle = rectangle;
      var deletableRectangle = new Rectangle(rectangle.X + (rectangle.Width - FULL_DELETABLE_SIZE) - 1, rectangle.Y + DELETABLE_MARGIN,
         FULL_DELETABLE_SIZE, FULL_DELETABLE_SIZE);

      return (penSize, textRectangle, deletableRectangle);
   }

   public Rectangle[] DeletableRectangles => deletableRectangles;

   protected void drawDeletable(Graphics g, Rectangle rectangle, Color foreColor, bool enabled)
   {
      if (enabled)
      {
         using var pen = new Pen(foreColor, 1);
         pen.StartCap = LineCap.Triangle;
         pen.EndCap = LineCap.Triangle;
         g.DrawLine(pen, rectangle.NorthWest(DELETABLE_MARGIN), rectangle.SouthEast(DELETABLE_MARGIN));
         g.DrawLine(pen, rectangle.NorthEast(DELETABLE_MARGIN), rectangle.SouthWest(DELETABLE_MARGIN));
      }
   }

   public void DrawBoldDeletable(Graphics g, int index)
   {
      var rectangle = deletableRectangles[index];
      var foreColor = GetAlternateForeColor(index);
      using var pen = new Pen(foreColor, 2);
      pen.StartCap = LineCap.Triangle;
      pen.EndCap = LineCap.Triangle;
      g.DrawLine(pen, rectangle.NorthWest(DELETABLE_MARGIN), rectangle.SouthEast(DELETABLE_MARGIN));
      g.DrawLine(pen, rectangle.NorthEast(DELETABLE_MARGIN), rectangle.SouthWest(DELETABLE_MARGIN));
   }

   protected override void onPaint(Graphics g, int index, Rectangle rectangle, UiActionWriter writer, string alternate)
   {
      var (_, textRectangle, smallRectangle) = splitRectangle(rectangle);
      deletableRectangles[index] = smallRectangle;

      if (deletableRectangles.Length > 0)
      {
         writer.Font = uiAction.Font;
         var foreColor = GetAlternateForeColor(index);
         writer.Color = foreColor;
         var backColor = GetAlternateBackColor(index);
         fillRectangle(g, rectangle, backColor);

         writer.Rectangle = textRectangle;
         writer.Write(g, alternate);

         drawDeletable(g, smallRectangle, foreColor, uiAction.Enabled);
      }
      else
      {
         writer.Font = disabledFont.Value;
         var foreColor = Color.Black;
         var backColor = Color.LightGray;
         fillRectangle(g, rectangle, backColor);

         if (uiAction.EmptyTextTitle is (true, var emptyTextTitle))
         {
            writer.Color = foreColor;
            writer.Rectangle = textRectangle;
            writer.Write(g, emptyTextTitle);
         }
      }
   }
}