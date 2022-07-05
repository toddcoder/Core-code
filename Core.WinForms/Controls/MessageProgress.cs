using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using Core.Collections;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using Core.WinForms.ControlWrappers;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls
{
   public class MessageProgress : UserControl
   {
      protected const string BUSY_TEXT_PROCESSOR_NOT_INITIALIZED = "BusyTextProcessor not initialized";
      protected const string PROGRESS_DEFINITE_PROCESSOR_NOT_INITIALIZED = "Progress Definite Processor not initialized";
      protected const string CHECK_MARK = "\u2713";

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
            [MessageProgressType.BusyText] = Color.White,
            [MessageProgressType.Automatic] = Color.Black,
            [MessageProgressType.Disabled] = Color.LightGray,
            [MessageProgressType.Caution] = Color.White
         };
         globalBackColors = new Hash<MessageProgressType, Color>
         {
            [MessageProgressType.Uninitialized] = Color.Gray,
            [MessageProgressType.Message] = Color.Blue,
            [MessageProgressType.Exception] = Color.Red,
            [MessageProgressType.Success] = Color.Green,
            [MessageProgressType.Failure] = Color.Gold,
            [MessageProgressType.Selected] = Color.FromArgb(0, 127, 0),
            [MessageProgressType.Unselected] = Color.FromArgb(127, 0, 0),
            [MessageProgressType.Automatic] = Color.White,
            [MessageProgressType.Disabled] = Color.DarkGray,
            [MessageProgressType.Caution] = Color.CadetBlue
         };
         globalStyles = new Hash<MessageProgressType, MessageStyle>
         {
            [MessageProgressType.Uninitialized] = MessageStyle.Italic,
            [MessageProgressType.Message] = MessageStyle.None,
            [MessageProgressType.Exception] = MessageStyle.Bold,
            [MessageProgressType.Success] = MessageStyle.Bold,
            [MessageProgressType.Failure] = MessageStyle.Bold,
            [MessageProgressType.BusyText] = MessageStyle.ItalicBold,
            [MessageProgressType.Caution] = MessageStyle.Bold
         };
      }

      public static Hash<MessageProgressType, Color> GlobalForeColors => globalForeColors;

      public static Hash<MessageProgressType, Color> GlobalBackColors => globalBackColors;

      public static Hash<MessageProgressType, MessageStyle> GlobalStyles => globalStyles;

      protected Font italicFont;
      protected Font boldFont;
      protected Font italicBoldFont;
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
      protected LateLazy<BusyTextProcessor> busyTextProcessor;
      protected LateLazy<ProgressDefiniteProcessor> progressDefiniteProcessor;
      protected Maybe<int> _percentage;
      protected Maybe<Color> _foreColor;
      protected Maybe<Color> _backColor;
      protected Maybe<MessageStyle> _style;
      protected Maybe<MessageProgressType> _lastType;
      protected Maybe<bool> _lastEnabled;
      protected Maybe<Color> _lastForeColor;
      protected Maybe<Color> _lastBackColor;
      protected Maybe<MessageStyle> _lastStyle;
      protected Maybe<Image> _image;
      protected List<SubText> subTexts;

      public event EventHandler<AutomaticMessageArgs> AutomaticMessage;
      public event EventHandler<PaintEventArgs> Painting;
      public event EventHandler<PaintEventArgs> PaintingBackground;

      public MessageProgress(Form form, bool center = false, bool is3D = true)
      {
         Center = center;
         Is3D = is3D;

         italicFont = new Font(base.Font, FontStyle.Italic);
         boldFont = new Font(base.Font, FontStyle.Bold);
         italicBoldFont = new Font(base.Font, FontStyle.Italic | FontStyle.Bold);

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

         _lastType = nil;
         _lastEnabled = nil;
         _lastForeColor = nil;
         _lastBackColor = nil;
         _lastStyle = nil;
         _image = nil;
         subTexts = new List<SubText>();

         timer = new Timer
         {
            Interval = 100,
            Enabled = false
         };
         timer.Tick += (_, _) =>
         {
            if (Enabled)
            {
               switch (type)
               {
                  case MessageProgressType.BusyText:
                     busyTextProcessor.Value.OnTick();
                     break;
                  case MessageProgressType.Automatic:
                  {
                     var args = new AutomaticMessageArgs();
                     AutomaticMessage?.Invoke(this, args);
                     if (args.GetText().Map(out var automaticText))
                     {
                        Text = automaticText;
                     }

                     break;
                  }
               }
            }

            this.Do(Refresh);
         };

         Minimum = 1;
         maximum = 0;

         toolTip = new ToolTip { IsBalloon = true };
         toolTip.SetToolTip(this, "");

         busyTextProcessor = new LateLazy<BusyTextProcessor>(true, BUSY_TEXT_PROCESSOR_NOT_INITIALIZED);
         progressDefiniteProcessor = new LateLazy<ProgressDefiniteProcessor>(errorMessage: PROGRESS_DEFINITE_PROCESSOR_NOT_INITIALIZED);
         Resize += (_, _) =>
         {
            busyTextProcessor.ActivateWith(() => new BusyTextProcessor(Color.White, ClientRectangle));
            progressDefiniteProcessor.Reset();
         };
         _percentage = nil;

         _foreColor = nil;
         _backColor = nil;
         _style = nil;

         form.Controls.Add(this);
      }

      public MessageProgressType Type => type;

      public bool Checked { get; set; }

      public void SetForeColor(Color foreColor) => _foreColor = foreColor;

      public void SetBackColor(Color backColor) => _backColor = backColor;

      public void SetStyle(MessageStyle style) => _style = style;

      protected void setUpFont(string fontName, float fontSize)
      {
         Font = new Font(fontName, fontSize);
      }

      protected void setUpDimensions(int x, int y, int width, int height, string fontName, float fontSize)
      {
         AutoSize = false;
         Location = new Point(x, y);
         Size = new Size(width, height);
         setUpFont(fontName, fontSize);
      }

      public void SetUp(int x, int y, int width, int height, AnchorStyles anchor, string fontName = "Consolas", float fontSize = 12f)
      {
         setUpDimensions(x, y, width, height, fontName, fontSize);
         Anchor = anchor;
      }

      public void SetUp(int x, int y, int width, int height, DockStyle dock, string fontName = "Consolas", float fontSize = 12f)
      {
         setUpDimensions(x, y, width, height, fontName, fontSize);
         Dock = dock;
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

      public void SetUpInPanel(Panel panel, string fontName = "Consolas", float fontSize = 12f, DockStyle dockStyle = DockStyle.Fill)
      {
         Dock = dockStyle;
         panel.Controls.Add(this);
         setUpFont(fontName, fontSize);
      }

      public void SetUp(int x, int y, int width, int height, string fontName = "Consolas", float fontSize = 12f)
      {
         setUpDimensions(x, y, width, height, fontName, fontSize);
      }

      public override Font Font
      {
         get => base.Font;
         set
         {
            base.Font = value;
            italicFont = new Font(base.Font, FontStyle.Italic);
            boldFont = new Font(base.Font, FontStyle.Bold);
            italicBoldFont = new Font(base.Font, FontStyle.Italic | FontStyle.Bold);
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

      public Image Image
      {
         set
         {
            _image = value;
            refresh();
         }
      }

      public void ClearImage()
      {
         _image = nil;
         refresh();
      }

      public bool StretchImage { get; set; }

      protected Font getFont() => getStyle() switch
      {
         MessageStyle.None => Font,
         MessageStyle.Italic => italicFont,
         MessageStyle.Bold => boldFont,
         MessageStyle.ItalicBold => italicBoldFont,
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

      public void Caution(string message) => ShowMessage(message, MessageProgressType.Caution);

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

      public void Result(Result<(string, MessageProgressType)> _result)
      {
         if (_result.Map(out var message, out var messageProgressType, out var exception))
         {
            ShowMessage(message, messageProgressType);
         }
         else
         {
            Exception(exception);
         }
      }

      public bool Clickable => _clickText.IsSome;

      public string ClickText
      {
         get => _clickText.DefaultTo(() => text);
         set
         {
            _clickText = maybe(value.IsNotEmpty(), () => value);
            this.Do(setToolTip);
         }
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

      public void Progress(int value, string text = "", bool asPercentage = false)
      {
         if (asPercentage)
         {
            _percentage = value;
         }
         else
         {
            this.value = value;
         }

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
         busyTextProcessor.ActivateWith(() => new BusyTextProcessor(Color.White, ClientRectangle));
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

      protected Color getForeColor(MessageProgressType type) => foreColors[type];

      protected Color getForeColor() => _foreColor.DefaultTo(() => foreColors[type]);

      protected Color getBackColor(MessageProgressType type) => backColors[type];

      protected Color getBackColor() => _backColor.DefaultTo(() => backColors[type]);

      protected MessageStyle getStyle(MessageProgressType type) => styles[type];

      protected MessageStyle getStyle() => _style.DefaultTo(() => styles[type]);

      protected Rectangle getCheckRectangle(Graphics graphics)
      {
         using var font = new Font("Verdana", 12);
         var size = TextRenderer.MeasureText(graphics, CHECK_MARK, font);
         var location = new Point(4, 4);

         return new Rectangle(location, size);
      }

      protected override void OnPaint(PaintEventArgs e)
      {
         base.OnPaint(e);

         var progressText = new MessageProgressText(Center)
         {
            Rectangle = ClientRectangle,
            Font = getFont(),
            Color = getForeColor()
         };

         switch (type)
         {
            case MessageProgressType.ProgressIndefinite:
               progressText.Write(text, e.Graphics);
               break;
            case MessageProgressType.ProgressDefinite:
            {
               var percentText = $"{getPercentage()}%";
               progressText.Rectangle = progressDefiniteProcessor.Value.PercentRectangle;
               progressText.Center(true);
               progressText.Color = Color.Black;
               progressText.Write(percentText, e.Graphics);

               progressText.Rectangle = progressDefiniteProcessor.Value.TextRectangle;
               progressText.Color = getForeColor();
               progressText.Write(text, e.Graphics);
               break;
            }
            case MessageProgressType.BusyText:
            {
               progressText.Rectangle = busyTextProcessor.Value.TextRectangle;
               progressText.Center(true);
               progressText.Write(text, e.Graphics);
               break;
            }
            default:
            {
               if (type != MessageProgressType.Tape)
               {
                  progressText.Write(text, e.Graphics);
               }

               break;
            }
         }

         foreach (var subText in subTexts)
         {
            subText.Draw(e.Graphics);
         }

         if (Clickable)
         {
            var color = getForeColor();
            using var pen = new Pen(color, 4);
            e.Graphics.DrawLine(pen, ClientRectangle.Right - 4, 4, ClientRectangle.Right - 4, ClientRectangle.Bottom - 4);

            if (mouseInside || mouseDown)
            {
               using var dashedPen = new Pen(color, 1);
               dashedPen.DashStyle = DashStyle.Dash;
               var rectangle = ClientRectangle;
               rectangle.Inflate(-2, -2);
               drawRectangle(e.Graphics, dashedPen, rectangle);
            }
         }

         if (Checked && type is not MessageProgressType.Busy && type is not MessageProgressType.BusyText)
         {
            var location = getCheckRectangle(e.Graphics).Location;

            var foreColor = getForeColor();
            using var invertedBrush = new SolidBrush(foreColor);
            var stringFormat = new StringFormat(StringFormatFlags.NoClip | StringFormatFlags.NoWrap);
            e.Graphics.HighQuality();
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            using var font = new Font("Verdana", 12);
            e.Graphics.DrawString(CHECK_MARK, font, invertedBrush, location, stringFormat);
         }

         Painting?.Invoke(this, e);
      }

      protected override void OnPaintBackground(PaintEventArgs pevent)
      {
         base.OnPaintBackground(pevent);

         switch (type)
         {
            case MessageProgressType.Tape:
            {
               using var brush = new HatchBrush(HatchStyle.BackwardDiagonal, Color.Black, Color.Gold);
               fillRectangle(pevent.Graphics, brush, ClientRectangle);
               break;
            }
            case MessageProgressType.ProgressIndefinite or MessageProgressType.Busy:
            {
               var x = random.Next(ClientRectangle.Width - 5);
               var y = random.Next(ClientRectangle.Height - 5);
               var location = new Point(x, y);
               var length = random.Next(ClientRectangle.Width - x - 5);
               using var brush = new SolidBrush(Color.DarkSlateGray);
               fillRectangle(pevent.Graphics, brush, ClientRectangle);

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
               progressDefiniteProcessor.ActivateWith(() => new ProgressDefiniteProcessor(Font, pevent.Graphics, ClientRectangle));
               progressDefiniteProcessor.Value.OnPaint(pevent.Graphics);
               var textRectangle = progressDefiniteProcessor.Value.TextRectangle;

               using var coralBrush = new SolidBrush(Color.Coral);
               fillRectangle(pevent.Graphics, coralBrush, textRectangle);
               var width = textRectangle.Width;
               var percentWidth = getPercentage(width);
               var location = textRectangle.Location;
               var size = new Size(percentWidth, textRectangle.Height);
               var rectangle = new Rectangle(location, size);
               using var cornflowerBlueBrush = new SolidBrush(Color.CornflowerBlue);
               fillRectangle(pevent.Graphics, cornflowerBlueBrush, rectangle);
               break;
            }
            case MessageProgressType.Unselected:
            {
               using var brush = new SolidBrush(Color.White);
               fillRectangle(pevent.Graphics, brush, ClientRectangle);

               using var pen = new Pen(Color.DarkGray, 10);
               drawRectangle(pevent.Graphics, pen, ClientRectangle);
               break;
            }
            case MessageProgressType.Selected:
            {
               using var brush = new SolidBrush(Color.White);
               fillRectangle(pevent.Graphics, brush, ClientRectangle);

               using var pen = new Pen(Color.Black, 10);
               drawRectangle(pevent.Graphics, pen, ClientRectangle);
               break;
            }
            case MessageProgressType.BusyText:
            {
               using var brush = new SolidBrush(Color.Teal);
               fillRectangle(pevent.Graphics, brush, ClientRectangle);

               busyTextProcessor.Value.OnPaint(pevent);

               break;
            }
            default:
            {
               var backColor = getBackColor();
               using var brush = new SolidBrush(backColor);
               fillRectangle(pevent.Graphics, brush, ClientRectangle);
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

         if (_image.Map(out var image))
         {
            if (StretchImage)
            {
               pevent.Graphics.DrawImage(image, ClientRectangle with { X = 0, Y = 0 });
            }
            else
            {
               pevent.Graphics.DrawImage(image, Point.Empty);
            }
         }

         if (Checked && type is not MessageProgressType.Busy && type is not MessageProgressType.BusyText)
         {
            var rectangle = getCheckRectangle(pevent.Graphics);

            var foreColor = getForeColor();
            using var pen = new Pen(foreColor, 2);
            pevent.Graphics.HighQuality();
            pevent.Graphics.DrawEllipse(pen, rectangle);
         }

         PaintingBackground?.Invoke(this, pevent);
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

      protected int getPercentage() => _percentage.DefaultTo(() => (int)((float)value / maximum * 100));

      protected int getPercentage(int width) => (int)((float)value / maximum * width);

      public int Index(bool increment) => increment ? index++ : index;

      public void Busy(bool enabled)
      {
         Text = "";
         type = MessageProgressType.Busy;
         this.Do(() => timer.Enabled = enabled);
      }

      public void StartAutomatic()
      {
         Text = "";
         type = MessageProgressType.Automatic;
         this.Do(() => timer.Enabled = true);
      }

      public void StopAutomatic()
      {
         this.Do(() => timer.Enabled = false);
      }

      public bool IsAutomaticRunning => timer.Enabled;

      protected override void OnEnabledChanged(EventArgs e)
      {
         base.OnEnabledChanged(e);

         if (Enabled)
         {
            if (_lastType.Map(out var lastType))
            {
               ShowMessage(text, lastType);
               _lastType = nil;
            }
            else
            {
               ShowMessage(text, MessageProgressType.Uninitialized);
            }

            if (_lastEnabled.Map(out var enabled))
            {
               timer.Enabled = enabled;
               _lastEnabled = nil;
            }

            _foreColor = _lastForeColor;
            _backColor = _lastBackColor;
            _style = _lastStyle;
         }
         else
         {
            _lastType = type;
            type = MessageProgressType.Disabled;

            _lastEnabled = timer.Enabled;
            timer.Enabled = false;

            _lastForeColor = _foreColor;
            _foreColor = nil;

            _lastBackColor = _backColor;
            _backColor = nil;

            _lastStyle = _style;
            _style = nil;

            refresh();
         }
      }

      protected virtual void drawRectangle(Graphics graphics, Pen pen, Rectangle rectangle) => graphics.DrawRectangle(pen, rectangle);

      protected virtual void fillRectangle(Graphics graphics, Brush brush, Rectangle rectangle) => graphics.FillRectangle(brush, rectangle);

      public SubText SubText(string text, int x, int y)
      {
         var subText = new SubText(text, x, y,getForeColor(), getBackColor());
         subTexts.Add(subText);

         return subText;
      }


      public void RemoveSubTextAt(int index) => subTexts.RemoveAt(index);

      public void ClearSubTexts() => subTexts.Clear();
   }
}