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
      protected static Hash<UiActionType, Color> globalForeColors;
      protected static Hash<UiActionType, Color> globalBackColors;
      protected static Hash<UiActionType, MessageStyle> globalStyles;

      static MessageLabel()
      {
         globalForeColors = new Hash<UiActionType, Color>
         {
            [UiActionType.Uninitialized] = Color.White,
            [UiActionType.Message] = Color.White,
            [UiActionType.Exception] = Color.White,
            [UiActionType.Success] = Color.White,
            [UiActionType.Failure] = Color.Black,
            [UiActionType.Busy] = Color.Black,
            [UiActionType.Selected] = Color.White,
            [UiActionType.Unselected] = Color.White
         };
         globalBackColors = new Hash<UiActionType, Color>
         {
            [UiActionType.Uninitialized] = Color.Gray,
            [UiActionType.Message] = Color.Blue,
            [UiActionType.Exception] = Color.Red,
            [UiActionType.Success] = Color.Green,
            [UiActionType.Failure] = Color.Gold,
            [UiActionType.Busy] = Color.White,
            [UiActionType.Selected] = Color.FromArgb(0, 127, 0),
            [UiActionType.Unselected] = Color.FromArgb(127, 0, 0)
         };
         globalStyles = new Hash<UiActionType, MessageStyle>
         {
            [UiActionType.Uninitialized] = MessageStyle.Italic,
            [UiActionType.Message] = MessageStyle.None,
            [UiActionType.Exception] = MessageStyle.Bold,
            [UiActionType.Success] = MessageStyle.Bold,
            [UiActionType.Failure] = MessageStyle.Bold,
            [UiActionType.Busy] = MessageStyle.Italic
         };
      }

      public static Hash<UiActionType, Color> GlobalForeColors => globalForeColors;

      public static Hash<UiActionType, Color> GlobalBackColors => globalBackColors;

      public static Hash<UiActionType, MessageStyle> GlobalStyles => globalStyles;

      protected Label labelMessage;
      protected Font font;
      protected Font italicFont;
      protected Font boldFont;
      protected AutoHash<UiActionType, Color> foreColors;
      protected AutoHash<UiActionType, Color> backColors;
      protected AutoHash<UiActionType, MessageStyle> styles;

      public MessageLabel(Label labelMessage)
      {
         this.labelMessage = labelMessage;
         this.labelMessage.AutoEllipsis = true;
         this.labelMessage.AutoSize = false;

         Font = new Font("Consolas", 12f);
         Center = false;
         Is3D = true;

         foreColors = new AutoHash<UiActionType, Color>(mlt => globalForeColors[mlt]);
         backColors = new AutoHash<UiActionType, Color>(mlt => globalBackColors[mlt]);
         styles = new AutoHash<UiActionType, MessageStyle>(mlt => globalStyles[mlt]);
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

      public AutoHash<UiActionType, Color> ForeColors => foreColors;

      public AutoHash<UiActionType, Color> BackColors => backColors;

      public AutoHash<UiActionType, MessageStyle> Styles => styles;

      public Label Label => labelMessage;

      protected Font getFont(UiActionType type) => styles[type] switch
      {
         MessageStyle.None => font,
         MessageStyle.Italic => italicFont,
         MessageStyle.Bold => boldFont,
         _ => font
      };

      public void ShowMessage(string message, UiActionType type)
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

      public void Uninitialized(string message) => ShowMessage(message, UiActionType.Uninitialized);

      public void Message(string message) => ShowMessage(message, UiActionType.Message);

      public void Exception(Exception exception) => ShowMessage(exception.Message, UiActionType.Exception);

      public void Success(string message) => ShowMessage(message, UiActionType.Success);

      public void Failure(string message) => ShowMessage(message, UiActionType.Failure);

      public void Busy(string message) => ShowMessage(message, UiActionType.Busy);

      public void Selected(string message) => ShowMessage(message, UiActionType.Selected);

      public void Unselected(string message) => ShowMessage(message, UiActionType.Unselected);

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

      public void Untape(string message, UiActionType type)
      {
         labelMessage.Do(() => labelMessage.Image = null);
         ShowMessage(message, type);
      }
   }
}