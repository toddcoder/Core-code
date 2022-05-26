using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Core.WinForms.Controls
{
   public class RoundedMessage : MessageProgress
   {
      public int CornerRadius { get; set; }

      protected GraphicsPath roundedRectangle(Rectangle rectangle, int radius)
      {
         var diameter = radius * 2;
         var size = new Size(diameter, diameter);
         var arc = new Rectangle(rectangle.Location, size);
         var path = new GraphicsPath();

         if (radius == 0)
         {
            path.AddRectangle(rectangle);
            return path;
         }

         path.AddArc(arc, 180, 90);

         arc.X = rectangle.Right - diameter;
         path.AddArc(arc, 270, 90);

         arc.Y = rectangle.Bottom - diameter;
         path.AddArc(arc, 0, 90);

         arc.X = rectangle.Left;
         path.AddArc(arc, 90, 90);

         path.CloseFigure();
         return path;
      }

      public RoundedMessage(Form form, bool center = false) : base(form, center, false)
      {
         CornerRadius = 8;
      }

      protected override void drawRectangle(Graphics graphics, Pen pen, Rectangle rectangle)
      {
         using var path = roundedRectangle(rectangle, CornerRadius);
         graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
         graphics.SmoothingMode = SmoothingMode.AntiAlias;
         graphics.DrawPath(pen, path);
      }

      protected override void fillRectangle(Graphics graphics, Brush brush, Rectangle rectangle)
      {
         using var path = roundedRectangle(rectangle, CornerRadius);
         graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
         graphics.SmoothingMode = SmoothingMode.AntiAlias;
         graphics.FillPath(brush, path);
      }
   }
}