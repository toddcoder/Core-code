using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Core.WinForms.Notification
{
   public class ImagePanel : Panel
   {
      protected Image image;

      public ImagePanel()
      {
         SetStyle(ControlStyles.SupportsTransparentBackColor, true);
         SetStyle(ControlStyles.UserPaint, true);
      }

      public Image Image
      {
         get => image;
         set
         {
            image = value;
            RecreateHandle();
         }
      }

      protected override void OnPaintBackground(PaintEventArgs e)
      {
      }

      protected override CreateParams CreateParams
      {
         get
         {
            var cp = base.CreateParams;
            cp.ExStyle |= 0x20;

            return cp;
         }
      }

      protected override void OnPaint(PaintEventArgs e)
      {
         if (image != null)
         {
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;

            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.CompositingMode = CompositingMode.SourceOver;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            using (var brush = new SolidBrush(BackColor))
            {
               e.Graphics.FillRectangle(brush, ClientRectangle);
            }

            var left = (Width - image.Width) / 2;
            var top = (Height - image.Height) / 2;
            e.Graphics.DrawImage(image, left, top, image.Width, image.Height);
         }
      }
   }
}