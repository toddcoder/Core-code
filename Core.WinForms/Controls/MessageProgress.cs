using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Core.Collections;
using Core.Monads;
using Core.Strings;
using Core.WinForms.ControlWrappers;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls
{
   public class MessageProgress : UserControl
   {
      protected static Hash<MessageProgressType, Color> globalForeColors;
      protected static Hash<MessageProgressType, Color> globalBackColors;
      protected static Hash<MessageProgressType, MessageStyle> globalStyles;

      static MessageProgress()
      {
         globalForeColors = new Hash<MessageProgressType, Color>
         {
            [MessageProgressType.Uninitialized] = Color.White,
            [MessageProgressType.Message] = Color.White,
            [MessageProgressType.Exception] = Color.White,
            [MessageProgressType.Success] = Color.White,
            [MessageProgressType.Failure] = Color.Black,
            [MessageProgressType.Selected] = Color.White,
            [MessageProgressType.Unselected] = Color.White,
            [MessageProgressType.ProgressIndefinite] = Color.White,
            [MessageProgressType.ProgressDefinite] = Color.White,
            [MessageProgressType.BusyText] = Color.White
         };
         globalBackColors = new Hash<MessageProgressType, Color>
         {
            [MessageProgressType.Uninitialized] = Color.Gray,
            [MessageProgressType.Message] = Color.Blue,
            [MessageProgressType.Exception] = Color.Red,
            [MessageProgressType.Success] = Color.Green,
            [MessageProgressType.Failure] = Color.Gold,
            [MessageProgressType.Selected] = Color.FromArgb(0, 127, 0),
            [MessageProgressType.Unselected] = Color.FromArgb(127, 0, 0)
         };
         globalStyles = new Hash<MessageProgressType, MessageStyle>
         {
            [MessageProgressType.Uninitialized] = MessageStyle.Italic,
            [MessageProgressType.Message] = MessageStyle.None,
            [MessageProgressType.Exception] = MessageStyle.Bold,
            [MessageProgressType.Success] = MessageStyle.Bold,
            [MessageProgressType.Failure] = MessageStyle.Bold
         };
      }

      public static Hash<MessageProgressType, Color> GlobalForeColors => globalForeColors;

      public static Hash<MessageProgressType, Color> GlobalBackColors => globalBackColors;

      public static Hash<MessageProgressType, MessageStyle> GlobalStyles => globalStyles;

      protected Font italicFont;
      protected Font boldFont;
      protected AutoHash<MessageProgressType, Color> foreColors;
      protected AutoHash<MessageProgressType, Color> backColors;
      protected AutoHash<MessageProgressType, MessageStyle> styles;
      protected string text;
      protected MessageProgressType type;
      protected Random random;
      protected int value;
      protected Timer timer;
      protected int maximum;
      protected int index;
      protected bool mouseInside;
      protected bool mouseDown;
      protected ToolTip toolTip;
      protected Maybe<string> _clickText;
      protected BusyTextProcessor busyTextProcessor;

      public MessageProgress(Form form)
      {
         form.Controls.Add(this);

         italicFont = new Font(base.Font, FontStyle.Italic);
         boldFont = new Font(base.Font, FontStyle.Bold);

         Center = false;
         Is3D = true;

         foreColors = new AutoHash<MessageProgressType, Color>(mlt => globalForeColors[mlt]);
         backColors = new AutoHash<MessageProgressType, Color>(mlt => globalBackColors[mlt]);
         styles = new AutoHash<MessageProgressType, MessageStyle>(mlt => globalStyles[mlt]);
         text = string.Empty;
         _clickText = nil;
         type = MessageProgressType.Uninitialized;

         SetStyle(ControlStyles.UserPaint, true);
         SetStyle(ControlStyles.DoubleBuffer, true);
         SetStyle(ControlStyles.AllPaintingInWmPaint, true);

         random = new Random();

         timer = new Timer
         {
            Interval = 100,
            Enabled = false
         };
         timer.Tick += (_, _) =>
         {
            if (type == MessageProgressType.BusyText)
            {
               busyTextProcessor.OnTick();
            }

            this.Do(Refresh);
         };

         Minimum = 1;
         maximum = 0;

         toolTip = new ToolTip { IsBalloon = true };
         toolTip.SetToolTip(this, "");

         busyTextProcessor = new BusyTextProcessor(Color.White, ClientRectangle);
         Resize += (_, _) => busyTextProcessor = new BusyTextProcessor(Color.White, ClientRectangle);
      }

      public MessageProgress(Form form, IContainer container) : this(form)
      {
         container.Add(this);
      }

      protected void setUpFont(string fontName, float fontSize)
      {
         Font = new Font(fontName, fontSize);
      }

      protected void setUpCore(int x, int y, int width, int height, string fontName, float fontSize)
      {
         AutoSize = false;
         Location = new Point(x, y);
         Size = new Size(width, height);
         setUpFont(fontName, fontSize);
      }

      public void SetUp(int x, int y, int width, int height, AnchorStyles anchor, string fontName = "Consolas", float fontSize = 12f)
      {
         setUpCore(x, y, width, height, fontName, fontSize);
         Anchor = anchor;
      }

      public void SetUp(int x, int y, int width, int height, DockStyle dockStyle, string fontName = "Consolas", float fontSize = 12f)
      {
         setUpCore(x, y, width, height, fontName, fontSize);
         Dock = dockStyle;
      }

      [Obsolete("Use SetUpInTableLayoutPanel")]
      public void SetUpAsDockFill(string fontName = "Consolas", float fontSize = 12f)
      {
         Dock = DockStyle.Fill;
         setUpFont(fontName, fontSize);
      }

      public void SetUpInTableLayoutPanel(TableLayoutPanel tableLayoutPanel, int column, int row, int columnSpan = 1, int rowSpan = 1,
         string fontName = "Consolas", float fontSize = 12f, DockStyle dockStyle = DockStyle.Fill)
      {
         Dock = dockStyle;
         tableLayoutPanel.Controls.Add(this, column, row);

         if (columnSpan > 1)
         {
            tableLayoutPanel.SetColumnSpan(this, columnSpan);
         }

         if (rowSpan > 1)
         {
            tableLayoutPanel.SetRowSpan(this, rowSpan);
         }

         setUpFont(fontName, fontSize);
      }

      public void SetUp(int x, int y, int width, int height, string fontName = "Consolas", float fontSize = 12f)
      {
         setUpCore(x, y, width, height, fontName, fontSize);
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

      protected void setToolTip()
      {
         if (Clickable && ClickText.IsNotEmpty())
         {
            toolTip.SetToolTip(this, ClickText);
         }
         else
         {
            toolTip.SetToolTip(this, text);
         }
      }

      public override string Text
      {
         get => text;
         set
         {
            text = value;
            this.Do(setToolTip);
         }
      }

      public bool Center { get; set; }

      public bool Is3D { get; set; }

      public AutoHash<MessageProgressType, Color> ForeColors => foreColors;

      public AutoHash<MessageProgressType, Color> BackColors => backColors;

      public AutoHash<MessageProgressType, MessageStyle> Styles => styles;

      protected Font getFont(MessageProgressType type) => styles[type] switch
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

      public void ShowMessage(string message, MessageProgressType type)
      {
         Busy(false);
         Text = message;
         this.type = type;
         refresh();
      }

      public void Uninitialized(string message) => ShowMessage(message, MessageProgressType.Uninitialized);

      public void Message(string message) => ShowMessage(message, MessageProgressType.Message);

      public void Exception(Exception exception) => ShowMessage(exception.Message, MessageProgressType.Exception);

      public void Success(string message) => ShowMessage(message, MessageProgressType.Success);

      public void Failure(string message) => ShowMessage(message, MessageProgressType.Failure);

      public void Selected(string message) => ShowMessage(message, MessageProgressType.Selected);

      public void Unselected(string message) => ShowMessage(message, MessageProgressType.Unselected);

      public void Tape()
      {
         type = MessageProgressType.Tape;
         refresh();
      }

      public void ProgressText(string text)
      {
         Text = text;
         type = MessageProgressType.ProgressIndefinite;

         refresh();
      }

      public bool Clickable => _clickText.IsSome;

      public string ClickText
      {
         get => _clickText.DefaultTo(() => text);
         set => _clickText = maybe(value.IsNotEmpty(), () => value);
      }

      public int Minimum { get; set; }

      public int Maximum
      {
         get => maximum;
         set
         {
            maximum = value;
            index = Minimum;
         }
      }

      public void Progress(int value, string text = "")
      {
         this.value = value;
         Text = text;

         type = MessageProgressType.ProgressDefinite;

         refresh();
      }

      public void Progress(string text)
      {
         value = index++;
         Text = text;

         type = MessageProgressType.ProgressDefinite;

         refresh();
      }

      [Obsolete("Use Busy with string argument")]
      public void BusyText(string text)
      {
         Text = text;
         type = MessageProgressType.BusyText;
         this.Do(() => timer.Enabled = true);
         refresh();
      }

      public void Busy(string text)
      {
         Text = text;
         type = MessageProgressType.BusyText;
         this.Do(() => timer.Enabled = true);
         refresh();
      }

      [Obsolete("Use SetUp")]
      public void AddToControls(ControlCollection controls, bool fill = true)
      {
         Font = new Font("Consolas", 12);
         if (fill)
         {
            Dock = DockStyle.Fill;
         }

         controls.Add(this);
      }

      protected void writeText(Graphics graphics, string text, bool center, Font font)
      {
         var foreColor = foreColors[type];
         var flags = TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix;
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

      protected void writeText(Graphics graphics, string text, bool center)
      {
         var font = getFont(type);
         writeText(graphics, text, center, font);
      }

      protected void writeText(Graphics graphics) => writeText(graphics, text, Center);

      protected override void OnPaint(PaintEventArgs e)
      {
         base.OnPaint(e);

         switch (type)
         {
            case MessageProgressType.ProgressIndefinite:
               writeText(e.Graphics);
               break;
            case MessageProgressType.ProgressDefinite:
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
            case MessageProgressType.BusyText:
            {
               var textRectangle = busyTextProcessor.TextRectangle;
               var font = getFont(type);
               var foreColor = foreColors[type];
               var flags = TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix;
               if (Center)
               {
                  flags |= TextFormatFlags.HorizontalCenter;
               }

               TextRenderer.DrawText(e.Graphics, text, font, textRectangle, foreColor, flags);
               break;
            }
            default:
            {
               if (type != MessageProgressType.Tape)
               {
                  writeText(e.Graphics);
               }

               break;
            }
         }

         if (Clickable)
         {
            var color = foreColors[type];
            using var pen = new Pen(color, 4);
            e.Graphics.DrawLine(pen, ClientRectangle.Right - 4, 4, ClientRectangle.Right - 4, ClientRectangle.Bottom - 4);

            if (mouseInside)
            {
               using var smallFont = new Font("Segoe UI", 8);
               var size = TextRenderer.MeasureText(e.Graphics, "↓", smallFont);
               var location = new Point(ClientRectangle.Right - size.Width - 8, ClientRectangle.Top + 8);
               TextRenderer.DrawText(e.Graphics, "↓", smallFont, new Rectangle(location, size), color);
               using var linePen = new Pen(color, 1);
               e.Graphics.DrawLine(linePen, location.X, location.Y + size.Height, location.X + size.Width, location.Y + size.Height);
            }

            if (mouseDown)
            {
               using var dashedPen = new Pen(color, 1);
               dashedPen.DashStyle = DashStyle.Dash;
               var rectangle = ClientRectangle;
               rectangle.Inflate(-2, -2);
               e.Graphics.DrawRectangle(dashedPen, rectangle);
            }
         }
      }

      protected override void OnPaintBackground(PaintEventArgs pevent)
      {
         base.OnPaintBackground(pevent);

         switch (type)
         {
            case MessageProgressType.Tape:
            {
               using var brush = new HatchBrush(HatchStyle.BackwardDiagonal, Color.Black, Color.Gold);
               pevent.Graphics.FillRectangle(brush, ClientRectangle);
               break;
            }
            case MessageProgressType.ProgressIndefinite or MessageProgressType.Busy:
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
            case MessageProgressType.ProgressDefinite:
            {
               using var coralBrush = new SolidBrush(Color.Coral);
               pevent.Graphics.FillRectangle(coralBrush, ClientRectangle);
               var width = ClientRectangle.Width;
               var percentWidth = getPercentage(width);
               var location = ClientRectangle.Location;
               var size = new Size(percentWidth, ClientRectangle.Height);
               var rectangle = new Rectangle(location, size);
               using var cornflowerBlueBrush = new SolidBrush(Color.CornflowerBlue);
               pevent.Graphics.FillRectangle(cornflowerBlueBrush, rectangle);
               break;
            }
            case MessageProgressType.Unselected:
            {
               using var brush = new SolidBrush(Color.White);
               pevent.Graphics.FillRectangle(brush, ClientRectangle);

               using var pen = new Pen(Color.DarkGray, 10);
               pevent.Graphics.DrawRectangle(pen, ClientRectangle);
               break;
            }
            case MessageProgressType.Selected:
            {
               using var brush = new SolidBrush(Color.White);
               pevent.Graphics.FillRectangle(brush, ClientRectangle);

               using var pen = new Pen(Color.Black, 10);
               pevent.Graphics.DrawRectangle(pen, ClientRectangle);
               break;
            }
            case MessageProgressType.BusyText:
            {
               using var brush = new SolidBrush(Color.Blue);
               pevent.Graphics.FillRectangle(brush, ClientRectangle);

               busyTextProcessor.OnPaint(pevent);

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

      protected override void OnMouseEnter(EventArgs e)
      {
         base.OnMouseEnter(e);

         if (!mouseInside)
         {
            mouseInside = true;
            refresh();
         }
      }

      protected override void OnMouseLeave(EventArgs e)
      {
         base.OnMouseLeave(e);

         if (mouseInside)
         {
            mouseInside = false;
            refresh();
         }
      }

      protected override void OnMouseDown(MouseEventArgs e)
      {
         base.OnMouseDown(e);

         if (!mouseDown)
         {
            mouseDown = true;
            refresh();
         }
      }

      protected override void OnMouseUp(MouseEventArgs e)
      {
         base.OnMouseUp(e);

         if (mouseDown)
         {
            mouseDown = false;
            refresh();
         }
      }

      protected int getPercentage() => (int)((float)value / maximum * 100);

      protected int getPercentage(int width) => (int)((float)value / maximum * width);

      public int Index(bool increment) => increment ? index++ : index;

      public void Busy(bool enabled)
      {
         Text = "";
         type = MessageProgressType.Busy;
         this.Do(() => timer.Enabled = enabled);
      }
   }
}