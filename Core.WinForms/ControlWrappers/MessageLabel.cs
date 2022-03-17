using System;
using System.Drawing;
using System.Windows.Forms;
using Core.Applications;
using Core.Collections;

namespace Core.WinForms.ControlWrappers
{
   public class MessageLabel
   {
      protected static Hash<MessageLabelType, Color> globalForeColors;
      protected static Hash<MessageLabelType, Color> globalBackColors;
      protected static Hash<MessageLabelType, MessageStyle> globalStyles;

      static MessageLabel()
      {
         globalForeColors = new Hash<MessageLabelType, Color>
         {
            [MessageLabelType.Uninitialized] = Color.White,
            [MessageLabelType.Message] = Color.White,
            [MessageLabelType.Exception] = Color.White,
            [MessageLabelType.Success] = Color.White,
            [MessageLabelType.Failure] = Color.Black,
            [MessageLabelType.Busy] = Color.Black
         };
         globalBackColors = new Hash<MessageLabelType, Color>
         {
            [MessageLabelType.Uninitialized] = Color.Gray,
            [MessageLabelType.Message] = Color.Blue,
            [MessageLabelType.Exception] = Color.Red,
            [MessageLabelType.Success] = Color.Green,
            [MessageLabelType.Failure] = Color.Gold,
            [MessageLabelType.Busy] = Color.White
         };
         globalStyles = new Hash<MessageLabelType, MessageStyle>
         {
            [MessageLabelType.Uninitialized] = MessageStyle.Italic,
            [MessageLabelType.Message] = MessageStyle.None,
            [MessageLabelType.Exception] = MessageStyle.Bold,
            [MessageLabelType.Success] = MessageStyle.Bold,
            [MessageLabelType.Failure] = MessageStyle.Bold,
            [MessageLabelType.Busy] = MessageStyle.Italic
         };
      }

      public static Hash<MessageLabelType, Color> GlobalForeColors => globalForeColors;

      public static Hash<MessageLabelType, Color> GlobalBackColors => globalBackColors;

      public static Hash<MessageLabelType, MessageStyle> GlobalStyles => globalStyles;

      protected Label labelMessage;
      protected Font font;
      protected Font italicFont;
      protected Font boldFont;
      protected AutoHash<MessageLabelType, Color> foreColors;
      protected AutoHash<MessageLabelType, Color> backColors;
      protected AutoHash<MessageLabelType, MessageStyle> styles;

      public MessageLabel(Label labelMessage)
      {
         this.labelMessage = labelMessage;
         this.labelMessage.AutoEllipsis = true;
         this.labelMessage.AutoSize = false;

         Font = new Font("Consolas", 12f);
         Center = false;
         Is3D = true;

         foreColors = new AutoHash<MessageLabelType, Color>(mlt => globalForeColors[mlt]);
         backColors = new AutoHash<MessageLabelType, Color>(mlt => globalBackColors[mlt]);
         styles = new AutoHash<MessageLabelType, MessageStyle>(mlt => globalStyles[mlt]);
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

      public AutoHash<MessageLabelType, Color> ForeColors => foreColors;

      public AutoHash<MessageLabelType, Color> BackColors => backColors;

      public AutoHash<MessageLabelType, MessageStyle> Styles => styles;

      protected Font getFont(MessageLabelType type) => styles[type] switch
      {
         MessageStyle.None => font,
         MessageStyle.Italic => italicFont,
         MessageStyle.Bold => boldFont,
         _ => font
      };

      public void ShowMessage(string message, MessageLabelType type)
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

      public void Uninitialized(string message) => ShowMessage(message, MessageLabelType.Uninitialized);

      public void Message(string message) => ShowMessage(message, MessageLabelType.Message);

      public void Exception(Exception exception) => ShowMessage(exception.Message, MessageLabelType.Exception);

      public void Success(string message) => ShowMessage(message, MessageLabelType.Success);

      public void Failure(string message) => ShowMessage(message, MessageLabelType.Failure);

      public void Busy(string message) => ShowMessage(message, MessageLabelType.Busy);

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

      public void Untape(string message, MessageLabelType type)
      {
         labelMessage.Do(() => labelMessage.Image = null);
         ShowMessage(message, type);
      }
   }
}