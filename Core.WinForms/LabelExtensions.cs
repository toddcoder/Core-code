using System;
using System.Drawing;
using System.Windows.Forms;

namespace Core.WinForms
{
   public static class LabelExtensions
   {
      public static void ShowMessage(this Label label, string message, Color foreColor, Color backColor)
      {
         label.ForeColor = foreColor;
         label.BackColor = backColor;
         label.Text = message;
      }

      public static void ShowMessage(this Label label, string message) => label.ShowMessage(message, Color.White, Color.Blue);

      public static void ShowException(this Label label, Exception exception) => label.ShowMessage(exception.Message, Color.White, Color.Red);

      public static void ShowSuccess(this Label label, string message) => label.ShowMessage(message, Color.White, Color.Green);

      public static void ShowFailure(this Label label, string message) => label.ShowMessage(message, Color.Black, Color.Yellow);

      public static void ShowBusy(this Label label, string message) => label.ShowMessage(message, Color.Red, Color.Gold);

      public static void ShowUninitialized(this Label label, string message) => label.ShowMessage(message, Color.DarkGray, Color.Gray);
   }
}