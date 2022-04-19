using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Core.Collections;
using Core.Strings;
using Core.WinForms.ControlWrappers;

namespace Core.WinForms.Controls
{
   public class MessageProgress : UserControl
   {
      protected static Hash<MessageLabelType, Color> globalForeColors;
      protected static Hash<MessageLabelType, Color> globalBackColors;
      protected static Hash<MessageLabelType, MessageStyle> globalStyles;

      static MessageProgress()
      {
         globalForeColors = new Hash<MessageLabelType, Color>
         {
            [MessageLabelType.Uninitialized] = Color.White,
            [MessageLabelType.Message] = Color.White,
            [MessageLabelType.Exception] = Color.White,
            [MessageLabelType.Success] = Color.White,
            [MessageLabelType.Failure] = Color.Black,
            [MessageLabelType.Selected] = Color.White,
            [MessageLabelType.Unselected] = Color.White,
            [MessageLabelType.ProgressIndefinite] = Color.White,
            [MessageLabelType.ProgressDefinite] = Color.Black
         };
         globalBackColors = new Hash<MessageLabelType, Color>
         {
            [MessageLabelType.Uninitialized] = Color.Gray,
            [MessageLabelType.Message] = Color.Blue,
            [MessageLabelType.Exception] = Color.Red,
            [MessageLabelType.Success] = Color.Green,
            [MessageLabelType.Failure] = Color.Gold,
            [MessageLabelType.Selected] = Color.FromArgb(0, 127, 0),
            [MessageLabelType.Unselected] = Color.FromArgb(127, 0, 0)
         };
         globalStyles = new Hash<MessageLabelType, MessageStyle>
         {
            [MessageLabelType.Uninitialized] = MessageStyle.Italic,
            [MessageLabelType.Message] = MessageStyle.None,
            [MessageLabelType.Exception] = MessageStyle.Bold,
            [MessageLabelType.Success] = MessageStyle.Bold,
            [MessageLabelType.Failure] = MessageStyle.Bold
         };
      }

      public static Hash<MessageLabelType, Color> GlobalForeColors => globalForeColors;

      public static Hash<MessageLabelType, Color> GlobalBackColors => globalBackColors;

      public static Hash<MessageLabelType, MessageStyle> GlobalStyles => globalStyles;

      protected Font italicFont;
      protected Font boldFont;
      protected AutoHash<MessageLabelType, Color> foreColors;
      protected AutoHash<MessageLabelType, Color> backColors;
      protected AutoHash<MessageLabelType, MessageStyle> styles;
      protected string text;
      protected MessageLabelType type;
      protected Random random;
      protected int value;
      protected Timer timer;

      public MessageProgress()
      {
         italicFont = new Font(base.Font, FontStyle.Italic);
         boldFont = new Font(base.Font, FontStyle.Bold);

         Center = false;
         Is3D = true;

         foreColors = new AutoHash<MessageLabelType, Color>(mlt => globalForeColors[mlt]);
         backColors = new AutoHash<MessageLabelType, Color>(mlt => globalBackColors[mlt]);
         styles = new AutoHash<MessageLabelType, MessageStyle>(mlt => globalStyles[mlt]);
         text = string.Empty;
         type = MessageLabelType.Uninitialized;

         SetStyle(ControlStyles.UserPaint, true);

         random = new Random();

         timer = new Timer
         {
            Interval = 100,
            Enabled = false
         };
         timer.Tick += (_, _) => this.Do(Refresh);
      }

      public void SetUp(int x, int y, int width, int height, AnchorStyles anchor)
      {
         Location = new Point(x, y);
         Size = new Size(width, height);
         Anchor = anchor;
      }

      public override Font Font
      {
         get => base.Font;
         set
         {
            base.Font = value;
            italicFont = new Font(base.Font, FontStyle.Italic);
            boldFont = new Font(base.Font, FontStyle.Bold);
         }
      }

      public override string Text
      {
         get => text;
         set => text = value;
      }

      public bool Center { get; set; }

      public bool Is3D { get; set; }

      public AutoHash<MessageLabelType, Color> ForeColors => foreColors;

      public AutoHash<MessageLabelType, Color> BackColors => backColors;

      public AutoHash<MessageLabelType, MessageStyle> Styles => styles;

      protected Font getFont(MessageLabelType type) => styles[type] switch
      {
         MessageStyle.None => Font,
         MessageStyle.Italic => italicFont,
         MessageStyle.Bold => boldFont,
         _ => Font
      };

      protected void refresh()
      {
         this.Do(() =>
         {
            Invalidate();
            Update();
         });
      }

      public void ShowMessage(string message, MessageLabelType type)
      {
         Busy(false);
         text = message;
         this.type = type;
         refresh();
      }

      public void Uninitialized(string message) => ShowMessage(message, MessageLabelType.Uninitialized);

      public void Message(string message) => ShowMessage(message, MessageLabelType.Message);

      public void Exception(Exception exception) => ShowMessage(exception.Message, MessageLabelType.Exception);

      public void Success(string message) => ShowMessage(message, MessageLabelType.Success);

      public void Failure(string message) => ShowMessage(message, MessageLabelType.Failure);

      public void Selected(string message) => ShowMessage(message, MessageLabelType.Selected);

      public void Unselected(string message) => ShowMessage(message, MessageLabelType.Unselected);

      public void Tape()
      {
         type = MessageLabelType.Tape;
         refresh();
      }

      public void ProgressText(string text)
      {
         this.text = text;
         type = MessageLabelType.ProgressIndefinite;

         refresh();
      }

      public int Minimum { get; set; }

      public int Maximum { get; set; }

      public void Progress(int value, string text = "")
      {
         this.value = value;
         this.text = text;

         type = MessageLabelType.ProgressDefinite;

         refresh();
      }

      public void AddToControls(ControlCollection controls, bool fill = true)
      {
         Font = new Font("Consolas", 12);
         if (fill)
         {
            Dock = DockStyle.Fill;
         }

         controls.Add(this);
      }

      protected void writeText(Graphics graphics, string text, bool center)
      {
         var font = getFont(type);
         var foreColor = foreColors[type];
         var flags = TextFormatFlags.EndEllipsis;
         if (center)
         {
            flags |= TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
         }
         else
         {
            flags |= TextFormatFlags.VerticalCenter | TextFormatFlags.LeftAndRightPadding;
         }

         TextRenderer.DrawText(graphics, text, font, ClientRectangle, foreColor, flags);
      }

      protected void writeText(Graphics graphics) => writeText(graphics, text, Center);

      protected override void OnPaint(PaintEventArgs e)
      {
         base.OnPaint(e);

         switch (type)
         {
            case MessageLabelType.ProgressIndefinite:
               writeText(e.Graphics);
               break;
            case MessageLabelType.ProgressDefinite:
            {
               var finalText = text;
               if (finalText.IsEmpty())
               {
                  finalText = $"{getPercentage()}%";
               }
               else
               {
                  finalText = $"{finalText} ({getPercentage()}%)";
               }

               writeText(e.Graphics, finalText, true);
               break;
            }
            default:
            {
               if (type != MessageLabelType.Tape)
               {
                  writeText(e.Graphics);
               }

               break;
            }
         }
      }

      protected override void OnPaintBackground(PaintEventArgs pevent)
      {
         base.OnPaintBackground(pevent);

         switch (type)
         {
            case MessageLabelType.Tape:
            {
               using var brush = new HatchBrush(HatchStyle.BackwardDiagonal, Color.Black, Color.Gold);
               pevent.Graphics.FillRectangle(brush, ClientRectangle);
               break;
            }
            case MessageLabelType.ProgressIndefinite or MessageLabelType.Busy:
            {
               var x = random.Next(ClientRectangle.Width - 5);
               var y = random.Next(ClientRectangle.Height - 5);
               var location = new Point(x, y);
               var length = random.Next(ClientRectangle.Width - x - 5);
               using var brush = new SolidBrush(Color.DarkSlateGray);
               pevent.Graphics.FillRectangle(brush, ClientRectangle);

               var endPoint = location;
               endPoint.Offset(length, 0);

               var location2 = location;
               location2.Offset(1, 1);
               var endPoint2 = endPoint;
               endPoint2.Offset(1, 1);

               using var whitePen = new Pen(Color.White, 5);
               pevent.Graphics.DrawLine(whitePen, location2, endPoint2);
               using var greenPen = new Pen(Color.Green, 5);
               pevent.Graphics.DrawLine(greenPen, location, endPoint);
               break;
            }
            case MessageLabelType.ProgressDefinite:
            {
               using var whiteBrush = new SolidBrush(Color.White);
               pevent.Graphics.FillRectangle(whiteBrush, ClientRectangle);
               var width = ClientRectangle.Width;
               var percentWidth = getPercentage(width);
               var location = ClientRectangle.Location;
               var size = new Size(percentWidth, ClientRectangle.Height);
               var rectangle = new Rectangle(location, size);
               using var greenBrush = new SolidBrush(Color.CornflowerBlue);
               pevent.Graphics.FillRectangle(greenBrush, rectangle);
               break;
            }
            default:
            {
               var backColor = backColors[type];
               using var brush = new SolidBrush(backColor);
               pevent.Graphics.FillRectangle(brush, ClientRectangle);
               break;
            }
         }

         if (Is3D)
         {
            using var darkGrayPen = new Pen(Color.DarkGray, 1);
            using var lightPen = new Pen(Color.White, 1);

            var left = ClientRectangle.Left;
            var top = ClientRectangle.Top;
            var width = ClientRectangle.Width - 1;
            var height = ClientRectangle.Height - 1;

            pevent.Graphics.DrawLine(darkGrayPen, new Point(left, top), new Point(width, top));
            pevent.Graphics.DrawLine(darkGrayPen, new Point(left, top), new Point(left, height));
            pevent.Graphics.DrawLine(lightPen, new Point(left, height), new Point(width, height));
            pevent.Graphics.DrawLine(lightPen, new Point(width, top), new Point(width, height));
         }
      }

      protected int getPercentage() => (int)((float)value / Maximum * 100);

      protected int getPercentage(int width) => (int)((float)value / Maximum * width);

      public void Busy(bool enabled)
      {
         text = "";
         type = MessageLabelType.Busy;
         this.Do(() => timer.Enabled = enabled);
      }
   }
}