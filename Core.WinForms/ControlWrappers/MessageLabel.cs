using System;
using System.Drawing;
using System.Windows.Forms;
using Core.Applications;
using Core.Collections;
using Core.WinForms.Controls;

namespace Core.WinForms.ControlWrappers
{
   [Obsolete("Use MessageProgress")]
   public class MessageLabel
   {
      protected static Hash<MessageProgressType, Color> globalForeColors;
      protected static Hash<MessageProgressType, Color> globalBackColors;
      protected static Hash<MessageProgressType, MessageStyle> globalStyles;

      static MessageLabel()
      {
         globalForeColors = new Hash<MessageProgressType, Color>
         {
            [MessageProgressType.Uninitialized] = Color.White,
            [MessageProgressType.Message] = Color.White,
            [MessageProgressType.Exception] = Color.White,
            [MessageProgressType.Success] = Color.White,
            [MessageProgressType.Failure] = Color.Black,
            [MessageProgressType.Busy] = Color.Black,
            [MessageProgressType.Selected] = Color.White,
            [MessageProgressType.Unselected] = Color.White
         };
         globalBackColors = new Hash<MessageProgressType, Color>
         {
            [MessageProgressType.Uninitialized] = Color.Gray,
            [MessageProgressType.Message] = Color.Blue,
            [MessageProgressType.Exception] = Color.Red,
            [MessageProgressType.Success] = Color.Green,
            [MessageProgressType.Failure] = Color.Gold,
            [MessageProgressType.Busy] = Color.White,
            [MessageProgressType.Selected] = Color.FromArgb(0, 127, 0),
            [MessageProgressType.Unselected] = Color.FromArgb(127, 0, 0)
         };
         globalStyles = new Hash<MessageProgressType, MessageStyle>
         {
            [MessageProgressType.Uninitialized] = MessageStyle.Italic,
            [MessageProgressType.Message] = MessageStyle.None,
            [MessageProgressType.Exception] = MessageStyle.Bold,
            [MessageProgressType.Success] = MessageStyle.Bold,
            [MessageProgressType.Failure] = MessageStyle.Bold,
            [MessageProgressType.Busy] = MessageStyle.Italic
         };
      }

      public static Hash<MessageProgressType, Color> GlobalForeColors => globalForeColors;

      public static Hash<MessageProgressType, Color> GlobalBackColors => globalBackColors;

      public static Hash<MessageProgressType, MessageStyle> GlobalStyles => globalStyles;

      protected Label labelMessage;
      protected Font font;
      protected Font italicFont;
      protected Font boldFont;
      protected AutoHash<MessageProgressType, Color> foreColors;
      protected AutoHash<MessageProgressType, Color> backColors;
      protected AutoHash<MessageProgressType, MessageStyle> styles;

      public MessageLabel(Label labelMessage)
      {
         this.labelMessage = labelMessage;
         this.labelMessage.AutoEllipsis = true;
         this.labelMessage.AutoSize = false;

         Font = new Font("Consolas", 12f);
         Center = false;
         Is3D = true;

         foreColors = new AutoHash<MessageProgressType, Color>(mlt => globalForeColors[mlt]);
         backColors = new AutoHash<MessageProgressType, Color>(mlt => globalBackColors[mlt]);
         styles = new AutoHash<MessageProgressType, MessageStyle>(mlt => globalStyles[mlt]);
      }

      public Font Font
      {
         get => font;
         set
         {
            font = value;
            italicFont = new Font(font, FontStyle.Italic);
            boldFont = new Font(font, FontStyle.Bold);
         }
      }

      public bool Center
      {
         get => labelMessage.TextAlign == ContentAlignment.MiddleCenter;
         set => labelMessage.TextAlign = value ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleLeft;
      }

      public bool Is3D
      {
         get => labelMessage.BorderStyle == BorderStyle.Fixed3D;
         set => labelMessage.BorderStyle = value ? BorderStyle.Fixed3D : BorderStyle.None;
      }

      public AutoHash<MessageProgressType, Color> ForeColors => foreColors;

      public AutoHash<MessageProgressType, Color> BackColors => backColors;

      public AutoHash<MessageProgressType, MessageStyle> Styles => styles;

      public Label Label => labelMessage;

      protected Font getFont(MessageProgressType type) => styles[type] switch
      {
         MessageStyle.None => font,
         MessageStyle.Italic => italicFont,
         MessageStyle.Bold => boldFont,
         _ => font
      };

      public void ShowMessage(string message, MessageProgressType type)
      {
         labelMessage.Do(() =>
         {
            labelMessage.Image = null;
            labelMessage.Font = getFont(type);
            labelMessage.ForeColor = foreColors[type];
            labelMessage.BackColor = backColors[type];
            labelMessage.Text = message;
         });
      }

      public void Uninitialized(string message) => ShowMessage(message, MessageProgressType.Uninitialized);

      public void Message(string message) => ShowMessage(message, MessageProgressType.Message);

      public void Exception(Exception exception) => ShowMessage(exception.Message, MessageProgressType.Exception);

      public void Success(string message) => ShowMessage(message, MessageProgressType.Success);

      public void Failure(string message) => ShowMessage(message, MessageProgressType.Failure);

      public void Busy(string message) => ShowMessage(message, MessageProgressType.Busy);

      public void Selected(string message) => ShowMessage(message, MessageProgressType.Selected);

      public void Unselected(string message) => ShowMessage(message, MessageProgressType.Unselected);

      public void Tape()
      {
         labelMessage.Do(() =>
         {
            labelMessage.Text = "";
            var resources = new Resources<MessageLabel>();
            using var stream = resources.Stream("tape.png");
            var image = Image.FromStream(stream);
            labelMessage.Image = image;
            labelMessage.ImageAlign = ContentAlignment.MiddleCenter;
         });
      }

      public void Untape(string message, MessageProgressType type)
      {
         labelMessage.Do(() => labelMessage.Image = null);
         ShowMessage(message, type);
      }
   }
}