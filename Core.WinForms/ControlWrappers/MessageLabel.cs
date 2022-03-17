using System;
using System.Drawing;
using System.Windows.Forms;

namespace Core.WinForms.ControlWrappers
{
   public class MessageLabel
   {
      protected Label labelMessage;
      protected Font font;
      protected Font italicFont;
      protected Font boldFont;

      public MessageLabel(Label labelMessage, bool center = false, bool is3d = true)
      {
         this.labelMessage = labelMessage;
         this.labelMessage.AutoEllipsis = true;
         this.labelMessage.AutoSize = false;

         font = new Font("Consolas", 12f);
         italicFont = new Font(font, FontStyle.Italic);
         boldFont = new Font(font, FontStyle.Bold);

         this.labelMessage.TextAlign = center ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleLeft;
         if (is3d)
         {
            this.labelMessage.BorderStyle = BorderStyle.Fixed3D;
         }
      }

      public void ShowMessage(string message, Color foreColor, Color backColor, Font font)
      {
         labelMessage.Do(() =>
         {
            labelMessage.Font = font;
            labelMessage.ForeColor = foreColor;
            labelMessage.BackColor = backColor;
            labelMessage.Text = message;
         });
      }

      public void Uninitialized(string message) => ShowMessage(message, Color.White, Color.Gray, italicFont);

      public void Message(string message) => ShowMessage(message, Color.White, Color.Blue, font);

      public void Exception(Exception exception) => ShowMessage(exception.Message, Color.White, Color.Red, boldFont);

      public void Success(string message) => ShowMessage(message, Color.White, Color.Green, boldFont);

      public void Failure(string message) => ShowMessage(message, Color.Black, Color.Gold, boldFont);

      public void Busy(string message) => ShowMessage(message, Color.Black, Color.White, italicFont);
   }
}