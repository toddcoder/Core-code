using System;
using System.Drawing;
using System.Windows.Forms;

namespace Core.WinForms
{
   public static class LabelExtensions
   {
      public static void ShowMessage(this Label label, string message)
      {
         label.ForeColor = SystemColors.InfoText;
         label.BackColor = SystemColors.Info;
         label.Text = message;
      }

      public static void ShowException(this Label label, Exception exception)
      {
         label.ForeColor = Color.White;
         label.BackColor = Color.Red;
         label.Text = exception.Message;
      }

      public static void ShowSuccess(this Label label, string message)
      {
         label.ForeColor = Color.White;
         label.BackColor = Color.Green;
         label.Text = message;
      }

      public static void ShowFailure(this Label label, string message)
      {
         label.ForeColor = Color.Black;
         label.BackColor = Color.Yellow;
         label.Text = message;
      }
   }
}