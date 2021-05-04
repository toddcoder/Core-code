using System;
using System.Drawing;
using System.Windows.Forms;

namespace Core.WinForms.Notification
{
   public partial class NotifierHost : Form
   {
      protected int x;
      protected Image iconImage;

      public NotifierHost(int duration, string title, string text, object icon, Color leftColor, Color rightColor, Color titleColor, Color textColor)
      {
         InitializeComponent();

         AutoScaleDimensions = new SizeF(6f, 13f);
         AutoScaleMode = AutoScaleMode.Font;
         panelRight.AutoSize = true;
         panelRight.AutoSizeMode = AutoSizeMode.GrowOnly;

         panelLeft.BackColor = leftColor;
         panelRight.BackColor = rightColor;
         labelTitle.Text = title;
         labelMessage.Text = text;
         iconImage = icon switch
         {
            Image image => image,
            string name => icons.Images[name],
            _ => iconImage
         };

         panelLeft.Image = iconImage;

         labelTitle.ForeColor = titleColor;
         labelMessage.ForeColor = textColor;
         ShowInTaskbar = false;
         timerCloser.Interval = duration;

         x = 1;
      }

      protected void timerCloser_Tick(object sender, EventArgs e)
      {
         try
         {
            Capture = true;
            var mousePosition = PointToClient(MousePosition);
            if (!ClientRectangle.Contains(mousePosition))
            {
               Close();
            }
         }
         finally
         {
            Capture = false;
         }
      }

      protected void NotifierHost_Load(object sender, EventArgs e)
      {
         var labelWidth = labelMessage.Size.Width;
         if (labelWidth > panelRight.Width)
         {
            labelWidth += 32;
            Width = panelLeft.Width + labelWidth;
            panelRight.Width = labelWidth;
         }

         timerStyler.Start();
         timerCloser.Start();
      }

      protected void timerStyler_Tick(object sender, EventArgs e)
      {
         x += 20;
         var workingArea = Screen.PrimaryScreen.WorkingArea;
         Location = new Point(workingArea.Right - x, workingArea.Bottom - Size.Height - 30);
         if (Location.X == workingArea.Right - Size.Width || Location.X < workingArea.Right - Size.Width)
         {
            timerStyler.Stop();
         }
      }
   }
}